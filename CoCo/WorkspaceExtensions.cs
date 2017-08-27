using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace CoCo
{
    public static class WorkspaceExtensions
    {
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
    }
}
