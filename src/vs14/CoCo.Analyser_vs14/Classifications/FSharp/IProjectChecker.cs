using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.FSharp.Compiler.SourceCodeServices;

namespace CoCo.Analyser.Classifications.FSharp
{
    public interface IProjectChecker
    {
        ParseCheckResult ParseAndCheckFileInProject(IListener listener, FSharpProjectOptions projectOptions, string itemPath,
           SourceText itemContent, VersionStamp itemVersion);
    }
}