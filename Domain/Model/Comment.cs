using BookingApp.Serializer;
using System;
using System.Globalization;

namespace BookingApp.Domain.Model
{
    public class Comment : ISerializable
    {
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string Text { get; set; }
        public User User { get; set; }

        public Comment() { }

        public Comment(DateTime creationTime, string text, User user)
        {
            CreationTime = creationTime;
            Text = text;
            User = user;
        }

        public string[] ToCSV()
        {
            string[] csvValues = { Id.ToString(), CreationTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), Text, User.Id.ToString() };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            if (!DateTime.TryParse(values[1], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
              
                parsedDate = DateTime.ParseExact(values[1],
                    new[] { "M/d/yyyy h:mm:ss tt", "yyyy-MM-dd HH:mm:ss", "dd-MM-yyyy HH:mm:ss" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
            }
            CreationTime = parsedDate;
            Text = values[2];
            User = new User() { Id = Convert.ToInt32(values[3]) };
        }
    }
}
