using BookingApp.DTO;
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class TouristAttendanceService : ITouristAttendanceService
    {
        private readonly ITouristAttendanceRepository _repository;

        public TouristAttendanceService(ITouristAttendanceRepository repository)
        {
            _repository = repository;
        }

        private TouristAttendanceDTO ToDTO(TouristAttendance model)
        {
            return new TouristAttendanceDTO
            {
                Id = model.Id,
                GuestId = model.GuestId,
                TourId = model.TourId,
                StartTourTime = model.StartTourTime,
                HasAppeared = model.HasAppeared,
                KeyPointJoinedAt = model.KeyPointJoinedAt
            };
        }

        private TouristAttendance ToModel(TouristAttendanceDTO dto)
        {
            return new TouristAttendance(
                dto.Id,
                dto.GuestId,
                dto.TourId,
                dto.StartTourTime,
                dto.HasAppeared,
                dto.KeyPointJoinedAt
            );
        }

        public List<TouristAttendanceDTO> GetAll() =>
            _repository.GetAll().Select(ToDTO).ToList();

        public TouristAttendanceDTO? GetById(int id)
        {
            var model = _repository.GetById(id);
            return model == null ? null : ToDTO(model);
        }

        public List<TouristAttendanceDTO> GetByTourId(int tourId) =>
            _repository.GetByTourId(tourId).Select(ToDTO).ToList();

        public List<TouristAttendanceDTO> GetByGuestId(int guestId) =>
            _repository.GetByGuestId(guestId).Select(ToDTO).ToList();

        public TouristAttendanceDTO Create(TouristAttendanceDTO dto)
        {
            var model = ToModel(dto);
            var created = _repository.Add(model);
            return ToDTO(created);
        }

        public TouristAttendanceDTO Update(TouristAttendanceDTO dto)
        {
            var model = ToModel(dto);
            var updated = _repository.Update(model);
            return ToDTO(updated);
        }

        public void Delete(int id)
        {
            var model = _repository.GetById(id);
            if (model != null)
                _repository.Delete(model);
        }
    }
}
