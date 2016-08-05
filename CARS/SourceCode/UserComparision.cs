using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace CARS.SourceCode
{
    public class UserComparision : IComparer<User>
    {
        public int Compare(User x, User y)
        {
            int result = 0;
            try
            {
                if (null != x && null != y
                    && !string.IsNullOrEmpty(x.ToString()) 
                    && !string.IsNullOrEmpty(y.ToString()))
                {
                    result = x.ToString().CompareTo(y.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to compare these two users.", ex);
            }

            return result;
        }
    }
}