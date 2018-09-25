namespace CSharpIdentifiers.Access.Namespaces
{
    internal class ByNamespace
    {
        private void Method()
        {
            System.Diagnostics.Tracing.EventSource.GetSources();
            // Access by global
            new global::System.Collections.BitArray(5);
        }
    }
}