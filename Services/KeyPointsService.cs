using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Services.DTO;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Services
{
    public class KeyPointService : IKeyPointService
    {
        private readonly KeyPointRepository _keyPointRepository;

        public KeyPointService(KeyPointRepository keyPointRepository)
        {
            _keyPointRepository = keyPointRepository;
        }

        public List<KeyPointDTO> GetAllKeyPoints()
        {
            return _keyPointRepository.GetAll().Select(kp => new KeyPointDTO(kp)).ToList();
        }

        public KeyPointDTO GetKeyPointById(int id)
        {
            var keyPoint = _keyPointRepository.GetById(id);
            return keyPoint == null ? null : new KeyPointDTO(keyPoint);
        }

        public KeyPointDTO AddKeyPoint(KeyPointDTO keyPointDto)
        {
            var keyPoint = keyPointDto.ToKeyPoint();
            _keyPointRepository.Save(keyPoint);
            return new KeyPointDTO(keyPoint);
        }

        public void DeleteKeyPoint(KeyPointDTO keyPointDto)
        {
            _keyPointRepository.Delete(keyPointDto.ToKeyPoint());
        }

        public KeyPointDTO UpdateKeyPoint(KeyPointDTO keyPointDto)
        {
            var updated = _keyPointRepository.Update(keyPointDto.ToKeyPoint());
            return new KeyPointDTO(updated);
        }
    }
}
