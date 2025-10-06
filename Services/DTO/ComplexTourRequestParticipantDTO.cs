using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookingApp.Domain.Model;

namespace BookingApp.Services.DTO
{
    public class ComplexTourRequestParticipantDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _complexTourRequestPartId;
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

        public int ComplexTourRequestPartId
        {
            get => _complexTourRequestPartId;
            set { _complexTourRequestPartId = value; OnPropertyChanged(); }
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

        public ComplexTourRequestParticipantDTO() { }

        public ComplexTourRequestParticipantDTO(ComplexTourRequestParticipant participant)
        {
            if (participant == null)
                throw new ArgumentNullException(nameof(participant));

            Id = participant.Id;
            ComplexTourRequestPartId = participant.ComplexTourRequestPartId;
            FirstName = participant.FirstName;
            LastName = participant.LastName;
            Age = participant.Age;
        }

        public ComplexTourRequestParticipant ToComplexTourRequestParticipant()
        {
            return new ComplexTourRequestParticipant(Id, ComplexTourRequestPartId, FirstName, LastName, Age);
        }

        public static ComplexTourRequestParticipantDTO FromDomain(ComplexTourRequestParticipant participant)
        {
            return new ComplexTourRequestParticipantDTO(participant);
        }
    }
}