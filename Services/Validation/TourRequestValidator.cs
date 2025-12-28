using BookingApp.Services.DTO;
using System;

namespace BookingApp.Services.Validation
{
    public class TourRequestValidator
    {
        public bool ValidateRequest(TourRequestDTO requestDTO)
        {
            if (requestDTO == null)
                return false;

            return ValidateBasicFields(requestDTO) && ValidateDateRange(requestDTO);
        }

        private bool ValidateBasicFields(TourRequestDTO requestDTO)
        {
            return !string.IsNullOrWhiteSpace(requestDTO.City) &&
                   !string.IsNullOrWhiteSpace(requestDTO.Country) &&
                   !string.IsNullOrWhiteSpace(requestDTO.Language) &&
                   requestDTO.NumberOfPeople > 0;
        }

        private bool ValidateDateRange(TourRequestDTO requestDTO)
        {
            return requestDTO.DateFrom > DateTime.Now.AddDays(1) &&
                   requestDTO.DateTo > requestDTO.DateFrom;
        }
    }
}