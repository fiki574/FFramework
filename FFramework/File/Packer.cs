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

using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFramework.File
{
    public class Packer
    {
        public static void Pack(string dir, string extension, uint header = 0x14B4150)
        {
            string file = dir + extension;

            int offset = 16;

            using (var ws = System.IO.File.OpenWrite(file))
            using (BinaryWriter fileWriter = new BinaryWriter(ws))
            {
                fileWriter.Write((uint)header);
                ws.Position = 16;

                byte[] compressed;

                using (var s = new MemoryStream())
                using (BinaryWriter w = new BinaryWriter(s))
                {
                    string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                    w.Write((int)files.Length);

                    foreach (var filePath in files)
                    {
                        string fileName = filePath.Substring(dir.Length + 1, filePath.Length - dir.Length - 1);
                        w.Write((int)fileName.Length);
                        w.Write(Encoding.ASCII.GetBytes(fileName));
                        w.Write((int)offset);
                        int size = (int)new FileInfo(filePath).Length;
                        w.Write((int)(offset + size));
                        offset += size;
                        fileWriter.Write(System.IO.File.ReadAllBytes(filePath));
                    }

                    ws.Position = 4;
                    fileWriter.Write((int)offset);
                    fileWriter.Write((int)s.Position);
                    byte[] decompressed = s.ToArray();
                    compressed = Compress(decompressed);
                }

                fileWriter.Write(compressed.Length);
                ws.Position = offset;
                fileWriter.Write(compressed, 0, compressed.Length);
            }
        }

        private static byte[] Compress(byte[] data)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var compressor = new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.Default)) compressor.Write(data, 0, data.Length);
                return ms.ToArray();
            }
        }

        public static void Unpack(string file, uint header = 0x14B4150)
        {
            string dir = Path.GetFileNameWithoutExtension(file);

            using (Stream s = System.IO.File.OpenRead(file))
            using (BinaryReader reader = new BinaryReader(s))
            {
                if (reader.ReadUInt32() != header)
                {
                    throw new Exception("Unexpected header");
                }

                int offset = reader.ReadInt32();
                int size = reader.ReadInt32();
                int compressedSize = reader.ReadInt32();

                s.Position = offset;
                byte[] compressed = reader.ReadBytes(compressedSize);
                byte[] decompressed = new byte[size];

                using (var cs = new MemoryStream(compressed))
                using (var zs = new ZlibStream(cs, CompressionMode.Decompress)) zs.Read(decompressed, 0, size);

                using (MemoryStream ds = new MemoryStream(decompressed))
                using (BinaryReader drs = new BinaryReader(ds))
                {
                    int files = drs.ReadInt32();
                    for (int i = 0; i < files; i++)
                    {
                        int nameLength = drs.ReadInt32();
                        string name = Encoding.ASCII.GetString(drs.ReadBytes(nameLength));
                        int fileOffset = drs.ReadInt32();
                        int fileSize = drs.ReadInt32() - fileOffset;
                        s.Position = fileOffset;
                        string path = Path.Combine(dir, name);
                        CreatePath(path);
                        System.IO.File.WriteAllBytes(path, reader.ReadBytes(fileSize));
                    }
                }
            }
        }

        private static void CreatePath(string path)
        {
            string fileName = Path.GetFileName(path);
            path = path.Substring(0, path.Length - fileName.Length);
            CreatePath(path.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static void CreatePath(string[] components)
        {
            string a = "";
            for (int i = 0; i < components.Length; i++)
            {
                a = Path.Combine(a, components[i]);
                if (!Directory.Exists(a)) Directory.CreateDirectory(a);
            }
        }
    }
}
