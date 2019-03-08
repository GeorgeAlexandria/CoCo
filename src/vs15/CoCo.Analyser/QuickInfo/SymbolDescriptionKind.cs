using System;

namespace CoCo.Analyser.QuickInfo
{
    /// <summary>
    /// Determines the kind of description in the quick info
    /// </summary>
    public enum SymbolDescriptionKind
    {
        None = 0,

        /// <summary>
        /// Main description e.g. signature
        /// </summary>
        Main = 1,
        Additional = 2,
        TypeParameter = 3,

        /// <summary>
        /// Captured variables for lambda, delegate and for local function
        /// </summary>
        Captures = 4,
        Exceptions = 5,
        AnonymousTypes = 6,
    }
}