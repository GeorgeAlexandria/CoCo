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
        Main = 1 << 0,
        Additional = 1 << 1,
        TypeParameter = 1 << 2,

        /// <summary>
        /// Captured variables for lambda, delegate and for local function
        /// </summary>
        Captures = 1 << 3,
    }
}