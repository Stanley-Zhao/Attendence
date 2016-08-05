using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using CARS.Backend.Common;

namespace CARS.Backend.DAL
{
    public class CommonConnection
    {
        public static SqlConnection Conn
        {
            get
            {
                return new SqlConnection(ConfigurationManager.AppSettings[GlobalParams.ConnNodeName]);
            }
        }
    }
}
