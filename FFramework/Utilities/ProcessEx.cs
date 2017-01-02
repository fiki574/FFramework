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

using System.Diagnostics;

namespace FFramework.Utilities
{
    public class ProcessEx
    {
        public static bool IsProcessActive(string process_name)
        {
            if (Process.GetProcessesByName(process_name).Length == 0) return false;
            else return true;
        }

        public static Process GetProcess(string process_name)
        {
            Process[] pname = Process.GetProcessesByName(process_name);
            if (pname.Length == 0) return null;
            else return pname[0];
        }
    }
}
