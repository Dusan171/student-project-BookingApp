using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;
using BookingApp.Model;
using System.Windows;


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

    public Tour()
    {
        KeyPoints = new List<KeyPoint>();
        StartTimes = new List<StartTourTime>();
        Images = new List<Images>();
    }

    public Tour(int id, string name, Location location, string description, string language,
                int maxTourists,int reservedSpots, double durationHours)
    {
        Id = id;
        Name = name;
        Location = location;
        Description = description;
        Language = language;
        MaxTourists = maxTourists;
        ReservedSpots = reservedSpots;
        DurationHours = durationHours;
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
        DurationHours.ToString(),
        keyPointIds,
        startTimeIds,
        imageIds
        };

        return csvValues;
    }


    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Name = values[1];
        Location = new Location { Id = int.Parse(values[2]) };
        Language = values[3];
        MaxTourists = int.Parse(values[4]);
        DurationHours = double.Parse(values[5]);

        KeyPoints = new List<KeyPoint>();
        if (!string.IsNullOrEmpty(values[6]))
        {
            var keyPointIds = values[6].Split('|');
            foreach (var id in keyPointIds)
            {
                KeyPoints.Add(new KeyPoint { Id = int.Parse(id) });
            }
        }

        StartTimes = new List<StartTourTime>();
        if (!string.IsNullOrEmpty(values[7]))
        {
            var startTimeIds = values[7].Split('|');
            foreach (var id in startTimeIds)
            {
                StartTimes.Add(new StartTourTime { Id = int.Parse(id) });
            }
        }

        Images = new List<Images>();
        if (!string.IsNullOrEmpty(values[8]))
        {
            var imageIds = values[8].Split('|');
            foreach (var id in imageIds)
            {
                Images.Add(new Images { Id = int.Parse(id) });
            }
        }
    }


}
