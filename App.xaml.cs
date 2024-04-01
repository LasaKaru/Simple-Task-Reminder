using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TaskReminderApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length > 0 && e.Args[0] == "--task")
            {
                // Retrieve the task name from the argument
                string taskName = e.Args[1];

                // Handle the task as needed (e.g., show details)
                MessageBox.Show($"Task '{taskName}' was selected.");
            }
        }
    }

    
}
