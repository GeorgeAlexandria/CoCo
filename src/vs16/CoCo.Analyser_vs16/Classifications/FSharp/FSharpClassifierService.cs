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
        private IClassificationType _namespaceType;
        private IClassificationType _classType;
        private IClassificationType _recordType;
        private IClassificationType _unionType;

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

            if (parseResults.ParseTree.Value is Ast.ParsedInput.ImplFile implFile)
            {
                foreach (var moduleOrNamespace in implFile.Item.modules)
                {
                    // TODO: Handle moduleOrNamespace as namespace separate
                    foreach (var item in moduleOrNamespace.decls)
                    {
                        Visit(item, cache, result, span);
                    }

                    System.Console.WriteLine();
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

        private void Visit(Ast.SynModuleDecl moduleDecl, Dictionary<Range.range, FSharpSymbolUse> cache, List<ClassificationSpan> result, SnapshotSpan snapshotSpan)
        {
            switch (moduleDecl)
            {
                case Ast.SynModuleDecl.Open openSyntax:
                    foreach (var item in openSyntax.longDotId.Lid)
                    {
                        var span = item.idRange.ToSpan(snapshotSpan.Snapshot);
                        result.Add(new ClassificationSpan(new SnapshotSpan(snapshotSpan.Snapshot, span.Start, span.Length), _namespaceType));
                    }
                    break;

                case Ast.SynModuleDecl.Types typeSyntax:
                    foreach (var typeDefinition in typeSyntax.Item1)
                    {
                        var range = typeDefinition.Item1.longId.Head.idRange;
                        var span = range.ToSpan(snapshotSpan.Snapshot);
                        if (cache.TryGetValue(range, out var symbolUse) && symbolUse.Symbol is FSharpEntity entity)
                        {
                            var type =
                                entity.IsFSharpUnion ? _unionType :
                                entity.IsFSharpRecord ? _recordType :
                                entity.IsClass ? _classType :
                                null;

                            if (!(type is null))
                            {
                                result.Add(new ClassificationSpan(new SnapshotSpan(snapshotSpan.Snapshot, span.Start, span.Length), _classType));
                            }
                        }

                        foreach (var item in typeDefinition.members)
                        {
                            Visit(item, cache, result, snapshotSpan);
                        }
                    }
                    break;

                case Ast.SynModuleDecl.Let letSyntax:
                    Visit(letSyntax.Item2.Head.headPat, cache, result);
                    break;

                default:
                    break;
            }
        }

        private void Visit(Ast.SynMemberDefn member, Dictionary<Range.range, FSharpSymbolUse> cache, List<ClassificationSpan> result, SnapshotSpan snapshotSpan)
        {
            // TODO:
        }

        private void Visit(Ast.SynPat pattern, Dictionary<Range.range, FSharpSymbolUse> cache, List<ClassificationSpan> result)
        {
            switch (pattern)
            {
                case Ast.SynPat.Named nameSyntax:
                    if (cache.TryGetValue(nameSyntax.range, out var symbolUse))
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

            _classifications = builder.TryMoveToImmutable();
        }
    }
}