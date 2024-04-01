using System;

namespace TaskReminderApp.ViewModels
{
    internal class WshShellClass
    {
        // This class wraps the Windows Script Host Shell object for creating shortcuts

        internal IWshShortcut CreateShortcut(string shortcutPath)
        {
            throw new NotImplementedException();
        }
    }

    internal interface IWshShortcut
    {
        string TargetPath { get; set; }
        string Arguments { get; set; }
        string WorkingDirectory { get; set; }
        void Save();
    }
}