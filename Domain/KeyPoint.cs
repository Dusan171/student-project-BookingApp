using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using System.ComponentModel;

public class KeyPoint : ISerializable
{
    public int Id { get; set; }
    public string Name { get; set; }

    public KeyPoint()
    {
    }
    public KeyPoint(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public string[] ToCSV()
    {
        return new string[] { $"{Id}|{Name}" };
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Name = values[1];
    }

    public override string? ToString()
    {
        return $"{Name}";
    }
}
