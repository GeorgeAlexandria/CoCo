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

        private class SymbolUseMap
        {
            private readonly Dictionary<Range.range, FSharpSymbolUse> _map;
            private readonly List<Range.range> _ranges;

            public SymbolUseMap(FSharpCheckFileResults checkResults)
            {
                SymbolsUse = FSharpAsync.RunSynchronously(checkResults.GetAllUsesOfAllSymbolsInFile(), null, null);

                var capacity = (SymbolsUse.Length >> 1) + 1;
                _map = new Dictionary<Range.range, FSharpSymbolUse>(capacity);
                _ranges = new List<Range.range>(capacity);
                foreach (var use in SymbolsUse)
                {
                    if (_map.TryAdd(use.RangeAlternate, use))
                    {
                        Insert(_ranges, use.RangeAlternate);
                        continue;
                    }
                    Log.Debug($"Item already exist in the cache by range {use.RangeAlternate}");
                }

                Log.Debug("Range of usages:");
                foreach (var item in _ranges)
                {
                    Log.Debug(item.ToShortString());
                }
            }

            public FSharpSymbolUse[] SymbolsUse { get; }

            private void Insert(List<Range.range> list, Range.range range)
            {
                // NOTE: try to keep sorting of items and move to the firstly positions an item that contains another items

                if (list.Count > 0)
                {
                    var last = list[list.Count - 1];
                    if (last.StartLine < range.StartLine || last.StartLine == range.StartLine && last.StartColumn < range.StartColumn)
                    {
                        list.Add(range);
                        return;
                    }
                }

                var left = 0;
                var right = list.Count - 1;
                while (left <= right)
                {
                    var position = left + ((right - left) >> 1);
                    var item = list[position];
                    if (item.Equals(range))
                    {
                        list.Insert(position + 1, range);
                        return;
                    }

                    if (item.StartLine < range.StartLine || item.StartLine == range.StartLine && item.StartColumn < range.StartColumn)
                    {
                        left = position + 1;
                        continue;
                    }
                    right = position - 1;
                }

                list.Insert(left, range);
            }

            public bool TryGetValue(Range.range range, out FSharpSymbolUse use)
            {
                if (_map.TryGetValue(range, out use))
                {
                    return true;
                }

                // NOTE: a couple of entities like property or field access have range that is match a range of receiver and identifier together,
                // So try to find the minimal range of symboluse that contains input range, to handle such of cases
                Log.Debug("Try to find minimal containing range for {0}", range.ToShortString());

                // NOTE: firstly, find the first range which start point is more then start point of range
                var left = 0;
                var right = _ranges.Count - 1;
                while (left <= right)
                {
                    var position = left + ((right - left) >> 1);
                    var item = _ranges[position];

                    if (item.StartLine < range.StartLine || item.StartLine == range.StartLine && item.StartColumn < range.StartColumn)
                    {
                        left = position + 1;
                        continue;
                    }
                    right = position - 1;
                }

                // NOTE: secondly, find the minimal range that contains input range and is above of range from step 1
                var start = left < _ranges.Count - 1 ? left : _ranges.Count - 1;
                for (int i = start; i >= 0; --i)
                {
                    var item = _ranges[i];
                    if (item.StartLine <= range.StartLine && item.StartColumn <= range.StartColumn &&
                        item.EndLine >= range.EndLine && item.EndColumn >= range.EndColumn)
                    {
                        Log.Debug("Found range {0}", item.ToShortString());
                        use = _map[item];
                        return true;
                    }
                }

                use = default;
                return false;
            }
        }

        private IClassificationType _namespaceType;
        private IClassificationType _classType;
        private IClassificationType _recordType;
        private IClassificationType _unionType;
        private IClassificationType _moduleType;
        private IClassificationType _structureType;
        private IClassificationType _propertyType;
        private IClassificationType _fieldType;
        private IClassificationType _selfIdentifierType;
        private IClassificationType _parameterType;
        private IClassificationType _enumType;
        private IClassificationType _enumFieldType;
        private IClassificationType _localBindingType;
        private IClassificationType _moduleFunctionType;
        private IClassificationType _methodType;
        private IClassificationType _staticMethodType;
        private IClassificationType _extensionMethodType;
        private IClassificationType _moduleBindingType;
        private IClassificationType _interfaceType;
        private IClassificationType _delegateType;
        private IClassificationType _typeParameterType;

        private static FSharpClassifierService _instance;
        private readonly ClassificationOptions _classificationOptions = new ClassificationOptions();
        private ImmutableArray<IClassificationType> _classifications;

        private Context _context;

        private List<ClassificationSpan> _result;
        private SnapshotSpan _snapshotSpan;
        private SymbolUseMap _cache;

        private FSharpClassifierService(IReadOnlyDictionary<string, ClassificationInfo> classifications)
        {
            InitializeClassifications(classifications);
        }

        internal static FSharpClassifierService GetClassifier(IReadOnlyDictionary<string, ClassificationInfo> classifications) =>
            _instance ?? (_instance = new FSharpClassifierService(classifications));

        public List<ClassificationSpan> GetClassificationSpans(
            FSharpParseFileResults parseResults, FSharpCheckFileResults checkResults, SnapshotSpan span)
        {
            Log.Debug("Classify file {0}...", parseResults.FileName);

            var cache = new SymbolUseMap(checkResults);

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
                    Visit(openSyntax.longDotId);
                    break;

                case Ast.SynModuleDecl.Types typeSyntax:
                    foreach (var typeDefinition in typeSyntax.Item1)
                    {
                        Visit(typeDefinition.Item1);
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

                case Ast.SynModuleDecl.Exception exception:
                    Visit(exception.Item1);
                    break;

                case Ast.SynModuleDecl.Attributes attributes:
                    foreach (var item in attributes.Item1)
                    {
                        Visit(item);
                    }
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in module declaration", moduleDecl.GetType());
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

                case Ast.SynMemberDefn.LetBindings letBindings:
                    foreach (var item in letBindings.Item1)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynMemberDefn.Inherit inherit:
                    // TODO: whats' about AddIdents(inherit.Item2, );
                    Visit(inherit.Item1);
                    break;

                case Ast.SynMemberDefn.ImplicitInherit inherit:
                    // TODO: whats' about AddIdents(inherit.inheritAlias, );
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

                case Ast.SynMemberDefn.Interface @interface:
                    Visit(@interface.Item1);
                    if (@interface.Item2.IsSome())
                    {
                        foreach (var item in @interface.Item2.Value)
                        {
                            Visit(item);
                        }
                    }
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in member definition", memberDefn.GetType());
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
                    Log.Debug("Ast type {0} wasn't handled in simple pats", simplePats.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynSimplePat simplePat)
        {
            switch (simplePat)
            {
                case Ast.SynSimplePat.Id id:
                    _context = _context.WithParams(_context.Params.Add(id.ident.idText));
                    AddIdent(id.ident, _parameterType);
                    break;

                case Ast.SynSimplePat.Attrib attribute:
                    Visit(attribute.Item1);
                    foreach (var item in attribute.Item2)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynSimplePat.Typed typed:
                    Visit(typed.Item1);
                    Visit(typed.Item2);
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in simple pat", simplePat.GetType());
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
                        if (item is Ast.SynMemberDefn.LetBindings)
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
                    Log.Debug("Ast type {0} wasn't handled in type definition", typeDefnRepr.GetType());
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
                    Visit(ident.longDotId);
                    break;

                case Ast.SynType.Tuple tuple:
                    foreach (var (_, typeName) in tuple.typeNames)
                    {
                        Visit(typeName);
                    }
                    break;

                case Ast.SynType.Var var:
                    Visit(var.genericName);
                    break;

                case Ast.SynType.App app:
                    Visit(app.typeName);
                    foreach (var item in app.typeArgs)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynType.Anon _:
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in type", type.GetType());
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
            foreach (var item in componentInfo.longId)
            {
                // TODO: if the one id from longId was, for example, a class, does it mean that the all id from longId would be a class?
                if (_cache.TryGetValue(item.idRange, out var use) && use.Symbol is FSharpEntity entity &&
                    TryClassifyType(entity, out var type))
                {
                    AddIdent(item, type);
                }
            }

            foreach (var item in componentInfo.typeParams)
            {
                Visit(item);
            }

            foreach (var item in componentInfo.constraints)
            {
                Visit(item);
            }

            foreach (var item in componentInfo.attribs)
            {
                Visit(item);
            }
        }

        private void Visit(Ast.SynExceptionDefn exceptionDefn)
        {
            Visit(exceptionDefn.Item1);
            foreach (var item in exceptionDefn.Item2)
            {
                Visit(item);
            }
        }

        private void Visit(Ast.SynTypeDefnSimpleRepr synTypeDefn)
        {
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
                        AddIdent(item.ident, _enumFieldType);
                    }
                    break;

                case Ast.SynTypeDefnSimpleRepr.Exception exception:
                    Visit(exception.Item);
                    break;

                case Ast.SynTypeDefnSimpleRepr.TypeAbbrev abbrev:
                    Visit(abbrev.Item2);
                    break;

                case Ast.SynTypeDefnSimpleRepr.General general:
                    foreach (var (type, _, ident) in general.Item2)
                    {
                        Visit(type);
                        // TODO: what is type ident?
                    }
                    foreach (var (sig, _) in general.Item3)
                    {
                        Visit(sig);
                    }
                    foreach (var item in general.Item4)
                    {
                        Visit(item);
                    }
                    if (general.Item7.IsSome())
                    {
                        Visit(general.Item7.Value);
                    }
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in simple type definition", synTypeDefn.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynExceptionDefnRepr defn)
        {
            foreach (var item in defn.Item1)
            {
                Visit(item);
            }
            if (defn.longId.IsSome())
            {
                AddIdents(defn.longId.Value, _classType);
            }
            Visit(defn.Item2);
        }

        private void Visit(Ast.SynUnionCase unionCase)
        {
            var type = _unionType;
            if (_cache.TryGetValue(unionCase.ident.idRange, out var use) && use.Symbol is FSharpEntity entity &&
                entity.IsFSharpExceptionDeclaration)
            {
                type = _classType;
            }

            AddIdent(unionCase.ident, type);
            Visit(unionCase.Item3);
        }

        private void Visit(Ast.SynUnionCaseType unionCaseType)
        {
            switch (unionCaseType)
            {
                case Ast.SynUnionCaseType.UnionCaseFields unionCaseFields:
                    foreach (var item in unionCaseFields.cases)
                    {
                        Visit(item);
                    }
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in union case type", unionCaseType.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynField field)
        {
            // NOTE: doesn't classify argument in union case declaration
            if (field.Item3.IsSome())
            {
                AddIdent(field.Item3.Value, _fieldType);
            }
            Visit(field.Item4);
        }

        private void Visit(Ast.SynValSig valSig)
        {
            foreach (var item in valSig.synAttributes)
            {
                Visit(item);
            }

            Visit(valSig.arity);

            if (_cache.TryGetValue(valSig.ident.idRange, out var use))
            {
                IClassificationType classification = null;
                if (_context.Parent is Ast.SynMemberDefn)
                {
                    classification = use.Symbol switch
                    {
                        FSharpMemberOrFunctionOrValue some =>
                            some.IsProperty || some.IsPropertyGetterMethod || some.IsPropertySetterMethod ? _propertyType :
                            some.IsInstanceMember && some.FullType.IsFunctionType ? _methodType :
                            null,
                        _ => null
                    };
                }
                // NOTE: handle member sig by type constraints
                else if (_context.Parent is Ast.SynMemberSig.Member memberSig)
                {
                    classification = use.Symbol switch
                    {
                        FSharpMemberOrFunctionOrValue some =>
                            some.IsProperty || some.IsPropertyGetterMethod || some.IsPropertySetterMethod ? _propertyType :
                            some.IsInstanceMember && some.FullType.IsFunctionType ? _methodType :
                            null,

                        FSharpParameter _ => valSig.SynType is Ast.SynType.Fun
                            ? memberSig.Item2.IsInstance
                                ? _methodType
                                : _staticMethodType
                            : _propertyType,

                        _ => null,
                    };
                }

                if (classification.IsNotNull())
                {
                    AddIdent(valSig.ident, classification);
                }
                else
                {
                    Log.Debug("Symbol type {0} wasn't handled in valsig", use.Symbol.GetType());
                }
            }

            if (valSig.synExpr.IsSome())
            {
                Visit(valSig.synExpr.Value);
            }
        }

        private void Visit(Ast.SynValInfo valInfo)
        {
            foreach (var list in valInfo.Item1)
            {
                foreach (var item in list)
                {
                    Visit(item);
                }
            }
        }

        private void Visit(Ast.SynArgInfo argInfo)
        {
            foreach (var item in argInfo.Item1)
            {
                Visit(item);
            }

            if (argInfo.Item3.IsSome())
            {
                _context = _context.WithParams(_context.Params.Add(argInfo.Item3.Value.idText));
                AddIdent(argInfo.Item3.Value, _parameterType);
            }
        }

        private void Visit(Ast.SynBinding binding)
        {
            foreach (var item in binding.attrs)
            {
                Visit(item);
            }

            var oldContext = _context;
            _context = _context.WithParent(binding);
            Visit(binding.headPat);
            _context = oldContext.WithParams(_context.Params);

            Visit(binding.expr);
            // NOTE: reset context by per function to not pass collected parameters from an one function to another
            _context = oldContext;
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
                                    AddIdent(nameSyntax.Item2, _parameterType);
                                }
                                else if (_context.Params.Contains(some.LogicalName))
                                {
                                    _context = _context.WithParams(_context.Params.Remove(some.LogicalName));
                                    AddIdent(nameSyntax.Item2, _localBindingType);
                                }
                                else
                                {
                                    // TODO: another types
                                    var classification =
                                        some.IsModuleValueOrMember && some.FullType.IsFunctionType ? _moduleFunctionType :
                                        some.IsModuleValueOrMember ? _moduleBindingType :
                                        some.FullType.IsFunctionType ? _localBindingType :
                                        some.IsValue ? _localBindingType :
                                        null;

                                    if (classification.IsNotNull())
                                    {
                                        AddIdent(nameSyntax.Item2, classification);
                                        break;
                                    }
                                    Log.Debug("Symbol type {0} wasn't handled in pattern", symbolUse.Symbol.GetType());
                                }
                                break;

                            default:
                                Log.Debug("Symbol type {0} wasn't handled in pattern", symbolUse.Symbol.GetType());
                                break;
                        }
                    }
                    break;

                case Ast.SynPat.LongIdent longIndent:
                    // TODO: what's about another items?
                    var isParentSynBinding = _context.Parent is Ast.SynBinding;
                    switch (longIndent.Item4)
                    {
                        case Ast.SynConstructorArgs.Pats pats:
                            foreach (var item in pats.Item)
                            {
                                if (!isParentSynBinding)
                                {
                                    Visit(item);
                                    continue;
                                }

                                var oldContext = _context;
                                _context = _context.WithParent(pats);
                                Visit(item);
                                _context = oldContext.WithParams(_context.Params);
                            }
                            break;

                        case Ast.SynConstructorArgs.NamePatPairs namePats:
                            // TODO: do namePats can be a parameter declarations, not accesses?
                            foreach (var (ident, pat) in namePats.Item1)
                            {
                                AddIdent(ident, _parameterType);
                                if (!isParentSynBinding)
                                {
                                    Visit(pat);
                                    continue;
                                }

                                var oldContext = _context;
                                _context = _context.WithParent(namePats);
                                Visit(pat);
                                _context = oldContext.WithParams(_context.Params);
                            }
                            break;

                        default:
                            Log.Debug("Ast type {0} wasn't handled in constructor args", longIndent.Item4.GetType());
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
                    Visit(isInst.Item1);
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

                case Ast.SynPat.ArrayOrList arrayOrList:
                    foreach (var item in arrayOrList.Item2)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynPat.OptionalVal optional:
                    _context = _context.WithParams(_context.Params.Add(optional.Item1.idText));
                    AddIdent(optional.Item1, _parameterType);
                    break;

                case Ast.SynPat.Attrib attrib:
                    Visit(attrib.Item1);
                    foreach (var item in attrib.Item2)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynPat.Record record:
                    foreach (var item in record.Item1)
                    {
                        var some = item.Item1;
                        AddIdent(some.Item2, _fieldType);
                        Visit(item.Item2);
                    }
                    break;

                case Ast.SynPat.Wild _:
                case Ast.SynPat.Const _:
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in pattern", pattern.GetType());
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
                    AddIdent(first, _selfIdentifierType);
                    idents = idents.Tail;
                }
            }

            foreach (var item in idents)
            {
                if (!_cache.TryGetValue(item.idRange, out use))
                {
                    Log.Debug("Ident {0} doesn't have any symbol", item);
                    continue;
                }

                if (use.Symbol is FSharpMemberOrFunctionOrValue some)
                {
                    if (TryClassifySome(some, out var classification))
                    {
                        AddIdent(item, classification);
                        continue;
                    }
                }
                if (use.Symbol is FSharpEntity entity)
                {
                    if (TryClassifyType(entity, out var type))
                    {
                        AddIdent(item, type);
                        continue;
                    }
                }
                if (use.Symbol is FSharpField field)
                {
                    var type = field.DeclaringEntity.IsSome() && field.DeclaringEntity.Value.IsEnum
                        ? _enumFieldType
                        : _fieldType;

                    AddIdent(item, type);
                    continue;
                }
                if (use.Symbol is FSharpUnionCase)
                {
                    // TODO: what's about fields?
                    AddIdent(item, _unionType);
                    continue;
                }

                Log.Debug("Symbol type {0} wasn't handled in long ident with dots", use.Symbol.GetType());
            }
        }

        private void Visit(Ast.SynTyparDecl typarDecl)
        {
            Visit(typarDecl.Item2);
            foreach (var item in typarDecl.attributes)
            {
                Visit(item);
            }
        }

        private void Visit(Ast.SynAttributeList attributeList)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                Visit(attribute.TypeName);
                Visit(attribute.ArgExpr);
            }
        }

        private void Visit(Ast.SynTypeConstraint constraint)
        {
            switch (constraint)
            {
                case Ast.SynTypeConstraint.WhereTyparDefaultsToType defalut:
                    Visit(defalut.typeName);
                    Visit(defalut.genericName);
                    break;

                case Ast.SynTypeConstraint.WhereTyparIsComparable comparable:
                    Visit(comparable.genericName);
                    break;

                case Ast.SynTypeConstraint.WhereTyparIsDelegate @delegate:
                    Visit(@delegate.genericName);
                    foreach (var item in @delegate.Item2)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynTypeConstraint.WhereTyparIsEnum @enum:
                    Visit(@enum.genericName);
                    foreach (var item in @enum.Item2)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynTypeConstraint.WhereTyparIsEquatable equatable:
                    Visit(equatable.genericName);
                    break;

                case Ast.SynTypeConstraint.WhereTyparIsReferenceType reference:
                    Visit(reference.genericName);
                    break;

                case Ast.SynTypeConstraint.WhereTyparIsUnmanaged unmanaged:
                    Visit(unmanaged.genericName);
                    break;

                case Ast.SynTypeConstraint.WhereTyparIsValueType value:
                    Visit(value.genericName);
                    break;

                case Ast.SynTypeConstraint.WhereTyparSubtypeOfType subType:
                    Visit(subType.typeName);
                    Visit(subType.genericName);
                    break;

                case Ast.SynTypeConstraint.WhereTyparSupportsMember member:
                    foreach (var item in member.genericNames)
                    {
                        Visit(item);
                    }

                    var oldContext = _context;
                    _context = _context.WithParent(constraint);
                    Visit(member.memberSig);
                    _context = oldContext;
                    break;

                case Ast.SynTypeConstraint.WhereTyparSupportsNull @null:
                    Visit(@null.genericName);
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in type constraint", constraint.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynExpr expression)
        {
            switch (expression)
            {
                case Ast.SynExpr.Ident ident:
                    if (_cache.TryGetValue(ident.Item.idRange, out var use))
                    {
                        if (use.Symbol is FSharpMemberOrFunctionOrValue some && TryClassifySome(some, out var classification))
                        {
                            AddIdent(ident.Item, classification);
                            return;
                        }
                        if (use.Symbol is FSharpParameter)
                        {
                            AddIdent(ident.Item, _parameterType);
                            return;
                        }
                        if (use.Symbol is FSharpUnionCase)
                        {
                            // TODO: what's about fields?
                            AddIdent(ident.Item, _unionType);
                            return;
                        }
                        if (use.Symbol is FSharpEntity entity && TryClassifyType(entity, out var type))
                        {
                            AddIdent(ident.Item, type);
                            return;
                        }
                        Log.Debug("Symbol type {0} wasn't handled in ident expression", use.Symbol.GetType());
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

                case Ast.SynExpr.DotSet dotSet:
                    Visit(dotSet.Item1);
                    Visit(dotSet.longDotId);
                    Visit(dotSet.Item3);
                    break;

                case Ast.SynExpr.DotIndexedGet indexedGet:
                    Visit(indexedGet.Item1);
                    foreach (var item in indexedGet.Item2)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynExpr.DotIndexedSet indexedSet:
                    Visit(indexedSet.objectExpr);
                    foreach (var item in indexedSet.indexExprs)
                    {
                        Visit(item);
                    }
                    Visit(indexedSet.valueExpr);
                    break;

                case Ast.SynExpr.Set set:
                    Visit(set.Item1);
                    Visit(set.Item2);
                    break;

                case Ast.SynExpr.NamedIndexedPropertySet namedSet:
                    Visit(namedSet.longDotId);
                    Visit(namedSet.Item2);
                    Visit(namedSet.Item3);
                    break;

                case Ast.SynExpr.DotNamedIndexedPropertySet dotNamedSet:
                    Visit(dotNamedSet.longDotId);
                    Visit(dotNamedSet.Item1);
                    Visit(dotNamedSet.Item3);
                    Visit(dotNamedSet.Item4);
                    break;

                case Ast.SynExpr.TypeTest typeTest:
                    Visit(typeTest.expr);
                    Visit(typeTest.typeName);
                    break;

                case Ast.SynExpr.Upcast upcast:
                    Visit(upcast.expr);
                    Visit(upcast.typeName);
                    break;

                case Ast.SynExpr.Downcast downcast:
                    Visit(downcast.expr);
                    Visit(downcast.typeName);
                    break;

                case Ast.SynExpr.InferredUpcast inferredUpcast:
                    Visit(inferredUpcast.expr);
                    break;

                case Ast.SynExpr.InferredDowncast inferredDowncast:
                    Visit(inferredDowncast.expr);
                    break;

                case Ast.SynExpr.AddressOf addresOf:
                    Visit(addresOf.Item2);
                    break;

                case Ast.SynExpr.JoinIn joinIn:
                    Visit(joinIn.Item1);
                    Visit(joinIn.Item3);
                    break;

                case Ast.SynExpr.YieldOrReturn yieldOrReturn:
                    Visit(yieldOrReturn.expr);
                    break;

                case Ast.SynExpr.YieldOrReturnFrom yieldOrReturnFrom:
                    Visit(yieldOrReturnFrom.expr);
                    break;

                case Ast.SynExpr.LetOrUseBang letBang:
                    Visit(letBang.Item4);
                    Visit(letBang.Item5);
                    Visit(letBang.Item6);
                    break;

                case Ast.SynExpr.MatchBang matchBang:
                    Visit(matchBang.expr);
                    foreach (var item in matchBang.clauses)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynExpr.DoBang doBang:
                    Visit(doBang.expr);
                    break;

                case Ast.SynExpr.DotGet dotGet:
                    Visit(dotGet.expr);
                    Visit(dotGet.longDotId);
                    break;

                case Ast.SynExpr.TraitCall traitCall:
                    foreach (var item in traitCall.Item1)
                    {
                        Visit(item);
                    }
                    Visit(traitCall.Item2);
                    Visit(traitCall.Item3);
                    break;

                case Ast.SynExpr.LongIdentSet identSet:
                    Visit(identSet.longDotId);
                    Visit(identSet.expr);
                    break;

                case Ast.SynExpr.Paren paren:
                    Visit(paren.expr);
                    break;

                case Ast.SynExpr.Quote quote:
                    Visit(quote.@operator);
                    Visit(quote.quotedSynExpr);
                    break;

                case Ast.SynExpr.Tuple tuple:
                    foreach (var item in tuple.exprs)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynExpr.AnonRecd anonRecord:
                    // TODO: what is copy info?
                    foreach (var (field, expr) in anonRecord.recordFields)
                    {
                        AddIdent(field, _fieldType);
                        Visit(expr);
                    }
                    break;

                case Ast.SynExpr.ArrayOrList arrayOrList:
                    foreach (var item in arrayOrList.exprs)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynExpr.Record record:
                    foreach (var ((ident, _), expr, _) in record.recordFields)
                    {
                        // TODO: classify them as fields
                        Visit(ident);
                        if (expr.IsSome())
                        {
                            Visit(expr.Value);
                        }
                    }
                    break;

                case Ast.SynExpr.New @new:
                    Visit(@new.typeName);
                    Visit(@new.expr);
                    break;

                case Ast.SynExpr.ObjExpr objExpr:
                    Visit(objExpr.objType);
                    if (objExpr.argOptions.IsSome())
                    {
                        // TODO: what's about ident?
                        var (exp, _) = objExpr.argOptions.Value;
                        Visit(exp);
                    }
                    foreach (var item in objExpr.bindings)
                    {
                        Visit(item);
                    }
                    foreach (var item in objExpr.extraImpls)
                    {
                        Visit(item.Item1);
                        foreach (var binding in item.Item2)
                        {
                            Visit(binding);
                        }
                    }
                    break;

                case Ast.SynExpr.While @while:
                    Visit(@while.whileExpr);
                    Visit(@while.doExpr);
                    break;

                case Ast.SynExpr.For @for:
                    AddIdent(@for.ident, _localBindingType);
                    Visit(@for.identBody);
                    Visit(@for.toBody);
                    Visit(@for.doBody);
                    break;

                case Ast.SynExpr.ForEach @foreach:
                    Visit(@foreach.pat);
                    Visit(@foreach.enumExpr);
                    Visit(@foreach.bodyExpr);
                    break;

                case Ast.SynExpr.LongIdent longIdent:
                    // TODO: altNameRefCell?
                    Visit(longIdent.longDotId);
                    break;

                case Ast.SynExpr.ArrayOrListOfSeqExpr some:
                    Visit(some.expr);
                    break;

                case Ast.SynExpr.MatchLambda lambda:
                    foreach (var item in lambda.Item3)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynExpr.Match match:
                    Visit(match.expr);
                    foreach (var item in match.clauses)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynExpr.Do @do:
                    Visit(@do.expr);
                    break;

                case Ast.SynExpr.Assert assert:
                    Visit(assert.expr);
                    break;

                case Ast.SynExpr.TryWith tryWith:
                    Visit(tryWith.tryExpr);
                    foreach (var item in tryWith.withCases)
                    {
                        Visit(item);
                    }
                    break;

                case Ast.SynExpr.TryFinally tryFinally:
                    Visit(tryFinally.tryExpr);
                    Visit(tryFinally.finallyExpr);
                    break;

                case Ast.SynExpr.Lazy lazy:
                    Visit(lazy.Item1);
                    break;

                case Ast.SynExpr.Sequential sequential:
                    Visit(sequential.expr1);
                    Visit(sequential.expr2);
                    break;

                case Ast.SynExpr.IfThenElse ifThenElse:
                    Visit(ifThenElse.ifExpr);
                    Visit(ifThenElse.thenExpr);
                    if (ifThenElse.elseExpr.IsSome())
                    {
                        Visit(ifThenElse.elseExpr.Value);
                    }
                    break;

                case Ast.SynExpr.CompExpr comp:
                    Visit(comp.expr);
                    break;

                case Ast.SynExpr.Fixed @fixed:
                    Visit(@fixed);
                    break;

                case Ast.SynExpr.Const _:
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in expression", expression.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynTypar synTypar)
        {
            if (_cache.TryGetValue(synTypar.ident.idRange, out var use))
            {
                if (use.Symbol is FSharpEntity entity && TryClassifyType(entity, out var type))
                {
                    AddIdent(synTypar.ident, type);
                    return;
                }
                if (use.Symbol is FSharpGenericParameter)
                {
                    AddIdent(synTypar.ident, _typeParameterType);
                    return;
                }
            }
            Log.Debug("Ast type {0} wasn't handled in typar", synTypar.GetType());
        }

        private void Visit(Ast.SynMemberSig memberSig)
        {
            switch (memberSig)
            {
                case Ast.SynMemberSig.Inherit inherit:
                    Visit(inherit.typeName);
                    break;

                case Ast.SynMemberSig.Interface @interface:
                    Visit(@interface.typeName);
                    break;

                case Ast.SynMemberSig.Member member:
                    var oldContext = _context;
                    _context = _context.WithParent(member);
                    Visit(member.Item1);
                    _context = oldContext;
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in member sig", memberSig.GetType());
                    break;
            }
        }

        private void Visit(Ast.SynMatchClause clause)
        {
            Visit(clause.Item1);
            if (clause.whenExpr.IsSome())
            {
                Visit(clause.whenExpr.Value);
            }
            Visit(clause.Item3);
        }

        private void Visit(Ast.SynIndexerArg arg)
        {
            switch (arg)
            {
                case Ast.SynIndexerArg.One one:
                    foreach (var item in one.Exprs)
                    {
                        Visit(item);
                    }
                    Visit(one.Item);
                    break;

                case Ast.SynIndexerArg.Two two:
                    foreach (var item in two.Exprs)
                    {
                        Visit(item);
                    }
                    Visit(two.Item1);
                    Visit(two.Item2);
                    break;

                default:
                    Log.Debug("Ast type {0} wasn't handled in indexer arg", arg.GetType());
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

        private bool TryClassifyType(FSharpEntity entity, out IClassificationType type)
        {
            if (entity.IsFSharpAbbreviation)
            {
                // NOTE: make sure that abbreviation has type definition to avoid invalid operation exception
                var abbrev = entity.AbbreviatedType;
                if (abbrev.HasTypeDefinition)
                {
                    return TryClassifyType(entity.AbbreviatedType.TypeDefinition, out type);
                }

                type =
                    abbrev.IsAnonRecordType ? _recordType :
                    // Does it delegate type?
                    abbrev.IsFunctionType ? _classType :
                    abbrev.IsStructTupleType ? _structureType :
                    null;

                return type.IsNotNull();
            }

            type =
                entity.IsNamespace ? _namespaceType :
                entity.IsFSharpModule ? _moduleType :
                entity.IsFSharpRecord ? _recordType :
                entity.IsFSharpUnion ? _unionType :
                entity.IsInterface ? _interfaceType :
                entity.IsDelegate ? _delegateType :
                entity.IsEnum ? _enumType :
                entity.IsValueType ? _structureType :
                entity.IsClass ? _classType :
                entity.IsFSharpExceptionDeclaration ? _classType :
                entity.IsOpaque ? _classType :
                entity.IsArrayType ? _classType :
                null;

            return type.IsNotNull();
        }

        private bool TryClassifySome(FSharpMemberOrFunctionOrValue some, out IClassificationType classification)
        {
            if (some.IsConstructor)
            {
                return TryClassifyType(some.ApparentEnclosingEntity, out classification);
            }

            // TODO: fields?
            classification =
                IsParameterByUse(some) ? _parameterType :
                some.IsProperty || some.IsPropertyGetterMethod || some.IsPropertySetterMethod ? _propertyType :
                some.IsExtensionMember && some.FullType.IsFunctionType ? _extensionMethodType :
                IsExtension(some) && some.FullType.IsFunctionType ? _extensionMethodType :
                some.IsInstanceMember && some.FullType.IsFunctionType ? _methodType :
                some.IsMember && some.FullType.IsFunctionType ? _staticMethodType :
                some.IsModuleValueOrMember && some.FullType.IsFunctionType ? _moduleFunctionType :
                some.FullType.IsFunctionType ? _localBindingType :
                some.IsValue && IsParameterByUse(some) ? _parameterType :
                some.IsValue ? _localBindingType :
                null;

            return classification.IsNotNull();
        }

        private bool IsExtension(FSharpMemberOrFunctionOrValue some)
        {
            bool HasExtensionAttribute(IEnumerable<FSharpAttribute> attributes)
            {
                foreach (var item in attributes)
                {
                    if (item.AttributeType.FullName.EqualsNoCase("System.Runtime.CompilerServices.ExtensionAttribute")) return true;
                }
                return false;
            }

            return HasExtensionAttribute(some.Attributes) && some.DeclaringEntity.IsSome() &&
                HasExtensionAttribute(some.DeclaringEntity.Value.Attributes);
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
            InitializeClassification(FSharpNames.SelfIdentifierName, ref _selfIdentifierType);
            InitializeClassification(FSharpNames.ParameterName, ref _parameterType);
            InitializeClassification(FSharpNames.EnumName, ref _enumType);
            InitializeClassification(FSharpNames.EnumFieldName, ref _enumFieldType);
            InitializeClassification(FSharpNames.LocalBindingValueName, ref _localBindingType);
            InitializeClassification(FSharpNames.ModuleFunctionName, ref _moduleFunctionType);
            InitializeClassification(FSharpNames.MethodName, ref _methodType);
            InitializeClassification(FSharpNames.StaticMethodName, ref _staticMethodType);
            InitializeClassification(FSharpNames.ExtensionMethodName, ref _extensionMethodType);
            InitializeClassification(FSharpNames.ModuleBindingValueName, ref _moduleBindingType);
            InitializeClassification(FSharpNames.InterfaceName, ref _interfaceType);
            InitializeClassification(FSharpNames.DelegateName, ref _delegateType);
            InitializeClassification(FSharpNames.TypeParameterName, ref _typeParameterType);

            _classifications = builder.TryMoveToImmutable();
        }
    }
}