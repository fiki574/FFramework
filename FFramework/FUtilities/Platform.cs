using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFramework.FUtilities
{
    ///<summary>Class used for platform managment</summary>
    public static class Platform
    {
        public static ThreadSafeList<string> allowed_platforms = new ThreadSafeList<string>();

        ///<summary>Retrieve the current platform ID</summary>
        ///<returns>Current PlatformID</returns>
        public static PlatformID GetCurrentPlatform()
        {
            return Environment.OSVersion.Platform;
        }

        ///<summary>Retrieve the current platform ID as string</summary>
        ///<returns>Current platform as string</returns>
        public static string GetCurrentPlatformString()
        {
            return Environment.OSVersion.Platform.ToString();
        }

        ///<summary>Allow "platform_id" by adding it to allowed platforms' list</summary>
        ///<param name="platform_id">Platform you want to allow</param>
        ///<returns>(nothing)</returns>
        public static void AllowPlatform(PlatformID platform_id)
        {
            allowed_platforms.Add(platform_id.ToString());
        }

        ///<summary>Check if "platform_id" is allowed. NOTE: You have to use "AllowPlatform()" to add desired platforms to the list, otherwise this function will not work and will cause exceptions</summary>
        ///<param name="platform_id">Platform you want to check</param>
        ///<returns>true if it's in the list (allowed), or false if it's not in the list (disallowed)</returns>
        public static bool IsPlatformAllowed(PlatformID platform_id)
        {
            return allowed_platforms.Contains(platform_id.ToString());
        }

        ///<summary>Clear the platform list when you're done with using it, or closing the application in general</summary>
        public static void DisposePlatformsList()
        {
            allowed_platforms.Remove(p => true);
            allowed_platforms = null;
        }
    }
}
