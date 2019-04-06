namespace CoCo.Analyser.QuickInfo
{
    public enum ImageKind
    {
        None = 0,

        ClassPublic = 1,
        ClassInternal,
        ClassProtected,
        ClassPrivate,

        ConstPublic = 5,
        ConstInternal,
        ConstProtected,
        ConstPrivate,

        DelegatePublic = 9,
        DelegateInternal,
        DelegateProtected,
        DelegatePrivate,

        EnumPublic = 13,
        EnumInternal,
        EnumProtected,
        EnumPrivate,

        EnumMemberPublic = 17,
        EnumMemberInternal,
        EnumMemberProtected,
        EnumMemberPrivate,

        EventPublic = 21,
        EventInternal,
        EventProtected,
        EventPrivate,

        ExtensionMethodPublic = 25,
        ExtensionMethodInternal,
        ExtensionMethodProtected,
        ExtensionMethodPrivate,

        FieldPublic = 29,
        FieldInternal,
        FieldProtected,
        FieldPrivate,

        InterfacePublic = 33,
        InterfaceInternal,
        InterfaceProtected,
        InterfacePrivate,

        MethodPublic = 37,
        MethodInternal,
        MethodProtected,
        MethodPrivate,

        ModulePublic = 41,
        ModuleInternal,
        ModuleProtected,
        ModulePrivate,

        PropertyPublic = 45,
        PropertyInternal,
        PropertyProtected,
        PropertyPrivate,

        StructPublic = 49,
        StructInternal,
        StructProtected,
        StructPrivate,

        Label = 53,
        Local = 54,
        Namespace = 55,
        Parameter = 56,
        TypeParameter = 57,
        RangeVariable = 58,
        Keyword = 60,

        Error = 100,
    }
}