using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CARS.Backend.Common
{
    public enum SearchComparator
    {
        None,
        Equal,
        NotEqual,
		Greater,
		Less
    }

    public enum SearchType
    {
        None,
        SearchString,
        SearchNotString
    }

    public enum Sex
    {
        None,
        Male,
        Female
    }

    public enum LeaveStatus
    {
        None = 0,
        Applying = 1,
        Rejected = 2,
        Accepted = 3,
        Cancelling = 4,
        Cancelled = 5
    }

    public enum RoleRank
    {
        None,
        Admin,
        Manager,
        Employee
    }

    public enum MonthRank
    {
        None = 0,
        Jan,
        Feb,
        Mar,
        Apr,
        May,
        Jun,
        Jul,
        Aug,
        Sep,
        Oct,
        Nov,
        Dec
    }

	public enum OrderType
	{
		ASC,
		DESC
	}

    class GlobalEnums
    {
        
    }
}
