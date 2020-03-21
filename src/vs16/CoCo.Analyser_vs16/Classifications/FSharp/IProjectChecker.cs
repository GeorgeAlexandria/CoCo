using FSharp.Compiler.SourceCodeServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace CoCo.Analyser.Classifications.FSharp
{
    public interface IProjectChecker
    {
        ParseCheckResult ParseAndCheckFileInProject(IListener listener, FSharpProjectOptions projectOptions, string itemPath,
           SourceText itemContent, VersionStamp itemVersion);
    }
}