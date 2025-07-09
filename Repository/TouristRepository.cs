using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Model;
using BookingApp.Serializer;

namespace BookingApp.Repository
{
    internal class TouristRepository
    {
        private const string FilePath = "C:/Users/PC/Desktop/5 semestar/sims-projekat/sims-ra-2025-group-7-team-b/Resources/Data/tourists.csv";

        private readonly Serializer<Tourist> _serializer;
        private List<Tourist> _tourists;

        public TouristRepository()
        {
            _serializer = new Serializer<Tourist>();
            _tourists = _serializer.FromCSV(FilePath);
        }

        public List<Tourist> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public Tourist GetById(int id)
        {
            _tourists = _serializer.FromCSV(FilePath);
            return _tourists.FirstOrDefault(t => t.Id == id);
        }

        public Tourist Save(Tourist tourist)
        {
            tourist.Id = NextId();
            _tourists = _serializer.FromCSV(FilePath);
            _tourists.Add(tourist);
            _serializer.ToCSV(FilePath, _tourists);
            return tourist;
        }

        public int NextId()
        {
            _tourists = _serializer.FromCSV(FilePath);
            if (_tourists.Count == 0) return 1;
            return _tourists.Max(t => t.Id) + 1;
        }

        public void Delete(Tourist tourist)
        {
            _tourists = _serializer.FromCSV(FilePath);
            var toRemove = _tourists.FirstOrDefault(t => t.Id == tourist.Id);
            if (toRemove != null)
            {
                _tourists.Remove(toRemove);
                _serializer.ToCSV(FilePath, _tourists);
            }
        }

        public Tourist Update(Tourist tourist)
        {
            _tourists = _serializer.FromCSV(FilePath);
            var current = _tourists.FirstOrDefault(t => t.Id == tourist.Id);
            if (current != null)
            {
                int index = _tourists.IndexOf(current);
                _tourists[index] = tourist;
                _serializer.ToCSV(FilePath, _tourists);
            }
            return tourist;
        }
    }
}
