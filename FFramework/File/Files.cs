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
using System.IO;

namespace FFramework.File
{
    public static class Files
    {
        public static string CreatePath(string location)
        {
            string[] folders = location.Split('\\');
            string root = folders[0];
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            for (int i = 1; i < folders.Length - 1; i++)
            {
                root += "\\" + folders[i];
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
            }

            if(!System.IO.File.Exists(folders[folders.Length - 1]))
                System.IO.File.Create(folders[folders.Length - 1]);

            return root;
        }

        public static float GetSizeMB(string file)
        { 
            return (float)Math.Round(((new FileInfo(file).Length / 1024f) / 1024f), 3);
        }

        public static float GetSizeKB(string file)
        {
            return (float)Math.Round((new FileInfo(file).Length / 1024f), 3);
        }
    }
}