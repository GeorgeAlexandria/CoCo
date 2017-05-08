//------------------------------------------------------------------------------
// <copyright file="EditorClassifier.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "EditorClassifier" classification type.
    /// </summary>
    internal class EditorClassifier : IClassifier
    {
        /// <summary>
        /// Classification type.
        /// </summary>
        private readonly IClassificationType _localFieldType;

        private readonly IClassificationType _namespaceType;
        private readonly IClassificationType _parameterType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorClassifier"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal EditorClassifier(IClassificationTypeRegistryService registry)
        {
            //TODO: send ITextBuffer?
            _localFieldType = registry.GetClassificationType(Names.LocalFieldName);
            _namespaceType = registry.GetClassificationType(Names.NamespaceName);
            _parameterType = registry.GetClassificationType(Names.ParameterName);
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range
        /// of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>
        /// A list of ClassificationSpans that represent spans identified to be of this classification.
        /// </returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();

            // NOTE: Workspace can be null for "Using directive is unnecessary".
            // Also workspace can be null when solution/project failed to load and VS gave some
            // reasons of it
            Workspace workspace = span.Snapshot.TextBuffer.GetWorkspace();

            //Document document = span.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
            //var t = span.Snapshot.AsText();
            //SyntaxTree syntaxTree = document.GetSyntaxTreeAsync().Result;

            //SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(span.Snapshot.AsText());
            Document document = workspace.GetDocument(span.Snapshot.AsText());
            //Document document2 = workspace2.GetDocument(span.Snapshot.AsText());

            //SemanticModel semanticModel2 = document2.GetSemanticModelAsync().Result;
            //SyntaxNode root = document2.GetSyntaxRootAsync().Result;

            // TODO:
            SemanticModel semanticModel = document.GetSemanticModelAsync().Result;
            SyntaxTree syntaxTree = semanticModel.SyntaxTree;

            //TextSpan forNode = new TextSpan(11593, 4);

            //var t = document.GetSemanticModelAsync().Result;
            //CancellationToken cancellationToken = new CancellationToken();
            //CompilationUnitSyntax unitCompilation = syntaxTree.GetCompilationUnitRoot(cancellationToken);
            //NamespaceDeclarationSyntax
            //unitCompilation.ChildNodes().OfType<NamespaceDeclarationSyntax>().Select(x => x);
            //CSharpSyntaxTree.ParseText
            //Classifier.GetClassifiedSpans(syntaxTree.)
            //span.Snapshot.
            //span.Snapshot.GetWorkspace();

            var test = span.GetText();
            TextSpan tSpan = new TextSpan(span.Start.Position, span.Length);
            var classifiedSpans = Classifier.GetClassifiedSpans(semanticModel, tSpan, workspace)
                .Where(item => item.ClassificationType == "identifier");

            //var fcs = classifiedSpans.Where(s => s.TextSpan.OverlapsWith(tSpan));
            CompilationUnitSyntax unitCompilation = syntaxTree.GetCompilationUnitRoot();

            foreach (var item in classifiedSpans)
            {
                SyntaxNode node = unitCompilation.FindNode(item.TextSpan).SpecificHandle();

                //SyntaxNode newNode = node;
                //if (node.Kind() == SyntaxKind.Argument)
                //{
                //    newNode = (node as ArgumentSyntax).Expression;
                //}

                // NOTE: Some kind of nodes, for example ArgumentSyntax, need specific handling
                ISymbol symbol = semanticModel.GetSymbolInfo(node).Symbol ?? semanticModel.GetDeclaredSymbol(node);
                if (symbol == null)
                {
                    // TODO: Log information about the node and semantic model, because semantic model
                    // didn't retrive information from node in this case
                    continue;
                }
                switch (symbol.Kind)
                {
                    case SymbolKind.Alias:
                    case SymbolKind.ArrayType:
                    case SymbolKind.Assembly:
                    case SymbolKind.DynamicType:
                    case SymbolKind.ErrorType:
                    case SymbolKind.Event:
                    case SymbolKind.Field:
                    case SymbolKind.Label:
                    case SymbolKind.Method:
                    case SymbolKind.NetModule:
                    case SymbolKind.NamedType:
                    case SymbolKind.PointerType:
                    case SymbolKind.Property:
                    case SymbolKind.RangeVariable:
                    case SymbolKind.TypeParameter:
                    case SymbolKind.Preprocessing:
                    case SymbolKind.Discard:
                        // TODO: Log input type and span positions here
                        break;

                    case SymbolKind.Local:
                        result.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, item.TextSpan.Start, item.TextSpan.Length), _localFieldType));
                        break;

                    case SymbolKind.Namespace:
                        result.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, item.TextSpan.Start, item.TextSpan.Length), _namespaceType));
                        break;

                    case SymbolKind.Parameter:
                        result.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, item.TextSpan.Start, item.TextSpan.Length), _parameterType));
                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        #endregion
    }

    // TODO: it's temporary name
    internal static class Help
    {
        // TODO: it's temporary name
        public static SyntaxNode SpecificHandle(this SyntaxNode node) =>
            node.Kind() == SyntaxKind.Argument ? (node as ArgumentSyntax).Expression : node;

        //public static IEnumerable<ClassifiedSpan> GetClassifiedSpans(
        //    SemanticModel semanticModel,
        //    TextSpan textSpan,
        //    Workspace workspace,
        //    CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var service = workspace.Services.GetLanguageServices(semanticModel.Language).GetService<IClassificationService>();

        // var syntaxClassifiers = service.GetDefaultSyntaxClassifiers();

        // var extensionManager = workspace.Services.GetService<IExtensionManager>(); var
        // getNodeClassifiers = extensionManager.CreateNodeExtensionGetter(syntaxClassifiers, c =>
        // c.SyntaxNodeTypes); var getTokenClassifiers =
        // extensionManager.CreateTokenExtensionGetter(syntaxClassifiers, c => c.SyntaxTokenKinds);

        // var syntacticClassifications = new List<ClassifiedSpan>(); var semanticClassifications =
        // new List<ClassifiedSpan>();

        // service.AddSyntacticClassifications(semanticModel.SyntaxTree, textSpan,
        // syntacticClassifications, cancellationToken);
        // service.AddSemanticClassifications(semanticModel, textSpan, workspace, getNodeClassifiers,
        // getTokenClassifiers, semanticClassifications, cancellationToken);

        // var allClassifications = new List<ClassifiedSpan>(semanticClassifications.Where(s =>
        // s.TextSpan.OverlapsWith(textSpan))); var semanticSet = semanticClassifications.Select(s => s.TextSpan).ToSet();

        // allClassifications.AddRange(syntacticClassifications.Where( s =>
        // s.TextSpan.OverlapsWith(textSpan) && !semanticSet.Contains(s.TextSpan)));
        // allClassifications.Sort((s1, s2) => s1.TextSpan.Start - s2.TextSpan.Start);

        //    return allClassifications;
        //}

        //TODO: Check behavior for document that isn't including in solution
        public static Document GetDocument(this Workspace workspace, SourceText text)
        {
            DocumentId id = workspace.GetDocumentIdInCurrentContext(text.Container);
            if (id == null)
            {
                return null;
            }

            return !workspace.CurrentSolution.ContainsDocument(id)
                ? workspace.CurrentSolution.WithDocumentText(id, text, PreservationMode.PreserveIdentity).GetDocument(id)
                : workspace.CurrentSolution.GetDocument(id);
        }

        public static Document GetOpenDocumentInCurrentContextWithChanges(this SourceText text)
        {
            if (Workspace.TryGetWorkspace(text.Container, out var workspace))
            {
                var id = workspace.GetDocumentIdInCurrentContext(text.Container);
                if (id == null || !workspace.CurrentSolution.ContainsDocument(id))
                {
                    return null;
                }

                var sol = workspace.CurrentSolution.WithDocumentText(id, text, PreservationMode.PreserveIdentity);
                return sol.GetDocument(id);
            }

            return null;
        }
    }
}