using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CARS.Backend.Entity;

namespace CARS.Backend.Common
{
	class LeaveInfoComparision:IComparer<LeaveInfo>
	{
        public int Compare(LeaveInfo x, LeaveInfo y)
        {
            if ((int)x.Status != (int)y.Status)
            {
                return ((int)x.Status).CompareTo((int)y.Status);
            }
            else if (x.FKSubmitEmployeeID != y.FKSubmitEmployeeID)
            {
                return x.FKSubmitEmployeeID.CompareTo(y.FKSubmitEmployeeID);
            }
            else
            {
                return x.FirstStartTime.CompareTo(y.FirstStartTime);
            }
        }
	}
}
