/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2016 Bruno Fištrek

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

using System.Security.Principal;
using System.Diagnostics;
using System.ComponentModel;

namespace FFramework.Utilities
{
    public class RunAsAdmin
    {
        public static void RunWithAdminRights(string executable_path)
        {
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (!pricipal.IsInRole(WindowsBuiltInRole.Administrator)) RunElevated(executable_path);
        }

        private static bool RunElevated(string fileName)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Verb = "runas";
            processInfo.FileName = fileName;
            try
            {
                Process.Start(processInfo);
                return true;
            }
            catch (Win32Exception)
            {
                return false;
            }
        }
    }
}
