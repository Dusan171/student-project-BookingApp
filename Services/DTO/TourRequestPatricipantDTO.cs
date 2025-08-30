using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Model;

namespace BookingApp.Services.DTO
{
    public class TourRequestParticipantDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _tourRequestId;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private int _age;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public int TourRequestId
        {
            get => _tourRequestId;
            set { _tourRequestId = value; OnPropertyChanged(); }
        }

        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); }
        }

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(); }
        }

        public string FullName => $"{FirstName} {LastName}".Trim();
        public string DisplayText => $"{FullName}, {Age} god.";

        public TourRequestParticipantDTO() { }

        public TourRequestParticipantDTO(TourRequestParticipant participant)
        {
            if (participant == null)
                throw new ArgumentNullException(nameof(participant));

            Id = participant.Id;
            TourRequestId = participant.TourRequestId;
            FirstName = participant.FirstName;
            LastName = participant.LastName;
            Age = participant.Age;
        }

        public TourRequestParticipant ToTourRequestParticipant()
        {
            return new TourRequestParticipant(Id, TourRequestId, FirstName, LastName, Age);
        }

        public static TourRequestParticipantDTO FromDomain(TourRequestParticipant participant)
        {
            return new TourRequestParticipantDTO(participant);
        }
    }
}
