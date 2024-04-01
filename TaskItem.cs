using System;
using System.ComponentModel;

namespace TaskReminderApp
{
    public class TaskItem : INotifyPropertyChanged
    {
        //public string TaskName { get; internal set; }
        //public DateTime TaskDate { get; internal set; }
        //public bool IsCompleted { get; internal set; }

        private string _priority;
        public string Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Priority)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

                // Update TimeRemaining when TaskDate changes
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeRemaining)));
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

        private TimeSpan _timeRemaining;
        public TimeSpan TimeRemaining
        {
            get
            {
                // Calculate the time remaining based on TaskDate and current date
                return TaskDate - DateTime.Now;
            }
            set
            {
                _timeRemaining = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeRemaining)));
            }
        }
    




        private void UpdateTimeRemaining()
        {
            // Calculate TimeRemaining based on TaskDate and current date/time
            TimeRemaining = TaskDate - DateTime.Now;
        }

        public bool IsPinned { get; internal set; }
    }
}
