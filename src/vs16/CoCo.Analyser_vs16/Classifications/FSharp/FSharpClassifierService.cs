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

            public Context WithParent<T>(T parent) where T : class => new Context(Cache, Result, SnapshotSpan, parent);
        }

        private IClassificationType _namespaceType;
        private IClassificationType _classType;
        private IClassificationType _recordType;
        private IClassificationType _unionType;
        private IClassificationType _moduleType;
        private IClassificationType _structureType;
        private IClassificationType _propertyType;
        private IClassificationType _fieldType;
        private IClassificationType _selfIdentifierName;

        private static FSharpClassifierService _instance;
        private readonly ClassificationOptions _classificationOptions = new ClassificationOptions();
        private ImmutableArray<IClassificationType> _classifications;

        private FSharpClassifierService(IReadOnlyDictionary<string, ClassificationInfo> classifications)
        {
            InitializeClassifications(classifications);
        }

        internal static FSharpClassifierService GetClassifier(IReadOnlyDictionary<string, ClassificationInfo> classifications) =>
            _instance ?? (_instance = new FSharpClassifierService(classifications));

        public List<ClassificationSpan> GetClassificationSpans(FSharpParseFileResults parseResults, FSharpCheckFileResults checkResults, SnapshotSpan span)
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
            var type =
                moduleOrNamespace.kind.IsDeclaredNamespace ? _namespaceType :
                moduleOrNamespace.kind.IsNamedModule ? _moduleType :
                null;

            if (type.IsNotNull())
            {
                AddIdents(moduleOrNamespace.longId, type, context);
            }

            foreach (var item in moduleOrNamespace.decls)
            {
                Visit(item, context);
            }
        }

        private void Visit(Ast.SynModuleDecl moduleDecl, Context context)
        {
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
                            // TODO: if the one id from longId was, for example, a class, does it mean that the all id from longId would be a class?
                            if (context.Cache.TryGetValue(ident.idRange, out var symbolUse) && symbolUse.Symbol is FSharpEntity entity)
                            {
                                var type =
                                    entity.IsFSharpUnion ? _unionType :
                                    entity.IsFSharpRecord ? _recordType :
                                    entity.IsValueType ? _structureType :
                                    entity.IsClass ? _classType :
                                    null;

                                if (type.IsNotNull())
                                {
                                    AddIdent(ident, type, context);
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
                    Log.Debug("Ast type {0} doesn't support in module declaration", moduleDecl.GetType());
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
                    AddIdent(property.ident, _propertyType, context);
                    Visit(property.synExpr, context);
                    break;

                case Ast.SynMemberDefn.ValField field:
                    Visit(field.Item1, context);
                    break;

                case Ast.SynMemberDefn.AbstractSlot slot:
                    Visit(slot.Item1, context.WithParent(slot));
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in member definition", memberDefn.GetType());
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

                case Ast.SynTypeDefnRepr.Simple simple:
                    Visit(simple.Item1, context);
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in type definition", typeDefnRepr.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynType type, Context context)
        {
            switch (type)
            {
                case Ast.SynType.AnonRecd record:
                    foreach (var (ident, fieldType) in record.typeNames)
                    {
                        AddIdent(ident, _fieldType, context);
                        Visit(fieldType, context);
                    }
                    break;

                case Ast.SynType.Array array:
                    Visit(array.elementType, context);
                    break;

                case Ast.SynType.Fun fun:
                    Visit(fun.argType, context);
                    Visit(fun.returnType, context);
                    break;

                case Ast.SynType.LongIdent ident:
                    foreach (var item in ident.longDotId.Lid)
                    {
                        if (context.Cache.TryGetValue(item.idRange, out var use) && use.Symbol is FSharpEntity entity)
                        {
                            var classification =
                                entity.IsNamespace ? _namespaceType :
                                entity.IsFSharpModule ? _moduleType :
                                entity.IsFSharpRecord ? _recordType :
                                entity.IsFSharpUnion ? _unionType :
                                entity.IsValueType ? _structureType :
                                entity.IsClass ? _classType :
                                null;
                            if (classification.IsNotNull())
                            {
                                AddIdent(item, classification, context);
                            }
                        }
                    }
                    break;

                case Ast.SynType.Tuple tuple:
                    foreach (var (_, typeName) in tuple.typeNames)
                    {
                        Visit(typeName, context);
                    }
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in type", type.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynTypeDefn typeDef, Context context)
        {
            Visit(typeDef.Item1, context);
            Visit(typeDef.Item2, context);

            foreach (var item in typeDef.members)
            {
                Visit(item, context);
            }
        }

        private void Visit(Ast.SynComponentInfo componentInfo, Context context)
        {
            // TODO: attributes, type parameters
            foreach (var item in componentInfo.longId)
            {
                if (context.Cache.TryGetValue(item.idRange, out var use) && use.Symbol is FSharpEntity entity)
                {
                    var type =
                        entity.IsNamespace ? _namespaceType :
                        entity.IsFSharpModule ? _moduleType :
                        entity.IsFSharpRecord ? _recordType :
                        entity.IsFSharpUnion ? _unionType :
                        entity.IsValueType ? _structureType :
                        entity.IsClass ? _classType :
                        null;
                    if (type.IsNotNull())
                    {
                        AddIdent(item, type, context);
                    }
                }
            }
        }

        private void Visit(Ast.SynTypeDefnSimpleRepr synTypeDefn, Context context)
        {
            // TODO:
            switch (synTypeDefn)
            {
                case Ast.SynTypeDefnSimpleRepr.Record record:
                    foreach (var item in record.recordFields)
                    {
                        Visit(item, context);
                    }
                    break;

                case Ast.SynTypeDefnSimpleRepr.Union union:
                    foreach (var item in union.unionCases)
                    {
                        Visit(item, context);
                    }
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in simple type definition", synTypeDefn.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynUnionCase unionCase, Context context)
        {
            AddIdent(unionCase.ident, _unionType, context);
            Visit(unionCase.Item3, context);
        }

        private void Visit(Ast.SynUnionCaseType unionCaseType, Context context)
        {
            switch (unionCaseType)
            {
                case Ast.SynUnionCaseType.UnionCaseFields unionCaseFields:
                    foreach (var item in unionCaseFields.cases)
                    {
                        Visit(item, context.WithParent(unionCaseFields));
                    }
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in union case type", unionCaseType.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynField field, Context context)
        {
            // NOTE: doesn't classify argument in union case declaration
            if (field.Item3.IsSome() && !(context.Parent is Ast.SynUnionCaseType.UnionCaseFields))
            {
                AddIdent(field.Item3.Value, _fieldType, context);
            }
            Visit(field.Item4, context);
        }

        private void Visit(Ast.SynValSig valSig, Context context)
        {
            if (valSig.synExpr.IsSome())
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
                                AddIdent(valSig.ident, _propertyType, context);
                            }
                            break;

                        default:
                            Log.Debug("Symbol type {0} doesn't support in valsig", use.Symbol.GetType());
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
                                Log.Debug("Symbol type {0} doesn't support in pattern", symbolUse.Symbol.GetType());
                                break;
                        }
                    }
                    break;

                case Ast.SynPat.LongIdent longIndent:
                    // TODO: what's about another items?
                    Visit(longIndent.longDotId, context);
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in pattern", pattern.GetType());
                    break;
            }
        }

        private void Visit(Ast.LongIdentWithDots identWithDots, Context context)
        {
            if (identWithDots.Lid.IsEmpty) return;

            var idents = identWithDots.Lid;
            var first = identWithDots.Lid.Head;
            if (context.Cache.TryGetValue(first.idRange, out var use))
            {
                if (use.Symbol is FSharpMemberOrFunctionOrValue some && some.IsMemberThisValue)
                {
                    AddIdent(first, _selfIdentifierName, context);
                    idents = idents.Tail;
                }
            }

            // TODO: if the one id from idents was, for example, a field, does it mean that the all id from idents would be a field?
            foreach (var item in idents)
            {
                if (context.Cache.TryGetValue(item.idRange, out use) && use.Symbol is FSharpMemberOrFunctionOrValue some)
                {
                    // TODO:
                    if (some.IsProperty || some.IsPropertyGetterMethod || some.IsPropertySetterMethod)
                    {
                        AddIdent(item, _propertyType, context);
                    }
                }
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

        private void AddIdent(Ast.Ident ident, IClassificationType classificationType, Context context)
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
            InitializeClassification(FSharpNames.FieldName, ref _fieldType);
            InitializeClassification(FSharpNames.SelfIdentifierName, ref _selfIdentifierName);

            _classifications = builder.TryMoveToImmutable();
        }
    }
}