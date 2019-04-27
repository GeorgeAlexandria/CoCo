using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Classifications.CSharp
{
    /// <summary>
    /// Classifies csharp code
    /// </summary>
    internal class CSharpClassifierService : ICodeClassifier
    {
        private IClassificationType _localVariableType;
        private IClassificationType _rangeVariableType;
        private IClassificationType _namespaceType;
        private IClassificationType _parameterType;
        private IClassificationType _extensionMethodType;
        private IClassificationType _methodType;
        private IClassificationType _eventType;
        private IClassificationType _propertyType;
        private IClassificationType _fieldType;
        private IClassificationType _staticMethodType;
        private IClassificationType _enumFieldType;
        private IClassificationType _aliasNamespaceType;
        private IClassificationType _constructorMethodType;
        private IClassificationType _labelType;
        private IClassificationType _constantFieldType;
        private IClassificationType _destructorMethodType;
        private IClassificationType _typeParameterType;
        private IClassificationType _classType;
        private IClassificationType _structureType;
        private IClassificationType _interfaceType;
        private IClassificationType _enumType;
        private IClassificationType _delegateType;

        private static CSharpClassifierService _instance;

        private readonly Dictionary<IClassificationType, ClassificationOption> _classificationOptions =
            new Dictionary<IClassificationType, ClassificationOption>();

        private ImmutableArray<IClassificationType> _classifications;

        private CSharpClassifierService(
            IReadOnlyDictionary<string, ClassificationInfo> classifications, IClassificationChangingService analyzingService)
        {
            InitializeClassifications(classifications);
            analyzingService.ClassificationChanged += OnClassificationsChanged;
        }

        private CSharpClassifierService(IReadOnlyDictionary<string, ClassificationInfo> classifications)
        {
            InitializeClassifications(classifications);
        }

        internal static CSharpClassifierService GetClassifier(
            IReadOnlyDictionary<string, ClassificationInfo> classifications, IClassificationChangingService analyzingService)
        {
            if (_instance is null)
            {
                _instance = new CSharpClassifierService(classifications, analyzingService);
            }
            return _instance;
        }

        internal static CSharpClassifierService GetClassifier(IReadOnlyDictionary<string, ClassificationInfo> classifications)
        {
            if (_instance is null)
            {
                _instance = new CSharpClassifierService(classifications);
            }
            return _instance;
        }

        internal static void Reset() => _instance = null;

        internal List<ClassificationSpan> GetClassificationSpans(
            Workspace workspace, SemanticModel semanticModel, SnapshotSpan span)
        {
            var spans = new List<ClassificationSpan>();

            var root = semanticModel.SyntaxTree.GetCompilationUnitRoot();
            var textSpan = new TextSpan(span.Start.Position, span.Length);
            foreach (var item in Classifier.GetClassifiedSpans(semanticModel, textSpan, workspace))
            {
                if (!ClassificationHelper.IsSupportedClassification(item.ClassificationType)) continue;

                var node = root.FindNode(item.TextSpan, true).HandleNode();
                if (!semanticModel.TryGetSymbolInfo(node, out var symbol, out var reason))
                {
                    // NOTE: handle alias in using directive
                    if ((node.Parent as NameEqualsSyntax)?.Parent is UsingDirectiveSyntax usingSyntax)
                    {
                        var aliasNameSymbol = semanticModel.GetSymbolInfo(usingSyntax.Name).Symbol;
                        IClassificationType aliasType = null;
                        if (!(aliasNameSymbol is null))
                        {
                            aliasType =
                                aliasNameSymbol.Kind == SymbolKind.Namespace ? _aliasNamespaceType :
                                aliasNameSymbol.Kind == SymbolKind.NamedType ? GetTypeClassification(aliasNameSymbol as INamedTypeSymbol) :
                                null;
                        }

                        if (!(aliasType is null))
                        {
                            AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, aliasType);
                            continue;
                        }
                    }

                    Log.Debug("Nothing is found. Span start at {0} and end at {1}", item.TextSpan.Start, item.TextSpan.End);
                    Log.Debug("Candidate Reason is {0}", reason);
                    Log.Debug("Node is {0}", node);
                    continue;
                }

                switch (symbol.Kind)
                {
                    case SymbolKind.Label:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _labelType);
                        break;

                    case SymbolKind.RangeVariable:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _rangeVariableType);
                        break;

                    case SymbolKind.Field:
                        var fieldSymbol = symbol as IFieldSymbol;
                        var fieldType =
                            fieldSymbol.Type.TypeKind == TypeKind.Enum ? _enumFieldType :
                            fieldSymbol.IsConst ? _constantFieldType :
                            _fieldType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, fieldType, node);
                        break;

                    case SymbolKind.Property:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _propertyType, node);
                        break;

                    case SymbolKind.Event:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _eventType, node);
                        break;

                    case SymbolKind.Local:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _localVariableType);
                        break;

                    case SymbolKind.Namespace:
                        var namesapceType = node.IsAliasNamespace(symbol, semanticModel) ? _aliasNamespaceType : _namespaceType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, namesapceType, node);
                        break;

                    case SymbolKind.Parameter:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _parameterType, node);
                        break;

                    case SymbolKind.Method:
                        var methodSymbol = symbol as IMethodSymbol;
                        var methodType =
                            methodSymbol.MethodKind == MethodKind.Destructor ? _destructorMethodType :
                            methodSymbol.MethodKind == MethodKind.Constructor ? _constructorMethodType :
                            methodSymbol.IsExtensionMethod ? _extensionMethodType :
                            methodSymbol.IsStatic ? _staticMethodType :
                            _methodType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, methodType, node);
                        break;

                    case SymbolKind.TypeParameter:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _typeParameterType, node);
                        break;

                    case SymbolKind.NamedType:
                        var type = GetTypeClassification(symbol as INamedTypeSymbol);
                        if (!(type is null))
                        {
                            AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, type, node);
                        }
                        break;

                    default:
                        Log.Debug("Symbol kind={0} was on position [{1}..{2}]", symbol.Kind, item.TextSpan.Start, item.TextSpan.End);
                        Log.Debug("Node is: {0}", node);
                        break;
                }
            }

            return spans;
        }

        public IClassificationType GetClassification(ISymbol symbol)
        {
            IClassificationType GetClassification()
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.Label:
                        return _labelType;

                    case SymbolKind.RangeVariable:
                        return _rangeVariableType;

                    case SymbolKind.Field:
                        var fieldSymbol = symbol as IFieldSymbol;
                        return
                            fieldSymbol.Type.TypeKind == TypeKind.Enum ? _enumFieldType :
                            fieldSymbol.IsConst ? _constantFieldType :
                            _fieldType;

                    case SymbolKind.Property:
                        return _propertyType;

                    case SymbolKind.Event:
                        return _eventType;

                    case SymbolKind.Local:
                        return _localVariableType;

                    case SymbolKind.Namespace:
                        return _namespaceType;

                    case SymbolKind.Parameter:
                        return _parameterType;

                    case SymbolKind.Method:
                        var methodSymbol = symbol as IMethodSymbol;
                        return
                            methodSymbol.MethodKind == MethodKind.Destructor ? _destructorMethodType :
                            methodSymbol.MethodKind == MethodKind.Constructor ? _constructorMethodType :
                            methodSymbol.IsExtensionMethod ? _extensionMethodType :
                            methodSymbol.IsStatic ? _staticMethodType :
                            _methodType;

                    case SymbolKind.TypeParameter:
                        return _typeParameterType;

                    case SymbolKind.NamedType:
                        return GetTypeClassification(symbol as INamedTypeSymbol);
                }
                return null;
            }

            var classification = GetClassification();
            if (classification is null) return null;

            var info = _classificationOptions[classification];
            return info.IsDisabled || info.IsDisabledInQuickInfo
                ? null
                : classification;
        }

        private IClassificationType GetTypeClassification(INamedTypeSymbol typeSymbol) =>
            typeSymbol.TypeKind == TypeKind.Class ? _classType :
            typeSymbol.TypeKind == TypeKind.Struct ? _structureType :
            typeSymbol.TypeKind == TypeKind.Interface ? _interfaceType :
            typeSymbol.TypeKind == TypeKind.Enum ? _enumType :
            typeSymbol.TypeKind == TypeKind.Delegate ? _delegateType :
            null;

        private void AppendClassificationSpan(
           List<ClassificationSpan> spans, ITextSnapshot snapshot, TextSpan span, IClassificationType type, SyntaxNode node = null)
        {
            var info = _classificationOptions[type];
            if (info.IsDisabled || info.IsDisabledInEditor) return;

            if (node is null || !info.IsDisabledInXml || !node.IsPartOfStructuredTrivia() || !node.IsDescendantXmlDocComment())
            {
                spans.Add(new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), type));
            }
        }

        private void OnClassificationsChanged(ClassificationsChangedEventArgs args)
        {
            foreach (var classification in _classifications)
            {
                if (args.ChangedClassifications.TryGetValue(classification, out var option))
                {
                    _classificationOptions[classification] = option;
                }
            }
        }

        private void InitializeClassifications(IReadOnlyDictionary<string, ClassificationInfo> classifications)
        {
            var builder = ImmutableArray.CreateBuilder<IClassificationType>(17);
            void InitializeClassification(string name, ref IClassificationType type)
            {
                var info = classifications[name];
                type = info.ClassificationType;
                _classificationOptions[type] = info.Option;
                builder.Add(type);
            }

            InitializeClassification(CSharpNames.LocalVariableName, ref _localVariableType);
            InitializeClassification(CSharpNames.RangeVariableName, ref _rangeVariableType);
            InitializeClassification(CSharpNames.NamespaceName, ref _namespaceType);
            InitializeClassification(CSharpNames.ParameterName, ref _parameterType);
            InitializeClassification(CSharpNames.ExtensionMethodName, ref _extensionMethodType);
            InitializeClassification(CSharpNames.MethodName, ref _methodType);
            InitializeClassification(CSharpNames.EventName, ref _eventType);
            InitializeClassification(CSharpNames.PropertyName, ref _propertyType);
            InitializeClassification(CSharpNames.FieldName, ref _fieldType);
            InitializeClassification(CSharpNames.StaticMethodName, ref _staticMethodType);
            InitializeClassification(CSharpNames.EnumFieldName, ref _enumFieldType);
            InitializeClassification(CSharpNames.AliasNamespaceName, ref _aliasNamespaceType);
            InitializeClassification(CSharpNames.ConstructorName, ref _constructorMethodType);
            InitializeClassification(CSharpNames.LabelName, ref _labelType);
            InitializeClassification(CSharpNames.ConstantFieldName, ref _constantFieldType);
            InitializeClassification(CSharpNames.DestructorName, ref _destructorMethodType);
            InitializeClassification(CSharpNames.TypeParameterName, ref _typeParameterType);
            InitializeClassification(CSharpNames.ClassName, ref _classType);
            InitializeClassification(CSharpNames.StructureName, ref _structureType);
            InitializeClassification(CSharpNames.InterfaceName, ref _interfaceType);
            InitializeClassification(CSharpNames.EnumName, ref _enumType);
            InitializeClassification(CSharpNames.DelegateName, ref _delegateType);

            _classifications = builder.TryMoveToImmutable();
        }
    }
}