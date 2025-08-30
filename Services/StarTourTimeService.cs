using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class StartTourTimeService : IStartTourTimeService
    {
        private readonly IStartTourTimeRepository _startTourTimeRepository;

        public StartTourTimeService(IStartTourTimeRepository startTourTimeRepository)
        {
            _startTourTimeRepository = startTourTimeRepository ?? throw new ArgumentNullException(nameof(startTourTimeRepository));
        }

        public List<StartTourTimeDTO> GetAllStartTourTimes()
        {
            var startTourTimes = _startTourTimeRepository.GetAll();
            return startTourTimes.Select(st => StartTourTimeDTO.FromDomain(st)).ToList();
        }

        public StartTourTimeDTO? GetStartTourTimeById(int id)
        {
            var startTourTime = _startTourTimeRepository.GetById(id);
            return startTourTime != null ? StartTourTimeDTO.FromDomain(startTourTime) : null;
        }

        public StartTourTimeDTO SaveStartTourTime(StartTourTimeDTO startTourTimeDTO)
        {
            if (startTourTimeDTO == null)
                throw new ArgumentNullException(nameof(startTourTimeDTO));

            if (!ValidateStartTourTime(startTourTimeDTO))
                throw new ArgumentException("StartTourTime validation failed", nameof(startTourTimeDTO));

            var startTourTime = startTourTimeDTO.ToStartTourTime();
            var savedStartTourTime = _startTourTimeRepository.Save(startTourTime);
            return StartTourTimeDTO.FromDomain(savedStartTourTime);
        }

        public StartTourTimeDTO UpdateStartTourTime(StartTourTimeDTO startTourTimeDTO)
        {
            if (startTourTimeDTO == null)
                throw new ArgumentNullException(nameof(startTourTimeDTO));

            if (!ValidateStartTourTime(startTourTimeDTO))
                throw new ArgumentException("StartTourTime validation failed", nameof(startTourTimeDTO));

            var startTourTime = startTourTimeDTO.ToStartTourTime();
            var updatedStartTourTime = _startTourTimeRepository.Update(startTourTime);
            return StartTourTimeDTO.FromDomain(updatedStartTourTime);
        }

        public void DeleteStartTourTime(StartTourTimeDTO startTourTimeDTO)
        {
            if (startTourTimeDTO == null)
                throw new ArgumentNullException(nameof(startTourTimeDTO));

            var startTourTime = startTourTimeDTO.ToStartTourTime();
            _startTourTimeRepository.Delete(startTourTime);
        }

        public List<StartTourTimeDTO> GetStartTourTimesByTourId(int tourId)
        {
            var startTourTimes = _startTourTimeRepository.GetByTourId(tourId);
            return startTourTimes.Select(st => StartTourTimeDTO.FromDomain(st)).ToList();
        }

        public bool IsTimeAvailable(StartTourTimeDTO startTourTimeDTO)
        {
            if (startTourTimeDTO == null)
                return false;

            // Proverava da li postoji vreme u istom datumu i vremenu
            var existingTimes = _startTourTimeRepository.GetAll();
            return !existingTimes.Any(st =>
                st.Id != startTourTimeDTO.Id && // Ignoriši trenutni objekat pri ažuriranju
                st.Time.Date == startTourTimeDTO.Time.Date &&
                Math.Abs((st.Time - startTourTimeDTO.Time).TotalMinutes) < 30);
        }

        public List<StartTourTimeDTO> GetAvailableTimesForDate(DateTime date)
        {
            var allTimes = _startTourTimeRepository.GetAll();
            var timesForDate = allTimes.Where(st => st.Time.Date == date.Date);
            return timesForDate.Select(st => StartTourTimeDTO.FromDomain(st)).ToList();
        }

        public bool ValidateStartTourTime(StartTourTimeDTO startTourTimeDTO)
        {
            if (startTourTimeDTO == null)
                return false;

            // Vreme ne može biti u prošlosti
            if (startTourTimeDTO.Time < DateTime.Now)
                return false;

            // Proverava dostupnost vremena
            if (!IsTimeAvailable(startTourTimeDTO))
                return false;

            // Vreme mora biti u radnim satima (8:00 - 20:00)
            var timeOfDay = startTourTimeDTO.Time.TimeOfDay;
            if (timeOfDay < new TimeSpan(8, 0, 0) || timeOfDay > new TimeSpan(20, 0, 0))
                return false;

            return true;
        }

        public List<StartTourTimeDTO> GetFutureStartTimes()
        {
            var allTimes = _startTourTimeRepository.GetAll();
            var futureTimes = allTimes.Where(st => st.Time > DateTime.Now);
            return futureTimes.Select(st => StartTourTimeDTO.FromDomain(st)).ToList();
        }

        public List<StartTourTimeDTO> GetStartTimesInDateRange(DateTime startDate, DateTime endDate)
        {
            var allTimes = _startTourTimeRepository.GetAll();
            var timesInRange = allTimes.Where(st =>
                st.Time.Date >= startDate.Date &&
                st.Time.Date <= endDate.Date);
            return timesInRange.Select(st => StartTourTimeDTO.FromDomain(st)).ToList();
        }
    }
}