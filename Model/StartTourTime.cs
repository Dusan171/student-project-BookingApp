using System;
using BookingApp.Serializer;
public class StartTourTime : ISerializable
{
    public int Id { get; set; }
    public DateTime Time { get; set; }

    public StartTourTime()
    {
    }

    public StartTourTime(int id, DateTime time)
    {
        Id = id;
        Time = time;
    }

    public string[] ToCSV()
    {
        return new string[]
        {
        Id.ToString(),
        Time.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }


    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Time = DateTime.ParseExact(values[1], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
    }
}

