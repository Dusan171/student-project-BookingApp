using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IDemoScenario
    {
     
        void Initialize();
        bool ExecuteStep(int step);
        void Cleanup();
    }
}
