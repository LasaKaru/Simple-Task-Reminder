using System;

namespace TaskReminderApp.ViewModels
{
    internal class NotifyIcon
    {
        public object Icon { get; internal set; }
        public ContextMenuStrip ContextMenuStrip { get; internal set; }
        public string BalloonTipTitle { get; internal set; }
        public string BalloonTipText { get; internal set; }

        internal void ShowBalloonTip(int v)
        {
            throw new NotImplementedException();
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}