using System;

namespace CSharpIdentifiers.Access.Member
{
    internal class TypeEvent
    {
        private class Wrapper
        {
            public static event EventHandler Changed;
        }

        public void Create() => Wrapper.Changed += TypeEvent_Changed;

        private void TypeEvent_Changed(object sender, EventArgs e) { }
    }
}