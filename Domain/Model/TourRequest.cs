using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public enum TourRequestStatus
    {
        PENDING,    // Na čekanju
        ACCEPTED,   // Prihvaćen
        INVALID     // Nevažeći
    }
    public class TourRequest : ISerializable
    {
        public int Id { get; set; }
        public int TouristId { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int NumberOfPeople { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public TourRequestStatus Status { get; set; }
        public int? AcceptedByGuideId { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? ScheduledDate { get; set; }

        public List<TourRequestParticipant> Participants { get; set; } = new List<TourRequestParticipant>();
        public User Tourist { get; set; }
        public User AcceptedGuide { get; set; }
        public string TouristName => Tourist != null
            ? Tourist.FirstName + " " + Tourist.LastName
            : $"Unknown Tourist {TouristId}";
        public bool IsValid => Status != TourRequestStatus.INVALID && DateFrom > DateTime.Now.AddDays(2);

        public TourRequest()
        {
            CreatedAt = DateTime.Now;
            Status = TourRequestStatus.PENDING;
            Participants = new List<TourRequestParticipant>();
        }

        public TourRequest(int id, int touristId, string city, string country, string description,
                          string language, int numberOfPeople, DateTime dateFrom, DateTime dateTo)
        {
            Id = id;
            TouristId = touristId;
            City = city ?? string.Empty;
            Country = country ?? string.Empty;
            Description = description ?? string.Empty;
            Language = language ?? string.Empty;
            NumberOfPeople = numberOfPeople;
            DateFrom = dateFrom;
            DateTo = dateTo;
            CreatedAt = DateTime.Now;
            Status = TourRequestStatus.PENDING;
            Participants = new List<TourRequestParticipant>();
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                TouristId.ToString(),
                City,
                Country,
                Description,
                Language,
                NumberOfPeople.ToString(),
                DateFrom.ToString("dd-MM-yyyy"),
                DateTo.ToString("dd-MM-yyyy"),
                CreatedAt.ToString("dd-MM-yyyy HH:mm:ss"),
                Status.ToString(),
                AcceptedByGuideId?.ToString() ?? "",
                AcceptedDate?.ToString("dd-MM-yyyy HH:mm:ss") ?? "",
                ScheduledDate?.ToString("dd-MM-yyyy HH:mm:ss") ?? ""
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 11)
                throw new ArgumentException("Invalid CSV data for TourRequest");

            Id = int.Parse(values[0]);
            TouristId = int.Parse(values[1]);
            City = values[2] ?? string.Empty;
            Country = values[3] ?? string.Empty;
            Description = values[4] ?? string.Empty;
            Language = values[5] ?? string.Empty;
            NumberOfPeople = int.Parse(values[6]);

            DateFrom = DateTime.ParseExact(values[7], "dd-MM-yyyy", null);
            DateTo = DateTime.ParseExact(values[8], "dd-MM-yyyy", null);
            CreatedAt = DateTime.ParseExact(values[9], "dd-MM-yyyy HH:mm:ss", null);

            Status = (TourRequestStatus)Enum.Parse(typeof(TourRequestStatus), values[10]);

            if (values.Length > 11 && !string.IsNullOrEmpty(values[11]))
                AcceptedByGuideId = int.Parse(values[11]);

            if (values.Length > 12 && !string.IsNullOrEmpty(values[12]))
                AcceptedDate = DateTime.ParseExact(values[12], "dd-MM-yyyy HH:mm:ss", null);

            if (values.Length > 13 && !string.IsNullOrEmpty(values[13]))
                ScheduledDate = DateTime.ParseExact(values[13], "dd-MM-yyyy HH:mm:ss", null);

            Participants = new List<TourRequestParticipant>();
        }

    }
}
