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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FFramework.Patch
{
    public class Patches
    {
        private Dictionary<IntPtr, byte[]> _patches = new Dictionary<IntPtr, byte[]>();
        private Process _process = null;

        public Patches(Process p)
        {
            _process = p;
        }

        ///<summary>Adds a memory patch with given byte data to patch list that will be applied only after using "ApplyPatches()" function</summary>
        ///<param name="address">The address you want to patch</param>
        ///<param name="data">Data you want to insert/patch 'into' the address</param>
        ///<returns>(nothing)</returns>
        public void AddPatch(IntPtr address, byte[] data)
        {
            _patches.Add(address, data);
        }

        ///<summary>Applies all patches from the patch list that were added with the function "AddPatch()"</summary>
        ///<param name="force_exit_if_patch_fails">If true, the selected process will be shut down if the patch(es) fail(s), by default this is set to false</param>
        ///<returns>(nothing)</returns>
        public void ApplyPatches(bool force_exit_if_patch_fails = false)
        {
            _process.WaitForInputIdle();
            try
            {
                IntPtr handle = Kernel32.OpenProcess(_process, ProcessAccessFlags.All);
                foreach (var patch in _patches)
                {
                    uint oldProtect = Kernel32.VirtualProtectEx(handle, patch.Key, patch.Value.Length, 0x40);
                    Kernel32.WriteProcessMemory(handle, patch.Key, patch.Value);
                    Kernel32.VirtualProtectEx(handle, patch.Key, patch.Value.Length, oldProtect);
                }  
            }
            catch (Exception)
            {                
                if(force_exit_if_patch_fails) _process.CloseMainWindow();
            }
        }     
    }
}
