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
            public readonly object Parent;
            public readonly ImmutableHashSet<string> Params;

            public Context(object parent, ImmutableHashSet<string> @params)
            {
                Parent = parent;
                Params = @params;
            }

            public Context WithParent<T>(T parent) where T : class => new Context(parent, Params);

            public Context WithParams(ImmutableHashSet<string> @params) => new Context(Parent, @params);
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
        private IClassificationType _parameterName;
        private IClassificationType _enumType;
        private IClassificationType _enumFieldName;
        private IClassificationType _localBindingValueName;
        private IClassificationType _moduleFunctionName;
        private IClassificationType _methodName;
        private IClassificationType _staticMethodName;

        private static FSharpClassifierService _instance;
        private readonly ClassificationOptions _classificationOptions = new ClassificationOptions();
        private ImmutableArray<IClassificationType> _classifications;

        private Context _context;

        private List<ClassificationSpan> _result;
        private SnapshotSpan _snapshotSpan;
        private Dictionary<Range.range, FSharpSymbolUse> _cache;

        private FSharpClassifierService(IReadOnlyDictionary<string, ClassificationInfo> classifications)
        {
            InitializeClassifications(classifications);
        }

        internal static FSharpClassifierService GetClassifier(IReadOnlyDictionary<string, ClassificationInfo> classifications) =>
            _instance ?? (_instance = new FSharpClassifierService(classifications));

        public List<ClassificationSpan> GetClassificationSpans(
            FSharpParseFileResults parseResults, FSharpCheckFileResults checkResults, SnapshotSpan span)
        {
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

            try
            {
                _result = new List<ClassificationSpan>();
                _snapshotSpan = span;
                _cache = cache;

                _context = new Context(null, ImmutableHashSet<string>.Empty);
                if (parseResults.ParseTree.Value is Ast.ParsedInput.ImplFile implFile)
                {
                    foreach (var moduleOrNamespace in implFile.Item.modules)
                    {
                        Visit(moduleOrNamespace);
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

                return _result;
            }
            finally
            {
                _result = default;
                _snapshotSpan = default;
                _cache = default;
            }
        }

        private void Visit(Ast.SynModuleOrNamespace moduleOrNamespace)
        {
            var type =
                moduleOrNamespace.kind.IsDeclaredNamespace ? _namespaceType :
                moduleOrNamespace.kind.IsNamedModule ? _moduleType :
                null;

            if (type.IsNotNull())
            {
                AddIdents(moduleOrNamespace.longId, type);
            }

            foreach (var item in moduleOrNamespace.decls)
            {
                Visit(item);
            }
        }

        private void Visit(Ast.SynModuleDecl moduleDecl)
        {
            switch (moduleDecl)
            {
                case Ast.SynModuleDecl.Open openSyntax:
                    AddIdents(openSyntax.longDotId.Lid, _namespaceType);
                    break;

                case Ast.SynModuleDecl.Types typeSyntax:
                    foreach (var typeDefinition in typeSyntax.Item1)
                    {
                        foreach (var ident in typeDefinition.Item1.longId)
                        {
                            // TODO: if the one id from longId was, for example, a class, does it mean that the all id from longId would be a class?
                            if (_cache.TryGetValue(ident.idRange, out var symbolUse) && symbolUse.Symbol is FSharpEntity entity)
                            {
                                var type =
                                    entity.IsFSharpUnion ? _unionType :
                                    entity.IsFSharpRecord ? _recordType :
                                    entity.IsEnum ? _enumType :
                                    entity.IsValueType ? _structureType :
                                    entity.IsClass ? _classType :
                                    null;

                                if (type.IsNotNull())
                                {
                                    AddIdent(ident, type);
                                }
                            }
                        }

                        foreach (var item in typeDefinition.members)
                        {
                            Visit(item);
                        }
                        Visit(typeDefinition.Item2);
                    }
                    break;

                case Ast.SynModuleDecl.NestedModule nestedModule:
                    AddIdents(nestedModule.Item1.longId, _moduleType);
                    foreach (var item in nestedModule.Item3)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynModuleDecl.Let letSyntax:
                    foreach (var item in letSyntax.Item2)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynModuleDecl.DoExpr doExpression:
                    Visit(doExpression.Item2);
                    break;

                case Ast.SynModuleDecl.NamespaceFragment fragment:
                    Visit(fragment.Item);
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in module declaration", moduleDecl.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynMemberDefn memberDefn)
        {
            switch (memberDefn)
            {
                case Ast.SynMemberDefn.Open open:
                    // TODO: what is it case?
                    // AddIdents(open.longId, );
                    Log.Debug("Open was met as member definition");
                    break;

                case Ast.SynMemberDefn.LetBindings letBinndings:
                    foreach (var item in letBinndings.Item1)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynMemberDefn.Inherit inherit:
                    // AddIdents(inherit.Item2, );
                    Visit(inherit.Item1);
                    break;

                case Ast.SynMemberDefn.ImplicitInherit inherit:
                    // AddIdents(inherit.inheritAlias, );
                    Visit(inherit.inheritType);
                    Visit(inherit.inheritArgs);
                    break;

                case Ast.SynMemberDefn.NestedType typeDef:
                    Visit(typeDef.typeDefn);
                    break;

                case Ast.SynMemberDefn.Member member:
                    Visit(member.memberDefn);
                    break;

                case Ast.SynMemberDefn.AutoProperty property:
                    AddIdent(property.ident, _propertyType);
                    Visit(property.synExpr);
                    break;

                case Ast.SynMemberDefn.ValField field:
                    Visit(field.Item1);
                    break;

                case Ast.SynMemberDefn.AbstractSlot slot:
                    var oldContext = _context;
                    _context = _context.WithParent(slot);
                    Visit(slot.Item1);
                    _context = oldContext;
                    break;

                case Ast.SynMemberDefn.ImplicitCtor ctor:
                    Visit(ctor.ctorArgs);
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in member definition", memberDefn.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynSimplePats simplePats)
        {
            switch (simplePats)
            {
                case Ast.SynSimplePats.SimplePats pats:
                    foreach (var item in pats.Item1)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynSimplePats.Typed typed:
                    Visit(typed.Item1);
                    Visit(typed.Item2);
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in simple pats", simplePats.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynSimplePat simplePat)
        {
            switch (simplePat)
            {
                case Ast.SynSimplePat.Id id:
                    _context = _context.WithParams(_context.Params.Add(id.ident.idText));
                    AddIdent(id.ident, _parameterName);
                    break;

                case Ast.SynSimplePat.Attrib attribute:
                    Visit(attribute.Item1);
                    // TODO: attributes
                    break;

                case Ast.SynSimplePat.Typed typed:
                    Visit(typed.Item1);
                    Visit(typed.Item2);
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in simple pat", simplePat.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynTypeDefnRepr typeDefnRepr)
        {
            switch (typeDefnRepr)
            {
                case Ast.SynTypeDefnRepr.ObjectModel objectModel:
                    // NOTE: keep context from primary ctor to use info about a params in the let and do bindings
                    Context ctorContext = default;
                    var members = objectModel.Item2;
                    if (!members.IsEmpty && members.Head is Ast.SynMemberDefn.ImplicitCtor)
                    {
                        var current = _context;
                        Visit(members.Head);
                        ctorContext = _context;
                        _context = current;
                        members = members.Tail;
                    }

                    foreach (var item in members)
                    {
                        if (item is Ast.SynMemberDefn.LetBindings bindings /*&& !bindings.Item1.IsEmpty &&
                            bindings.Item1.Head.kind.IsDoBinding*/)
                        {
                            var current = _context;
                            _context = ctorContext;
                            Visit(item);
                            _context = current;
                            continue;
                        }
                        Visit(item);
                    }
                    break;

                case Ast.SynTypeDefnRepr.Simple simple:
                    Visit(simple.Item1);
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in type definition", typeDefnRepr.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynType type)
        {
            switch (type)
            {
                case Ast.SynType.AnonRecd record:
                    foreach (var (ident, fieldType) in record.typeNames)
                    {
                        AddIdent(ident, _fieldType);
                        Visit(fieldType);
                    }
                    break;

                case Ast.SynType.Array array:
                    Visit(array.elementType);
                    break;

                case Ast.SynType.Fun fun:
                    Visit(fun.argType);
                    Visit(fun.returnType);
                    break;

                case Ast.SynType.LongIdent ident:
                    foreach (var item in ident.longDotId.Lid)
                    {
                        if (_cache.TryGetValue(item.idRange, out var use) && use.Symbol is FSharpEntity entity)
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
                                AddIdent(item, classification);
                            }
                        }
                    }
                    break;

                case Ast.SynType.Tuple tuple:
                    foreach (var (_, typeName) in tuple.typeNames)
                    {
                        Visit(typeName);
                    }
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in type", type.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynTypeDefn typeDef)
        {
            Visit(typeDef.Item1);
            Visit(typeDef.Item2);

            foreach (var item in typeDef.members)
            {
                Visit(item);
            }
        }

        private void Visit(Ast.SynComponentInfo componentInfo)
        {
            // TODO: attributes, type parameters
            foreach (var item in componentInfo.longId)
            {
                if (_cache.TryGetValue(item.idRange, out var use) && use.Symbol is FSharpEntity entity)
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
                        AddIdent(item, type);
                    }
                }
            }
        }

        private void Visit(Ast.SynTypeDefnSimpleRepr synTypeDefn)
        {
            // TODO:
            switch (synTypeDefn)
            {
                case Ast.SynTypeDefnSimpleRepr.Record record:
                    foreach (var item in record.recordFields)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynTypeDefnSimpleRepr.Union union:
                    foreach (var item in union.unionCases)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynTypeDefnSimpleRepr.Enum @enum:
                    foreach (var item in @enum.Item1)
                    {
                        AddIdent(item.ident, _enumFieldName);
                    }
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in simple type definition", synTypeDefn.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynUnionCase unionCase)
        {
            AddIdent(unionCase.ident, _unionType);
            Visit(unionCase.Item3);
        }

        private void Visit(Ast.SynUnionCaseType unionCaseType)
        {
            switch (unionCaseType)
            {
                case Ast.SynUnionCaseType.UnionCaseFields unionCaseFields:
                    foreach (var item in unionCaseFields.cases)
                    {
                        var oldContext = _context;
                        _context = _context.WithParent(unionCaseFields);
                        Visit(item);
                        _context = oldContext;
                    }
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in union case type", unionCaseType.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynField field)
        {
            // NOTE: doesn't classify argument in union case declaration
            if (field.Item3.IsSome() && !(_context.Parent is Ast.SynUnionCaseType.UnionCaseFields))
            {
                AddIdent(field.Item3.Value, _fieldType);
            }
            Visit(field.Item4);
        }

        private void Visit(Ast.SynValSig valSig)
        {
            if (valSig.synExpr.IsSome())
            {
                Visit(valSig.synExpr.Value);
            }

            if (_cache.TryGetValue(valSig.ident.idRange, out var use))
            {
                if (_context.Parent is Ast.SynMemberDefn)
                {
                    switch (use.Symbol)
                    {
                        case FSharpMemberOrFunctionOrValue some:
                            if (some.IsMember && (some.IsProperty || some.IsPropertyGetterMethod || some.IsPropertySetterMethod))
                            {
                                AddIdent(valSig.ident, _propertyType);
                            }
                            break;

                        default:
                            Log.Debug("Symbol type {0} doesn't support in valsig", use.Symbol.GetType());
                            break;
                    }
                }
            }
        }

        private void Visit(Ast.SynBinding binding)
        {
            Visit(binding.headPat);
            Visit(binding.expr);
        }

        private void Visit(Ast.SynPat pattern)
        {
            // TODO:
            switch (pattern)
            {
                case Ast.SynPat.Named nameSyntax:
                    Visit(nameSyntax.Item1);
                    if (_cache.TryGetValue(nameSyntax.Item2.idRange, out var symbolUse))
                    {
                        switch (symbolUse.Symbol)
                        {
                            case FSharpMemberOrFunctionOrValue some:
                                if (IsParameterByDefn(some))
                                {
                                    _context = _context.WithParams(_context.Params.Add(some.LogicalName));
                                    AddIdent(nameSyntax.Item2, _parameterName);
                                }
                                else if (_context.Params.Contains(some.LogicalName))
                                {
                                    _context = _context.WithParams(_context.Params.Remove(some.LogicalName));
                                    AddIdent(nameSyntax.Item2, _localBindingValueName);
                                }
                                break;

                            default:
                                Log.Debug("Symbol type {0} doesn't support in pattern", symbolUse.Symbol.GetType());
                                break;
                        }
                    }
                    break;

                case Ast.SynPat.LongIdent longIndent:
                    // TODO: what's about another items?
                    switch (longIndent.Item4)
                    {
                        case Ast.SynConstructorArgs.Pats pats:
                            foreach (var item in pats.Item)
                            {
                                var oldContext = _context;
                                _context = _context.WithParent(pats);
                                Visit(item);
                                _context = oldContext.WithParams(_context.Params);
                            }
                            break;

                        default:
                            Log.Debug("Ast type {0} doesn't support in constructor args", longIndent.Item4.GetType());
                            break;
                    }
                    Visit(longIndent.longDotId);
                    break;

                case Ast.SynPat.Ands ands:
                    foreach (var item in ands.Item1)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynPat.IsInst isInst:
                    Visit(isInst);
                    break;

                case Ast.SynPat.Or or:
                    Visit(or.Item1);
                    Visit(or.Item2);
                    break;

                case Ast.SynPat.Paren paren:
                    Visit(paren.Item1);
                    break;

                case Ast.SynPat.QuoteExpr quoteExpr:
                    Visit(quoteExpr.Item1);
                    break;

                case Ast.SynPat.Tuple tuple:
                    foreach (var item in tuple.Item2)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynPat.Typed typed:
                    Visit(typed.Item1);
                    Visit(typed.Item2);
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in pattern", pattern.GetType());
                    break;
            }
        }

        private void Visit(Ast.LongIdentWithDots identWithDots)
        {
            if (identWithDots.Lid.IsEmpty) return;

            var idents = identWithDots.Lid;
            var first = identWithDots.Lid.Head;
            if (_cache.TryGetValue(first.idRange, out var use))
            {
                if (use.Symbol is FSharpMemberOrFunctionOrValue some && some.IsMemberThisValue)
                {
                    AddIdent(first, _selfIdentifierName);
                    idents = idents.Tail;
                }
            }

            // TODO: if the one id from idents was, for example, a field, does it mean that the all id from idents would be a field?
            foreach (var item in idents)
            {
                if (_cache.TryGetValue(item.idRange, out use) && use.Symbol is FSharpMemberOrFunctionOrValue some)
                {
                    // TODO:
                    var classification =
                        some.IsProperty || some.IsPropertyGetterMethod || some.IsPropertySetterMethod ? _propertyType :
                        some.IsInstanceMember && some.FullType.IsFunctionType ? _methodName :
                        some.IsMember && some.FullType.IsFunctionType ? _staticMethodName :
                        some.IsModuleValueOrMember && some.FullType.IsFunctionType ? _moduleFunctionName :
                        null;

                    if (classification.IsNotNull())
                    {
                        AddIdent(item, classification);
                    }
                }
            }
        }

        private void Visit(Ast.SynExpr expression)
        {
            // TODO:
            switch (expression)
            {
                case Ast.SynExpr.Ident ident:
                    if (_cache.TryGetValue(ident.Item.idRange, out var use))
                    {
                        if (use.Symbol is FSharpMemberOrFunctionOrValue some && IsParameterByUse(some))
                        {
                            AddIdent(ident.Item, _parameterName);
                        }
                    }
                    break;

                case Ast.SynExpr.LetOrUse letOrUse:
                    foreach (var item in letOrUse.bindings)
                    {
                        Visit(item);
                    }
                    Visit(letOrUse.body);
                    break;

                case Ast.SynExpr.Typed typed:
                    Visit(typed.typeName);
                    Visit(typed.expr);
                    break;

                case Ast.SynExpr.Lambda lambda:
                    Visit(lambda.args);
                    Visit(lambda.body);
                    break;

                case Ast.SynExpr.App app:
                    Visit(app.argExpr);
                    Visit(app.funcExpr);
                    break;

                default:
                    Log.Debug("Ast type {0} doesn't support in expression", expression.GetType());
                    break;
            }
        }

        // TODO:
        private bool IsParameterByUse(FSharpMemberOrFunctionOrValue some) =>
            some.IsValue && _context.Params.Contains(some.LogicalName);

        // TODO:
        private bool IsParameterByDefn(FSharpMemberOrFunctionOrValue some) =>
            some.IsValue && _context.Parent is Ast.SynConstructorArgs;

        private void AddIdents<T>(T longIds, IClassificationType classificationType) where T : IEnumerable<Ast.Ident>
        {
            var snapshot = _snapshotSpan.Snapshot;
            foreach (var item in longIds)
            {
                var span = item.idRange.ToSpan(snapshot);
                _result.Add(new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), classificationType));
            }
        }

        private void AddIdent(Ast.Ident ident, IClassificationType classificationType)
        {
            var snapshot = _snapshotSpan.Snapshot;
            var span = ident.idRange.ToSpan(snapshot);
            _result.Add(new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), classificationType));
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
            InitializeClassification(FSharpNames.ParameterName, ref _parameterName);
            InitializeClassification(FSharpNames.EnumName, ref _enumType);
            InitializeClassification(FSharpNames.EnumFieldName, ref _enumFieldName);
            InitializeClassification(FSharpNames.LocalBindingValueName, ref _localBindingValueName);
            InitializeClassification(FSharpNames.ModuleFunctionName, ref _moduleFunctionName);
            InitializeClassification(FSharpNames.MethodName, ref _methodName);
            InitializeClassification(FSharpNames.StaticMethodName, ref _staticMethodName);

            _classifications = builder.TryMoveToImmutable();
        }
    }
}