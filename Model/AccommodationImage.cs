using BookingApp.Serializer;
using System;


namespace BookingApp.Model
{
    public class AccommodationImage : ISerializable
    {
        public int Id { get; set; }
        public string Path { get; set; }

        public int AccommodationId {  get; set; } 

        public AccommodationImage() { }

        public AccommodationImage(int id, string path, int accommodationId)
        {
            Id = id;
            Path = path;
            AccommodationId = accommodationId;
        }


        public string[] ToCSV()
        {

            string[] csvValues = {
               Id.ToString(),
               Path.ToString(),
               AccommodationId.ToString(),


            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Path =values[1];
            AccommodationId = int.Parse(values[2]);
        }





    }
}
