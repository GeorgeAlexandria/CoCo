using System.Collections.Generic;
using System.Collections.Immutable;
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

            public Context(IReadOnlyDictionary<Range.range, FSharpSymbolUse> cache, List<ClassificationSpan> result, SnapshotSpan snapshotSpan)
            {
                Cache = cache;
                Result = result;
                SnapshotSpan = snapshotSpan;
            }
        }

        private IClassificationType _namespaceType;
        private IClassificationType _classType;
        private IClassificationType _recordType;
        private IClassificationType _unionType;
        private IClassificationType _moduleType;
        private IClassificationType _structureType;

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

            var context = new Context(cache, result, span);
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
            Range.range range;
            Span span;
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
                            span = ident.idRange.ToSpan(snapshot);
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
                    // TODO:
                    Visit(letSyntax.Item2.Head.headPat, context);
                    break;

                case Ast.SynModuleDecl.NamespaceFragment fragment:
                    Visit(fragment.Item, context);
                    break;

                default:
                    break;
            }
        }

        private void Visit(Ast.SynMemberDefn member, Context context)
        {
            // TODO:
        }

        private void Visit(Ast.SynPat pattern, Context context)
        {
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

        private void AddIdents<T>(T longIds, IClassificationType classificationType, Context context) where T : IEnumerable<Ast.Ident>
        {
            var snapshot = context.SnapshotSpan.Snapshot;
            foreach (var item in longIds)
            {
                var span = item.idRange.ToSpan(snapshot);
                context.Result.Add(new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), classificationType));
            }
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

            _classifications = builder.TryMoveToImmutable();
        }
    }
}