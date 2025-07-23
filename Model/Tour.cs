using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;
using BookingApp.Model;
using System.Windows;
using BookingApp.Repository;


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
    public bool IsFinished { get; set; }
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
        Id = int.Parse(values[0]);
        Name = values[1];
        Location = new Location { Id = int.Parse(values[2]) };
        Language = values[3];
        MaxTourists = int.Parse(values[4]);
        ReservedSpots = int.Parse(values[5]);
        DurationHours = double.Parse(values[6]);
        IsFinished = bool.Parse(values[7]);

        KeyPoints = new List<KeyPoint>();
        if (!string.IsNullOrEmpty(values[8]))
        {
            var keyPointIds = values[8].Split('|').Select(int.Parse).ToList();

            KeyPointRepository keyPointRepository = new KeyPointRepository();
            var keyPointsFromFile = keyPointRepository.GetAll();

            foreach (var id in keyPointIds)
            {
                var keyPointObj = keyPointsFromFile.FirstOrDefault(kp => kp.Id == id);
                if (keyPointObj != null)
                {
                    KeyPoints.Add(keyPointObj);
                }
                else
                {
                    Console.WriteLine($"Pokušavaš da pristupiš KeyPoint ID-ju koji ne postoji: {id}");
                }
            }
        }

        StartTimes = new List<StartTourTime>();
        if (!string.IsNullOrEmpty(values[9]))
        {
            StartTourTimeRepository startTourTimeRepository = new();
            var startTimesFromFile = startTourTimeRepository.GetAll();

            var startTimeIds = values[9].Split('|').Select(int.Parse).ToList();

            foreach (var id in startTimeIds)
            {
                var startTimeObj = startTimesFromFile.FirstOrDefault(st => st.Id == id);
                if (startTimeObj != null)
                {
                    StartTimes.Add(startTimeObj);
                }
                else
                {
                    Console.WriteLine($"Pokušavaš da pristupiš StartTime ID-ju koji ne postoji: {id}");
                }
            }
        }

        Images = new List<Images>();
        if (!string.IsNullOrEmpty(values[10]))
        {
            var imageIds = values[10].Split('|').Select(int.Parse).ToList();

            ImageRepository imageRepository = new ImageRepository();
            var imagesFromFile = imageRepository.GetAll();

            foreach (var id in imageIds)
            {
                var imageObj = imagesFromFile.FirstOrDefault(img => img.Id == id);
                if (imageObj != null)
                {
                    Images.Add(imageObj);
                }
                else
                {
                    Console.WriteLine($"Pokušavaš da pristupiš image ID-ju koji ne postoji: {id}");
                }
            }
        }
    }
}
