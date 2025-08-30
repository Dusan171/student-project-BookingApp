using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IKeyPointService
    {
        List<KeyPointDTO> GetAllKeyPoints();
        KeyPointDTO GetKeyPointById(int id);
        KeyPointDTO AddKeyPoint(KeyPointDTO keyPointDto);
        void DeleteKeyPoint(KeyPointDTO keyPointDto);
        KeyPointDTO UpdateKeyPoint(KeyPointDTO keyPointDto);
    }
}
