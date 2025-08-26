using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Serializer;

namespace BookingApp.Domain
{
    public enum RequestStatus { Pending, Approved, Rejected}

    public class RescheduleRequest : ISerializable
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int GuestId { get; set; }
        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public RequestStatus Status { get; set; }
        public string OwnerComment { get; set; }
        public bool IsSeenByGuest { get; set; } //za notifikacije

        public RescheduleRequest()
        {
            OwnerComment = string.Empty; //inicijalizacija da ne bude null
        }
        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                ReservationId.ToString(),
                GuestId.ToString(),
                NewStartDate.ToString("o"),
                NewEndDate.ToString("o"),
                Status.ToString(),
                OwnerComment,
                IsSeenByGuest.ToString()
            };
        }
        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            ReservationId = int.Parse(values[1]);
            GuestId = int.Parse(values[2]);
            NewStartDate = DateTime.Parse(values[3]);
            NewEndDate = DateTime.Parse(values[4]);
            Status = Enum.Parse<RequestStatus>(values[5]);
            OwnerComment = values[6];
            IsSeenByGuest = bool.Parse(values[7]);
        }
    }
}
