using System;

namespace TaskReminderApp.ViewModels
{
    internal class ToolStripMenuItem
    {
        private string v;

        public ToolStripMenuItem(string v)
        {
            this.v = v;
        }

        public Action<object, EventArgs> Click { get; set; }
    }
}