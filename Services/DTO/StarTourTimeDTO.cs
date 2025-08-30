using BookingApp.Domain.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingApp.Services.DTO
{
    public class StartTourTimeDTO : INotifyPropertyChanged
    {
        private int _id;
        private DateTime _time;
        private string _timeText = string.Empty;
        private string _dateText = string.Empty;
        private string _displayText = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    UpdateDisplayTexts();
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TimeText));
                    OnPropertyChanged(nameof(DateText));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public string TimeText
        {
            get => _timeText;
            private set
            {
                if (_timeText != value)
                {
                    _timeText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DateText
        {
            get => _dateText;
            private set
            {
                if (_dateText != value)
                {
                    _dateText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DisplayText
        {
            get => _displayText;
            private set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged();
                }
            }
        }

        public StartTourTimeDTO()
        {
            Time = DateTime.Now;
        }

        public StartTourTimeDTO(StartTourTime startTourTime)
        {
            if (startTourTime == null)
                throw new ArgumentNullException(nameof(startTourTime));

            Id = startTourTime.Id;
            Time = startTourTime.Time;
        }

        private void UpdateDisplayTexts()
        {
            TimeText = _time.ToString("HH:mm");
            DateText = _time.ToString("dd.MM.yyyy");
            DisplayText = $"{DateText} {TimeText}";
        }

        public StartTourTime ToStartTourTime()
        {
            return new StartTourTime(Id, Time);
        }

        public static StartTourTimeDTO FromDomain(StartTourTime startTourTime)
        {
            return new StartTourTimeDTO(startTourTime);
        }

        public override string ToString()
        {
            return DisplayText;
        }
    }
}