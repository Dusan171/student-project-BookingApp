using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IAnywhereSearchService
    {
        List<AnywhereSearchResultDTO> Search(AnywhereSearchParamsDTO searchParams);
    }
}