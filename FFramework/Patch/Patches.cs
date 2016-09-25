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
using System.Collections.Generic;
using System.Diagnostics;

namespace FFramework.Patch
{
    public struct Patch
    {
        public IntPtr Address;
        public string StringData;
        public byte[] ByteData;
    }

    public class Patches
    {
        private List<Patch> _patches = new List<Patch>();
        private Process _process = null;

        public Patches(Process p)
        {
            _process = p;
        }

        public void AddPatch(IntPtr address, object data)
        {
            Type t = data.GetType();
            if (t == typeof(string)) _patches.Add(new Patch() { Address = address, StringData = (string)data, ByteData = null });
            else if (t == typeof(byte[])) _patches.Add(new Patch() { Address = address, StringData = null, ByteData = (byte[])data });
            else return;
        }

        public void ApplyPatches()
        {
            if (_patches.Count == 0) return;
            try
            {
                IntPtr handle = Kernel32.OpenProcess(_process, ProcessAccessFlags.All);
                foreach (var patch in _patches)
                {
                    if (GetPatchType(patch) == typeof(byte[]))
                    {
                        uint oldProtect = Kernel32.VirtualProtectEx(handle, patch.Address, patch.ByteData.Length, 0x40);
                        Kernel32.WriteProcessMemory(handle, patch.Address, patch.ByteData);
                        Kernel32.VirtualProtectEx(handle, patch.Address, patch.ByteData.Length, oldProtect);
                    }
                    else if (GetPatchType(patch) == typeof(string))
                    {
                        uint oldProtect = Kernel32.VirtualProtectEx(handle, patch.Address, patch.StringData.Length, 0x40);
                        Kernel32.WriteString(handle, patch.Address, patch.StringData);
                        Kernel32.VirtualProtectEx(handle, patch.Address, patch.StringData.Length, oldProtect);
                    }
                    else return;
                }
            }
            catch
            {
                return;
            }
        }

        public Type GetPatchType(Patch patch)
        {
            if (patch.StringData == null && patch.ByteData != null) return typeof(byte[]);
            else if (patch.StringData != null && patch.ByteData == null) return typeof(string);
            else return null;
        }
    }
}
