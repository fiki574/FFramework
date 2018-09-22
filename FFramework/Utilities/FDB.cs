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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using FFramework.Extensions;

namespace FFramework.Utilities
{
    public struct FDBFile
    {
        public int ID;
        public byte[] Data;
    }

    public class FDB
    {
        private byte[] Signature = new byte[] { 0x0F, 0x0D, 0xB, 0x01, 0x00 };
        private List<FDBFile> FDBFiles;
        private string Name;

        public FDB(string name)
        {
            try
            {
                Name = name + ".fdb";
                FDBFiles = new List<FDBFile>();
                if (!System.IO.File.Exists(Name))
                    Initial();

                Load();
            }
            catch
            {
                return;
            }
        }

        private void Load()
        {
            try
            {
                var decompressed = Decompress(System.IO.File.ReadAllBytes(Name));
                using (var reader = new BinaryReader(new MemoryStream(decompressed)))
                {
                    var header = new byte[Signature.Length];
                    for (int i = 0; i < Signature.Length; i++)
                        if (reader.ReadByte() != Signature[i])
                            return;

                    var count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                        FDBFiles.Add((FDBFile)reader.ReadStructure(typeof(FDBFile)));
                }
            }
            catch
            {
                return;
            }
        }

        public void CommitChanges()
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(Signature);
                        writer.Write(FDBFiles.Count);
                        FDBFiles.ForEach(f => writer.WriteStructure(f));
                    }

                    stream.Flush();
                    var compressed = Compress(stream.GetBuffer());
                    System.IO.File.WriteAllBytes(Name, compressed);
                }
            }
            catch
            {
                return;
            }
        }

        public void Write<T>(List<T> entries, ref FDBFile file)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(ms))
                    {
                        writer.Write(entries.Count);
                        writer.Write(Marshal.SizeOf(typeof(T)));
                        foreach (T t in entries) writer.WriteStructure(t);
                    }

                    var data = ms.ToArray();
                    file.Data = new byte[data.Length];
                    Buffer.BlockCopy(data, 0, file.Data, 0, data.Length);
                }
                UpdateFile(ref file);
            }
            catch
            {
                return;
            }
        }

        public List<T> Read<T>(ref FDBFile File)
        {
            try
            {
                using (var reader = new BinaryReader(new MemoryStream(File.Data)))
                {
                    var entries = reader.ReadInt32();
                    var size = reader.ReadInt32();
                    var cursize = Marshal.SizeOf(typeof(T));
                    if (size < cursize || size > cursize)
                        throw new Exception($"Size of structure in database: {size} | Current structure size: {cursize} | Sizes must be the same!");

                    var list = new List<T>(entries);
                    for (int i = 0; i < entries; i++)
                        list.Add((T)reader.ReadStructure(typeof(T)));

                    reader.Close();
                    return list;
                }
            }
            catch 
            {
                return null;
            }
        }

        public void AddFile(byte[] data, int index = -1)
        {
            try
            {
                int id = -1;
                if (index != -1)
                {
                    if (FDBFiles.Count(f => f.ID == index) > 0)
                        return;
                    else
                        id = index;
                }
                else
                    id = GetNextID();

                var file = new FDBFile();
                file.ID = id;
                file.Data = new byte[data.Length];
                Array.Copy(data, file.Data, data.Length);
                FDBFiles.Add(file);
            }
            catch
            {
                return;
            }
        }

        public void AddFile(string data, int index = -1)
        {
            AddFile(System.Text.Encoding.ASCII.GetBytes(data), index);
        }

        public void RemoveFile(ref FDBFile file)
        {
            FDBFiles.Remove(file);
        }

        public void RemoveFile(int index)
        {
            FDBFiles.RemoveAt(index);
        }

        public int GetNextID()
        {
            var highest = 1;
            foreach (FDBFile file in FDBFiles)
                if (file.ID > highest)
                    highest = file.ID;

            return highest + 1;
        }

        public List<FDBFile> GetFiles()
        {
            return FDBFiles;
        }

        public int GetFileIndex(int ID)
        {
            return FDBFiles.IndexOf(FDBFiles.Find(f => f.ID == ID));
        }

        private void UpdateFile(ref FDBFile file)
        {
            FDBFiles[GetFileIndex(file.ID)] = file;
        }

        private FDBFile GetFileFromID(int id)
        {
            foreach (var file in FDBFiles)
                if (file.ID == id)
                    return file;

            return default(FDBFile);
        }

        private void Initial()
        {
            try
            {
                System.IO.File.WriteAllText("0", "This is generic/example file that is used in FDB. FDB is in-memory file based database system. FDB uses compression and specific file structures to ensure best possible memory space saving and performance. Files that are used to store data have no extension and are acting like IDs starting from 1.");
                var file = new FDBFile();
                file.ID = 0;
                file.Data = System.IO.File.ReadAllBytes("0");
                FDBFiles.Add(file);
                System.IO.File.Delete("0");
                CommitChanges();
            }
            catch
            {
                return;
            }
        }

        private byte[] Read(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private byte[] Compress(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var dstream = new DeflateStream(output, CompressionLevel.Optimal))
                    dstream.Write(data, 0, data.Length);

                return output.ToArray();
            }
        }

        private byte[] Decompress(byte[] data)
        {
            using (MemoryStream input = new MemoryStream(data))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                        dstream.CopyTo(output);

                    return output.ToArray();
                }
            }
        }

        public override string ToString()
        {
            return $"[Name = {Name} | Entries = {FDBFiles.Count}]";
        }
    }
}