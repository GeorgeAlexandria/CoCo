using System;

namespace CSharpIdentifiers.Access.Member
{
    internal class Event
    {
        public event EventHandler Changed;

        public void Create()
        {
            Changed += Event_Changed;
            Changed -= Event_Changed;
        }

        private void Event_Changed(object sender, EventArgs e)
        {
        }
    }
}