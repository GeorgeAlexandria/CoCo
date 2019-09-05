using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using CoCo.Utils;
using FSharp.Compiler;
using FSharp.Compiler.SourceCodeServices;
using Microsoft.FSharp.Control;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Classifications.FSharp
{
    using ClassificationOptions = Dictionary<IClassificationType, ClassificationOption>;

    internal class FSharpClassifierService
    {
        private readonly struct Context
        {
            // TODO: naming
            public readonly IReadOnlyDictionary<Range.range, FSharpSymbolUse> Cache;

            public readonly List<ClassificationSpan> Result;
            public readonly SnapshotSpan SnapshotSpan;
            public readonly object Parent;

            public Context(IReadOnlyDictionary<Range.range, FSharpSymbolUse> cache, List<ClassificationSpan> result,
                SnapshotSpan snapshotSpan, object parent)
            {
                Cache = cache;
                Result = result;
                SnapshotSpan = snapshotSpan;
                Parent = parent;
            }

            public Context WithParent(object parent) => new Context(Cache, Result, SnapshotSpan, parent);
        }

        private IClassificationType _namespaceType;
        private IClassificationType _classType;
        private IClassificationType _recordType;
        private IClassificationType _unionType;
        private IClassificationType _moduleType;
        private IClassificationType _structureType;
        private IClassificationType _propertyType;

        private static FSharpClassifierService _instance;
        private readonly ClassificationOptions _classificationOptions = new ClassificationOptions();
        private ImmutableArray<IClassificationType> _classifications;

        private FSharpClassifierService(IReadOnlyDictionary<string, ClassificationInfo> classifications)
        {
            InitializeClassifications(classifications);
        }

        internal static FSharpClassifierService GetClassifier(IReadOnlyDictionary<string, ClassificationInfo> classifications) =>
            _instance ?? (_instance = new FSharpClassifierService(classifications));

        public IList<ClassificationSpan> GetClassificationSpans(FSharpParseFileResults parseResults, FSharpCheckFileResults checkResults, SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();
            var symbolsUse = FSharpAsync.RunSynchronously(checkResults.GetAllUsesOfAllSymbolsInFile(), null, null);

            // TODO: would be better to use binary search?
            var cache = new Dictionary<Range.range, FSharpSymbolUse>(symbolsUse.Length);
            foreach (var symbolUse in symbolsUse)
            {
                if (!cache.TryAdd(symbolUse.RangeAlternate, symbolUse))
                {
                    Log.Debug($"Item already exist in the cache by range {symbolUse.RangeAlternate}");
                }
            }

            var context = new Context(cache, result, span, null);
            if (parseResults.ParseTree.Value is Ast.ParsedInput.ImplFile implFile)
            {
                foreach (var moduleOrNamespace in implFile.Item.modules)
                {
                    Visit(moduleOrNamespace, context);
                }
            }

            // TODO: iterate symbols isn't allowed to classify keywords => process untyped AST in the future
            foreach (var item in symbolsUse)
            {
                var symbol = item.Symbol;

                // TODO: match symbols
                switch (symbol)
                {
                    case FSharpActivePatternCase _:
                        break;

                    case FSharpEntity _:
                        break;

                    case FSharpField _:
                        break;

                    case FSharpGenericParameter _:
                        break;

                    case FSharpMemberOrFunctionOrValue _:
                        break;

                    case FSharpParameter _:
                        break;

                    case FSharpStaticParameter _:
                        break;

                    case FSharpUnionCase _:
                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        private void Visit(Ast.SynModuleOrNamespace moduleOrNamespace, Context context)
        {
            // TODO: Handle moduleOrNamespace as namespace separate
            foreach (var item in moduleOrNamespace.decls)
            {
                Visit(item, context);
            }
        }

        private void Visit(Ast.SynModuleDecl moduleDecl, Context context)
        {
            var snapshot = context.SnapshotSpan.Snapshot;
            switch (moduleDecl)
            {
                case Ast.SynModuleDecl.Open openSyntax:
                    AddIdents(openSyntax.longDotId.Lid, _namespaceType, context);
                    break;

                case Ast.SynModuleDecl.Types typeSyntax:
                    foreach (var typeDefinition in typeSyntax.Item1)
                    {
                        foreach (var ident in typeDefinition.Item1.longId)
                        {
                            var span = ident.idRange.ToSpan(snapshot);
                            // TODO: if the one id from longId was, for example, a class, does it mean that the all id from longId would be a class?
                            if (context.Cache.TryGetValue(ident.idRange, out var symbolUse) && symbolUse.Symbol is FSharpEntity entity)
                            {
                                var type =
                                    entity.IsFSharpUnion ? _unionType :
                                    entity.IsFSharpRecord ? _recordType :
                                    entity.IsValueType ? _structureType :
                                    entity.IsClass ? _classType :
                                    null;

                                if (!(type is null))
                                {
                                    context.Result.Add(new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), type));
                                }
                            }
                        }

                        foreach (var item in typeDefinition.members)
                        {
                            Visit(item, context);
                        }
                        Visit(typeDefinition.Item2, context);
                    }
                    break;

                case Ast.SynModuleDecl.NestedModule nestedModule:
                    AddIdents(nestedModule.Item1.longId, _moduleType, context);
                    foreach (var item in nestedModule.Item3)
                    {
                        Visit(item, context);
                    }
                    break;

                case Ast.SynModuleDecl.Let letSyntax:
                    foreach (var item in letSyntax.Item2)
                    {
                        Visit(item, context);
                    }
                    break;

                case Ast.SynModuleDecl.DoExpr doExpression:
                    Visit(doExpression.Item2, context);
                    break;

                case Ast.SynModuleDecl.NamespaceFragment fragment:
                    Visit(fragment.Item, context);
                    break;

                default:
                    break;
            }
        }

        private void Visit(Ast.SynMemberDefn memberDefn, Context context)
        {
            switch (memberDefn)
            {
                case Ast.SynMemberDefn.Open open:
                    // TODO: what is it case?
                    // AddIdents(open.longId, );
                    Debug.Fail("");
                    break;

                case Ast.SynMemberDefn.LetBindings letBinndings:
                    foreach (var item in letBinndings.Item1)
                    {
                        Visit(item, context);
                    }
                    break;

                case Ast.SynMemberDefn.Inherit inherit:
                    // AddIdents(inherit.Item2, );
                    Visit(inherit.Item1, context);
                    break;

                case Ast.SynMemberDefn.ImplicitInherit inherit:
                    // AddIdents(inherit.inheritAlias, );
                    Visit(inherit.inheritType, context);
                    Visit(inherit.inheritArgs, context);
                    break;

                case Ast.SynMemberDefn.NestedType typeDef:
                    Visit(typeDef.typeDefn, context);
                    break;

                case Ast.SynMemberDefn.Member member:
                    Visit(member.memberDefn, context);
                    break;

                case Ast.SynMemberDefn.AutoProperty property:
                    AddIndent(property.ident, _propertyType, context);
                    Visit(property.synExpr, context);
                    break;

                case Ast.SynMemberDefn.ValField field:
                    Visit(field.Item1, context);
                    break;

                case Ast.SynMemberDefn.AbstractSlot slot:
                    Visit(slot.Item1, context.WithParent(slot));
                    break;

                default:
                    break;
            }
        }

        private void Visit(Ast.SynTypeDefnRepr typeDefnRepr, Context context)
        {
            switch (typeDefnRepr)
            {
                case Ast.SynTypeDefnRepr.ObjectModel objectModel:
                    foreach (var item in objectModel.Item2)
                    {
                        Visit(item, context);
                    }
                    break;

                case Ast.SynTypeDefnRepr.Simple _:
                    // TODO: what is it case?
                    break;

                default:
                    break;
            }
        }

        private void Visit(Ast.SynType type, Context context)
        {
            // todo:
        }

        private void Visit(Ast.SynTypeDefn typeDef, Context context)
        {
            // todo:
        }

        private void Visit(Ast.SynField field, Context context)
        {
            // todo:
        }

        private void Visit(Ast.SynValSig valSig, Context context)
        {
            if (!(valSig.synExpr is null))
            {
                Visit(valSig.synExpr.Value, context);
            }

            if (context.Cache.TryGetValue(valSig.ident.idRange, out var use))
            {
                if (context.Parent is Ast.SynMemberDefn)
                {
                    switch (use.Symbol)
                    {
                        case FSharpMemberOrFunctionOrValue some:
                            if (some.IsMember && (some.IsProperty || some.IsPropertyGetterMethod || some.IsPropertySetterMethod))
                            {
                                AddIndent(valSig.ident, _propertyType, context);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void Visit(Ast.SynBinding binding, Context context)
        {
            Visit(binding.headPat, context);
            Visit(binding.expr, context);
        }

        private void Visit(Ast.SynPat pattern, Context context)
        {
            // TODO:
            switch (pattern)
            {
                case Ast.SynPat.Named nameSyntax:
                    if (context.Cache.TryGetValue(nameSyntax.range, out var symbolUse))
                    {
                        switch (symbolUse.Symbol)
                        {
                            case FSharpMemberOrFunctionOrValue memberOrFunctionOrValue:
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void Visit(Ast.SynExpr expression, Context context)
        {
            // TODO:
        }

        private void AddIdents<T>(T longIds, IClassificationType classificationType, Context context) where T : IEnumerable<Ast.Ident>
        {
            var snapshot = context.SnapshotSpan.Snapshot;
            foreach (var item in longIds)
            {
                var span = item.idRange.ToSpan(snapshot);
                context.Result.Add(new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), classificationType));
            }
        }

        private void AddIndent(Ast.Ident ident, IClassificationType classificationType, Context context)
        {
            var snapshot = context.SnapshotSpan.Snapshot;
            var span = ident.idRange.ToSpan(snapshot);
            context.Result.Add(new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), classificationType));
        }

        private void InitializeClassifications(IReadOnlyDictionary<string, ClassificationInfo> classifications)
        {
            var builder = ImmutableArray.CreateBuilder<IClassificationType>(7);
            void InitializeClassification(string name, ref IClassificationType type)
            {
                var info = classifications[name];
                type = info.ClassificationType;
                _classificationOptions[type] = info.Option;
                builder.Add(type);
            }

            InitializeClassification(FSharpNames.NamespaceName, ref _namespaceType);
            InitializeClassification(FSharpNames.ClassName, ref _classType);
            InitializeClassification(FSharpNames.RecordName, ref _recordType);
            InitializeClassification(FSharpNames.UnionName, ref _unionType);
            InitializeClassification(FSharpNames.ModuleName, ref _moduleType);
            InitializeClassification(FSharpNames.StructureName, ref _structureType);
            InitializeClassification(FSharpNames.PropertyName, ref _propertyType);

            _classifications = builder.TryMoveToImmutable();
        }
    }
}