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
    
    Credits: https://github.com/usertoroot
*/

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace FFramework.Patch
{
    public class Patches
    {
        private Dictionary<IntPtr, object> _patches = new Dictionary<IntPtr, object>();
        private Process _process = null;

        public Patches(Process p)
        {
            _process = p;
        }

        public void AddPatch(IntPtr address, object data)
        {
            _patches.Add(address, data);
        }

        public void ApplyPatches()
        {
            if (_patches.Count == 0) return;
            try
            {
                IntPtr handle = Kernel32.OpenProcess(_process, ProcessAccessFlags.All);
                foreach (var patch in _patches)
                {
                    if (GetDataType(patch.Value) == typeof(byte[]))
                    {
                        byte[] data = (byte[])patch.Value;
                        uint oldProtect = Kernel32.VirtualProtectEx(handle, patch.Key, data.Length, 0x40);
                        Kernel32.WriteProcessMemory(handle, patch.Key, data);
                        Kernel32.VirtualProtectEx(handle, patch.Key, data.Length, oldProtect);
                    }
                    else if (GetDataType(patch.Value) == typeof(string))
                    {
                        string data = (string)patch.Value;
                        uint oldProtect = Kernel32.VirtualProtectEx(handle, patch.Key, data.Length, 0x40);
                        Kernel32.WriteString(handle, patch.Key, data);
                        Kernel32.VirtualProtectEx(handle, patch.Key, data.Length, oldProtect);
                    }
                    else return;
                }
            }
            catch
            {
                return;
            }
        }

        public Type GetDataType(object data)
        {
            Type t = data.GetType();
            if (t == typeof(string)) return typeof(string);
            else if (t == typeof(byte[])) return typeof(byte[]);
            else return null;
        }
    }
}
