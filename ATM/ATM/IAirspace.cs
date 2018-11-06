using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM
{
    public interface IAirspace
    {
        bool CheckIfInMonitoredArea(double xCord, double yCord, double zCord);

    }
}
