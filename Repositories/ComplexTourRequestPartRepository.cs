// ComplexTourRequestPartRepository.cs
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class ComplexTourRequestPartRepository : IComplexTourRequestPartRepository
    {
        private const string FilePath = "../../../Resources/Data/complex_tour_request_parts.csv";
        private readonly Serializer<ComplexTourRequestPart> _serializer;
        private List<ComplexTourRequestPart> _parts;

        public ComplexTourRequestPartRepository()
        {
            _serializer = new Serializer<ComplexTourRequestPart>();
            _parts = _serializer.FromCSV(FilePath);
        }

        public ComplexTourRequestPart GetById(int id)
        {
            return _parts.FirstOrDefault(p => p.Id == id);
        }

        public List<ComplexTourRequestPart> GetAll()
        {
            return _parts.ToList();
        }

        public ComplexTourRequestPart Save(ComplexTourRequestPart part)
        {
            part.Id = NextId();
            _parts.Add(part);
            _serializer.ToCSV(FilePath, _parts);
            return part;
        }

        public ComplexTourRequestPart Update(ComplexTourRequestPart part)
        {
            var existingPart = GetById(part.Id);
            if (existingPart != null)
            {
                existingPart.ComplexTourRequestId = part.ComplexTourRequestId;
                existingPart.PartIndex = part.PartIndex;
                existingPart.City = part.City;
                existingPart.Country = part.Country;
                existingPart.Description = part.Description;
                existingPart.Language = part.Language;
                existingPart.NumberOfPeople = part.NumberOfPeople;
                existingPart.DateFrom = part.DateFrom;
                existingPart.DateTo = part.DateTo;
                existingPart.Status = part.Status;
                existingPart.AcceptedByGuideId = part.AcceptedByGuideId;
                existingPart.AcceptedDate = part.AcceptedDate;
                existingPart.ScheduledDate = part.ScheduledDate;

                _serializer.ToCSV(FilePath, _parts);
                return existingPart;
            }
            return null;
        }

        public void Delete(ComplexTourRequestPart part)
        {
            _parts.RemoveAll(p => p.Id == part.Id);
            _serializer.ToCSV(FilePath, _parts);
        }

        public List<ComplexTourRequestPart> GetByComplexRequestId(int complexRequestId)
        {
            return _parts.Where(p => p.ComplexTourRequestId == complexRequestId)
                        .OrderBy(p => p.PartIndex)
                        .ToList();
        }

        public void DeleteByComplexRequestId(int complexRequestId)
        {
            _parts.RemoveAll(p => p.ComplexTourRequestId == complexRequestId);
            _serializer.ToCSV(FilePath, _parts);
        }

        private int NextId()
        {
            return _parts.Count > 0 ? _parts.Max(p => p.Id) + 1 : 1;
        }
    }
}