using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;

namespace FFramework.Utilities
{
    public class IP
    {
        ///<summary>Function that retrieves your computer's public IP from an external website</summary>
        ///<returns>Your computer's public IP address</returns>
        public static string GetPublicIP()
        {
            try
            {
                string externalIP;
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
                return externalIP;
            }
            catch 
            {
                throw new Exception("IP retrieval failed");
            }
        }
    }
}
