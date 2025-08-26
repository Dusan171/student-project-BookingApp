using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;
using System.Windows;
using BookingApp.Domain;


public class Tour : ISerializable

{
    public int Id { get; set; }
    public string Name { get; set; }
    public Location Location { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }
    public int MaxTourists { get; set; }
    public int ReservedSpots { get; set; }
    public List<KeyPoint> KeyPoints { get; set; }
    public List<StartTourTime> StartTimes { get; set; }
    public double DurationHours { get; set; }
    public List<Images> Images { get; set; }
    public bool IsFinished { get; set; }
    public Tour()
    {
        KeyPoints = new List<KeyPoint>();
        StartTimes = new List<StartTourTime>();
        Images = new List<Images>();
    }

    public Tour(int id, string name, Location location, string description, string language,
                 int maxTourists, int reservedSpots, double durationHours, bool isFinished = false)
    {
        Id = id;
        Name = name;
        Location = location;
        Description = description;
        Language = language;
        MaxTourists = maxTourists;
        ReservedSpots = reservedSpots;
        DurationHours = durationHours;
        IsFinished = isFinished;
        KeyPoints = new List<KeyPoint>();
        StartTimes = new List<StartTourTime>();
        Images = new List<Images>();
    }

    public string[] ToCSV()
    {
        string keyPointIds = string.Join("|", KeyPoints.Select(kp => kp.Id));
        string startTimeIds = string.Join("|", StartTimes.Select(st => st.Id));
        string imageIds = string.Join("|", Images.Select(img => img.Id));

        string[] csvValues = {
        Id.ToString(),
        Name,
        Location.Id.ToString(),
        Language,
        MaxTourists.ToString(),
        ReservedSpots.ToString(),
        DurationHours.ToString(),
        IsFinished.ToString(),
        keyPointIds,
        startTimeIds,
        imageIds
        };

        return csvValues;
    }


    public void FromCSV(string[] values)
    {
        if (values == null || values.Length < 7)
        {
            throw new ArgumentException("Invalid CSV data for Tour");
        }

        Id = int.Parse(values[0]);                
        Name = values[1] ?? string.Empty;            
        Location = new Location { Id = int.Parse(values[2]) }; 
        Language = values[3] ?? string.Empty;         
        MaxTourists = int.Parse(values[4]);          
        ReservedSpots = int.Parse(values[5]);        
        DurationHours = double.Parse(values[6]);
        IsFinished = bool.Parse(values[7]);

        KeyPoints = new List<KeyPoint>();
        StartTimes = new List<StartTourTime>();
        Images = new List<Images>();

        
        if (values.Length > 7 && !string.IsNullOrEmpty(values[8]))
        {
            var keyPointIds = values[8].Split(',');
            foreach (var id in keyPointIds)
            {
                if (int.TryParse(id, out int keyPointId))
                {
                    KeyPoints.Add(new KeyPoint { Id = keyPointId });
                }
            }
        }

        
        if (values.Length > 9 && !string.IsNullOrEmpty(values[9]))
        {
            var startTimeIds = values[9].Split(',');
            foreach (var id in startTimeIds)
            {
                if (int.TryParse(id, out int startTimeId))
                {
                    StartTimes.Add(new StartTourTime { Id = startTimeId });
                }
            }
        }

        
        if (values.Length > 10 && !string.IsNullOrEmpty(values[10]))
        {
            var imageIds = values[10].Split(',');
            foreach (var id in imageIds)
            {
                if (int.TryParse(id, out int imageId))
                {
                    Images.Add(new Images { Id = imageId });
                }
            }
        }
    }

    public int AvailableSpots
    {
        get
        {
            int available = MaxTourists - ReservedSpots;
            return available < 0 ? 0 : available;
        }
    }

}
