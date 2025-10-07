using System;
using System.Collections.Generic;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class ComplexTourRequestPart : ISerializable
    {
        public int Id { get; set; }
        public int ComplexTourRequestId { get; set; }
        public int TouristId { get; set; }
        public int PartIndex { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int NumberOfPeople { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public TourRequestStatus Status { get; set; } = TourRequestStatus.PENDING;
        public int? AcceptedByGuideId { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? ScheduledDate { get; set; }

        public List<ComplexTourRequestParticipant> Participants { get; set; } = new List<ComplexTourRequestParticipant>();

        public ComplexTourRequestPart() { }

        public ComplexTourRequestPart(int id, int complexTourRequestId, int touristId, int partIndex,
                                    string city, string country, string description, string language,
                                    int numberOfPeople, DateTime dateFrom, DateTime dateTo)
        {
            Id = id;
            ComplexTourRequestId = complexTourRequestId;
            TouristId = touristId;
            PartIndex = partIndex;
            City = city ?? string.Empty;
            Country = country ?? string.Empty;
            Description = description ?? string.Empty;
            Language = language ?? string.Empty;
            NumberOfPeople = numberOfPeople;
            DateFrom = dateFrom;
            DateTo = dateTo;
            Status = TourRequestStatus.PENDING;
            Participants = new List<ComplexTourRequestParticipant>();
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                ComplexTourRequestId.ToString(),
                TouristId.ToString(),
                PartIndex.ToString(),
                City,
                Country,
                Description,
                Language,
                NumberOfPeople.ToString(),
                DateFrom.ToString("dd-MM-yyyy"),
                DateTo.ToString("dd-MM-yyyy"),
                Status.ToString(),
                AcceptedByGuideId?.ToString() ?? "",
                AcceptedDate?.ToString("dd-MM-yyyy HH:mm:ss") ?? "",
                ScheduledDate?.ToString("dd-MM-yyyy HH:mm:ss") ?? ""
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 12)
                throw new ArgumentException("Invalid CSV data for ComplexTourRequestPart");

            Id = int.Parse(values[0]);
            ComplexTourRequestId = int.Parse(values[1]);
            TouristId = int.Parse(values[2]);
            PartIndex = int.Parse(values[3]);
            City = values[4] ?? string.Empty;
            Country = values[5] ?? string.Empty;
            Description = values[6] ?? string.Empty;
            Language = values[7] ?? string.Empty;
            NumberOfPeople = int.Parse(values[8]);
            var culture = new System.Globalization.CultureInfo("sr-Latn-RS");
            DateFrom = DateTime.ParseExact(values[9], "dd-MM-yyyy", culture);
            DateTo = DateTime.ParseExact(values[10], "dd-MM-yyyy", culture);
            Status = (TourRequestStatus)Enum.Parse(typeof(TourRequestStatus), values[11]);

            if (values.Length > 12 && !string.IsNullOrEmpty(values[12]))
                AcceptedByGuideId = int.Parse(values[12]);

            if (values.Length > 13 && !string.IsNullOrEmpty(values[13]))
                AcceptedDate = DateTime.ParseExact(values[13], "dd-MM-yyyy HH:mm:ss", culture);

            if (values.Length > 14 && !string.IsNullOrEmpty(values[14]))
                ScheduledDate = DateTime.ParseExact(values[14], "dd-MM-yyyy HH:mm:ss", culture);

            Participants = new List<ComplexTourRequestParticipant>();
        }
    }
}
