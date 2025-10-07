using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IHomeStatisticsService
    {
        HomeStatisticDTO GetOwnerStatistics(int ownerId);
    }
}
