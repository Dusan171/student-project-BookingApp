using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;

public enum TourStatus { NONE, ACTIVE, FINISHED, CANCELLED }

public class Tour : ISerializable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Location Location { get; set; } = new Location();
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int MaxTourists { get; set; }
    public int ReservedSpots { get; set; }
    public List<KeyPoint> KeyPoints { get; set; } = new List<KeyPoint>();
    public List<StartTourTime> StartTimes { get; set; } = new List<StartTourTime>();
    public double DurationHours { get; set; }
    public List<Images> Images { get; set; } = new List<Images>();
    public TourStatus Status { get; set; } = TourStatus.NONE;
    public User Guide { get; set; } = new User();

    public int AvailableSpots
    {
        get
        {
            int available = MaxTourists - ReservedSpots;
            return available < 0 ? 0 : available;
        }
    }

    public Tour() { }

    public Tour(int id, string name, Location location, string description, string language,
                int maxTourists, int reservedSpots, double durationHours, TourStatus status = TourStatus.NONE, User? guide = null)
    {
        Id = id;
        Name = name ?? string.Empty;
        Location = location ?? new Location();
        Description = description ?? string.Empty;
        Language = language ?? string.Empty;
        MaxTourists = maxTourists;
        ReservedSpots = reservedSpots;
        DurationHours = durationHours;
        Status = status;
        Guide = guide ?? new User();

        KeyPoints = new List<KeyPoint>();
        StartTimes = new List<StartTourTime>();
        Images = new List<Images>();
    }

    public string[] ToCSV()
    {
        string keyPointIds = string.Join(",", KeyPoints.Select(kp => kp.Id));
        string startTimeIds = string.Join(",", StartTimes.Select(st => st.Id));
        string imageIds = string.Join(",", Images.Select(img => img.Id));

        return new string[]
        {
            Id.ToString(),
            Name,
            Location.Id.ToString(),
            Language,
            MaxTourists.ToString(),
            ReservedSpots.ToString(),
            DurationHours.ToString(),
            Status.ToString(),
            keyPointIds,
            startTimeIds,
            imageIds,
            Guide?.Id.ToString() ?? "0"
        };
    }

    public void FromCSV(string[] values)
    {
        if (values == null || values.Length < 7)
            throw new ArgumentException("Invalid CSV data for Tour");

        Id = int.Parse(values[0]);
        Name = values[1] ?? string.Empty;
        Location = new Location { Id = int.Parse(values[2]) };
        Language = values[3] ?? string.Empty;
        MaxTourists = int.Parse(values[4]);
        ReservedSpots = int.Parse(values[5]);
        DurationHours = double.Parse(values[6]);
        Status = Enum.Parse<TourStatus>(values[7]);

        KeyPoints = new List<KeyPoint>();
        StartTimes = new List<StartTourTime>();
        Images = new List<Images>();

        if (values.Length > 8 && !string.IsNullOrEmpty(values[8]))
        {
            foreach (var id in values[8].Split(','))
                if (int.TryParse(id, out int kpId))
                    KeyPoints.Add(new KeyPoint { Id = kpId });
        }

        if (values.Length > 9 && !string.IsNullOrEmpty(values[9]))
        {
            foreach (var id in values[9].Split(','))
                if (int.TryParse(id, out int stId))
                    StartTimes.Add(new StartTourTime { Id = stId });
        }

        if (values.Length > 10 && !string.IsNullOrEmpty(values[10]))
        {
            foreach (var id in values[10].Split(','))
                if (int.TryParse(id, out int imgId))
                    Images.Add(new Images { Id = imgId });
        }

        if (values.Length > 11 && int.TryParse(values[11], out int guideId))
            Guide = new User { Id = guideId };
    }
}
