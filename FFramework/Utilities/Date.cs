using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFramework.Utilities
{
    public class Date
    {
        public static bool IsLeapYear(int year)
        {
            if (year % 4 == 0)
            {
                if (year % 100 == 0)
                {
                    if (year % 400 == 0) return true;
                    else return false;
                }
                else return true;
            }
            else return false;
        }

        public static int GetDaysInMonth(int month, int year)
        {
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) return 31;
            else if (month == 4 || month == 6 || month == 9 || month == 11) return 30;
            else if (month == 2) return IsLeapYear(year) == true ? 29 : 28;
            else return 0;
        }
    }
}
