/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2018/2019 Bruno Fištrek

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using FFramework.Threading;

namespace FFramework.Utilities
{
    public static class Platform
    {
        public static ThreadSafeList<string> allowed_platforms = new ThreadSafeList<string>();

        public static PlatformID GetCurrentPlatform()
        {
            return Environment.OSVersion.Platform;
        }

        public static string GetCurrentPlatformString()
        {
            return Environment.OSVersion.Platform.ToString();
        }

        public static void AllowPlatform(PlatformID platform_id)
        {
            allowed_platforms.Add(platform_id.ToString());
        }

        public static bool IsPlatformAllowed(PlatformID platform_id)
        {
            return allowed_platforms.Contains(platform_id.ToString());
        }

        public static void DisposePlatformsList()
        {
            allowed_platforms.Remove(p => true);
            allowed_platforms = null;
        }
    }
}
