using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Toolkit.Uwp.Notifications;
using ClosedXML.Excel;



namespace TaskReminderApp.ViewModels
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainViewModel viewModel;

        private string _taskName;
        public string TaskName
        {
            get { return _taskName; }
            set
            {
                _taskName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaskName)));
            }
        }

        private DateTime _taskDate;
        public DateTime TaskDate
        {
            get { return _taskDate; }
            set
            {
                _taskDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaskDate)));
            }
        }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                _isCompleted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            }
        }

        private bool _isPinned;
        public bool IsPinned
        {
            get { return _isPinned; }
            set
            {
                _isPinned = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPinned)));
            }
        }

        private int _priority;
        public int Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Priority)));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            viewModel.SaveData("data.json");
        }

        // Modify the DeleteTaskCommand property in the MainViewModel class to use a private field
        private ICommand _deleteTaskCommand;
        public ICommand DeleteTaskCommand => _deleteTaskCommand ?? (_deleteTaskCommand = new RelayCommand(DeleteTask));

        private void DeleteTask(object parameter)
        {
            if (parameter is TaskItem task)
            {
                viewModel.Tasks.Remove(task);
                SaveData(); // Update data.json file
            }
        }

        private void SaveData()
        {
            try
            {
                // Serialize tasks to JSON
                string json = JsonConvert.SerializeObject(viewModel.Tasks);

                // Write JSON to file
                File.WriteAllText("data.json", json);

                MessageBox.Show("Data saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TaskItem> Tasks { get; set; }
        

        private TaskItem _selectedTask;
        private object ToastNotificationManagerCompat;

        public TaskItem SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTask)));
            }
        }

        public ICommand AddTaskCommand { get; }
        public ICommand CompleteTaskCommand { get; }
        public ICommand SetReminderCommand { get; }
        public ICommand ScheduleTaskCommand { get; }
        public ICommand PinToDesktopCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand SaveCommand { get; }

        private const string ExcelFilePath = "data.xlsx";
        //public ICommand DeleteTaskCommand { get; }
        public TaskItem task { get; private set; }

        public MainViewModel()
        {

            DeleteTaskCommand = new RelayCommand(DeleteTask);
            // Load data from file if it exists
            if (File.Exists("data.json"))
            {
                string json = File.ReadAllText("data.json");
                Tasks = JsonConvert.DeserializeObject<ObservableCollection<TaskItem>>(json);
                DeleteTaskCommand = new RelayCommand(DeleteTask);

            }
            else
            {
                Tasks = new ObservableCollection<TaskItem>();
                DeleteTaskCommand = new RelayCommand(DeleteTask);

            }

            AddTaskCommand = new RelayCommand(AddTask);
            CompleteTaskCommand = new RelayCommand(CompleteTask);
            SetReminderCommand = new RelayCommand(SetReminder, CanSetReminder);
            ScheduleTaskCommand = new RelayCommand(ScheduleTask, CanScheduleTask);
            PinToDesktopCommand = new RelayCommand(PinToDesktop, CanPinToDesktop);
            SaveCommand = new RelayCommand(SaveData);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
        }

        private void DeleteTask(object parameter)
        {
            if (parameter is TaskItem task)
            {
                // Show a confirmation dialog
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this task?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // If the user clicks Yes, delete the task
                if (result == MessageBoxResult.Yes)
                {
                    Tasks.Remove(task);

                    // Save the changes after deleting the task
                    SaveData("data.json");
                }
            }
        }

        /*private void DeleteTask(object parameter)
        {
            if (task is TaskItem taskItem)
            {
                // Show a confirmation dialog
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this task?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // If the user clicks Yes, delete the task
                if (result == MessageBoxResult.Yes)
                {
                    Tasks.Remove(taskItem);
                    SaveData("data.json"); // Update data.json file
                }
            }
        }
        */

        private void AddTask(object parameter)
        {
            Tasks.Add(new TaskItem { TaskName = "New Task", TaskDate = DateTime.Now });
        }

        private void CompleteTask(object parameter)
        {
            if (SelectedTask != null)
            {
                SelectedTask.IsCompleted = true;
                SaveData(); // Save data when a task is completed

            }
        }

        private void SaveData()
        {
            try
            {
                // Serialize tasks to JSON
                string json = JsonConvert.SerializeObject(Tasks);

                // Write JSON to file
                File.WriteAllText("data.json", json);

                MessageBox.Show("Data saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }
    

        private void SetReminder(object parameter)
        {
            if (SelectedTask != null)
            {
                // Calculate the time remaining
                TimeSpan timeRemaining = SelectedTask.TaskDate - DateTime.Now;

                // If time remaining is one day
                if (timeRemaining.Days == 1)
                {
                    // Construct the toast notification content
                    ToastContentBuilder contentBuilder = new ToastContentBuilder()
                        .AddText("Reminder: " + SelectedTask.TaskName)
                        .AddText("Time Remaining: " + timeRemaining.ToString("d' day(s) 'h' hour(s) 'm' minute(s)'"));

                    // Add the reminder time
                    DateTimeOffset dueTime = DateTimeOffset.Now.AddMinutes(1); // Remind after 1 minute (for testing purposes)

                    // Schedule the toast notification
                    //ScheduledToastNotification scheduledNotification = new ScheduledToastNotification(contentBuilder.GetToastContent().GetXml(), dueTime);
                    //ToastNotificationManagerCompat.CreateToastNotifier().AddToSchedule(scheduledNotification);
                }

            }
        }



        private bool CanSetReminder(object parameter)
        {
            return SelectedTask != null; // Allow setting a reminder only if a task is selected
        }

        private void ScheduleTask(object parameter)
        {
            if (SelectedTask != null)
            {
                // Implement scheduling logic here
            }
        }

        private bool CanScheduleTask(object parameter)
        {
            return SelectedTask != null; // Allow scheduling a task only if a task is selected
        }



        private bool CanPinToDesktop(object parameter)
        {
            return SelectedTask != null && !SelectedTask.IsPinned; // Enable pinning if a task is selected and not already pinned
        }


        public void SaveData(object parameter)
        {
            try
            {
                // Serialize tasks to JSON
                string json = JsonConvert.SerializeObject(Tasks);

                // Write JSON to file
                File.WriteAllText("data.json", json);

                MessageBox.Show("Data saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }

        private void PinToDesktop(object parameter)
        {
            if (SelectedTask != null)
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string shortcutPath = Path.Combine(desktopPath, $"{SelectedTask.TaskName}.lnk");

                // Create shortcut
                WshShellClass wshShell = new WshShellClass();
                IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                shortcut.Arguments = $"--task \"{SelectedTask.TaskName}\""; // Pass task name as argument
                shortcut.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                shortcut.Save();

                MessageBox.Show($"Task '{SelectedTask.TaskName}' pinned to desktop.");
            }
        }
    }

    internal class ScheduledToastNotification
    {
        private DateTimeOffset dueTime;
        private object p;

        public ScheduledToastNotification(DateTimeOffset dueTime)
        {
            this.dueTime = dueTime;
        }

        public ScheduledToastNotification(object p, DateTimeOffset dueTime)
        {
            this.p = p;
            this.dueTime = dueTime;
        }
    }

    internal class ToastNotification
    {
        private object p;

        public ToastNotification(object p)
        {
            this.p = p;
        }
    }

    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public class TaskItem
    {
       // public string TaskName { get; set; }
        //public DateTime TaskDate { get; set; }
        //public bool IsCompleted { get; set; }
        //public bool IsPinned { get; set; }
        //public int Priority { get; set; }


        public string TaskName { get; set; }
        public DateTime TaskDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsPinned { get; set; }
        public int Priority { get; set; }

        // Property to calculate and display remaining time
        public TimeSpan TimeRemaining => TaskDate - DateTime.Now;

        //private DateTime _taskDate;
        /*
        public DateTime TaskDate
        {
            get { return _taskDate; }
            set
            {
                _taskDate = value;
                OnPropertyChanged(nameof(TaskDate));
                CalculateTimeRemaining();
            }
        }

        

        private string _timeRemaining;
        public string TimeRemaining
        {
            get { return _timeRemaining; }
            set
            {
                _timeRemaining = value;
                OnPropertyChanged(nameof(TimeRemaining));
            }
        }

        */

        /*
       public TaskItem()
       {
           // Initialize TaskDate with today's date
           TaskDate = DateTime.Today;

           // Calculate time remaining when TaskDate property is changed
           PropertyChanged += (sender, args) =>
           {
               if (args.PropertyName == nameof(TaskDate))
               {
                   CalculateTimeRemaining();
               }
           };
           CalculateTimeRemaining();// Calculate initial time difference
       }

       /*
       private void CalculateTimeRemaining()
       {
           TimeSpan difference = TaskDate - DateTime.Now;
           TimeRemaining = $"{(int)difference.TotalDays} days {(int)difference.Hours} hours {(int)difference.Minutes} minutes";
       }*/

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}