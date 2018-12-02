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
using System.Text;

namespace FFramework.IO.Extensions
{
    public static class BinaryReaderExtension
    {
        public static object ReadStructure(this BinaryReader reader, Type t)
        {
            object o = Activator.CreateInstance(t);
            if (t.IsValueType && !t.IsEnum)
            {
                if (t == typeof(byte))
                    return reader.ReadByte();
                else if (t == typeof(short))
                    return reader.ReadInt16();
                else if (t == typeof(int))
                    return reader.ReadInt32();
                else if (t == typeof(uint))
                    return reader.ReadInt64();
                else
                {
                    foreach (var f in t.GetFields())
                    {
                        if (f.FieldType.IsCustomValueType())
                            f.SetValue(o, reader.ReadStructure(f.FieldType));
                        else if (f.FieldType.IsArray)
                        {
                            int length = reader.ReadInt32();
                            Array a = Array.CreateInstance(f.FieldType.GetElementType(), length);
                            for (int i = 0; i < length; i++)
                                a.SetValue(reader.ReadStructure(f.FieldType.GetElementType()), i);
                            f.SetValue(o, a);
                        }
                        else
                            f.SetValue(o, reader.ReadStructureValue(f.FieldType));
                    }
                    return o;
                }
            }
            else
                return reader.ReadStructureValue(t);
        }

        public static object ReadStructureValue(this BinaryReader reader, Type t)
        {
            if (t == typeof(string))
            {
                int length = reader.ReadUInt16();
                byte[] textBytes = reader.ReadBytes(length);
                return Encoding.UTF8.GetString(textBytes);
            }
            else if (t == typeof(byte))
                return reader.ReadByte();
            else if (t == typeof(bool))
                return reader.ReadBoolean();
            else if (t == typeof(short))
                return reader.ReadInt16();
            else if (t == typeof(int))
                return reader.ReadInt32();
            else if (t == typeof(long))
                return reader.ReadInt64();
            else
                return null;
        }

        public static T ReadStructure<T>(this BinaryReader reader) where T : struct
        {
            Type t = typeof(T);
            object o = reader.ReadStructure(t);
            return (T)o;
        }

        public static T[] ReadStructures<T>(this BinaryReader reader) where T : struct
        {
            Type t = typeof(T);
            int length = reader.ReadInt32();
            T[] ts = new T[length];
            for (int i = 0; i < length; i++)
                ts[i] = (T)reader.ReadStructure(t);
            return ts;
        }
    }
}