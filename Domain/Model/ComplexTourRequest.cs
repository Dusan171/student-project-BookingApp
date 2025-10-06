using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public enum ComplexTourRequestStatus
    {
        PENDING,    // Na čekanju
        ACCEPTED,   // Prihvaćen
        INVALID     // Nevažeći
    }

    public class ComplexTourRequest : ISerializable
    {
        public int Id { get; set; }
        public int TouristId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ComplexTourRequestStatus Status { get; set; }
        public DateTime InvalidationDeadline { get; set; }
        public List<ComplexTourRequestPart> Parts { get; set; } = new List<ComplexTourRequestPart>();

        public ComplexTourRequest()
        {
            CreatedAt = DateTime.Now;
            Status = ComplexTourRequestStatus.PENDING;
        }

        public ComplexTourRequest(int id, int touristId)
        {
            Id = id;
            TouristId = touristId;
            CreatedAt = DateTime.Now;
            Status = ComplexTourRequestStatus.PENDING;
            Parts = new List<ComplexTourRequestPart>();
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                TouristId.ToString(),
                CreatedAt.ToString("dd-MM-yyyy HH:mm:ss"),
                Status.ToString(),
                InvalidationDeadline.ToString("dd-MM-yyyy HH:mm:ss")
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 5)
                throw new ArgumentException("Invalid CSV data for ComplexTourRequest");

            Id = int.Parse(values[0]);
            TouristId = int.Parse(values[1]);

           
            CreatedAt = DateTime.Parse(values[2]);

            Status = (ComplexTourRequestStatus)Enum.Parse(typeof(ComplexTourRequestStatus), values[3]);

   
            InvalidationDeadline = DateTime.Parse(values[4]);

            Parts = new List<ComplexTourRequestPart>();
        }

        public bool CheckValidity()
        {
            return DateTime.Now <= InvalidationDeadline && Status != ComplexTourRequestStatus.INVALID;
        }

        public void UpdateStatus()
        {
            if (!CheckValidity())
            {
                Status = ComplexTourRequestStatus.INVALID;
                return;
            }

            if (Parts.All(p => p.Status == TourRequestStatus.ACCEPTED))
            {
                Status = ComplexTourRequestStatus.ACCEPTED;
            }
            else if (Parts.Any(p => p.Status == TourRequestStatus.INVALID))
            {
                Status = ComplexTourRequestStatus.INVALID;
            }
        }
    }
}