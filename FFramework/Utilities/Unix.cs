using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFramework.Utilities
{
    public class Unix
    {
        ///<summary>Function used to transform current date to total seconds that have passed since 1.1.1970.</summary>
        ///<returns>Total amount of seconds passed from 1.1.1970.</returns>
        public static int GetCurrentUnixTimestamp()
        {
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        ///<summary>Function used to transform unix timestamp to human readable date</summary>
        ///<returns>DateTime object for given unix timestamp</returns>
        public static DateTime GetDatetimeFromUnixTimestamp(int ut)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Add(TimeSpan.FromSeconds(ut)).ToLocalTime();
        }
    }
}
