/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2017 Bruno Fištrek
    
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFramework.Extensions
{
    public static class BinaryWriterExtension
    {
        public static void WriteStructures<T>(this BinaryWriter writer, T[] value)
        {
            writer.Write(value.Length);
            for (int i = 0; i < value.Length; i++) writer.WriteStructure(value[i]);
        }

        public static void WriteStructures<T>(this BinaryWriter writer, List<T> value)
        {
            writer.Write(value.Count);
            for (int i = 0; i < value.Count; i++) writer.WriteStructure(value[i]);
        }

        public static void WriteStructure(this BinaryWriter writer, object value)
        {
            Type t = value.GetType();
            if (t.IsCustomValueType())
            {
                foreach (var f in t.GetFields())
                {
                    if (f.FieldType.IsCustomValueType()) writer.WriteStructure(f.GetValue(value));
                    else if (f.FieldType.IsArray)
                    {
                        Array a = (Array)f.GetValue(value);
                        if (a == null || a.Length <= 0) writer.Write(0);
                        else
                        {
                            writer.Write(a.Length);
                            for (int i = 0; i < a.Length; i++) writer.WriteStructure(a.GetValue(i));
                        }
                    }
                    else if (f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        IList genericList = (IList)f.GetValue(value);
                        if (genericList == null || genericList.Count <= 0) writer.Write(0);
                        else
                        {
                            writer.Write(genericList.Count);
                            for (int i = 0; i < genericList.Count; i++) writer.WriteStructure(genericList[i]);
                        }
                    }
                    else writer.WriteStructureValue(f.GetValue(value), f.FieldType);
                }
            }
            else writer.WriteStructureValue(value, t);
        }

        public static void WriteStructureValue(this BinaryWriter writer, object value, Type t)
        {
            if (t == typeof(string))
            {
                string s = (string)value;
                writer.Write((ushort)s.Length);
                writer.Write(Encoding.UTF8.GetBytes(s));
            }
            else if (t == typeof(bool)) writer.Write((bool)value);
            else if (t == typeof(sbyte)) writer.Write((sbyte)value);
            else if (t == typeof(byte)) writer.Write((byte)value);
            else if (t == typeof(short)) writer.Write((short)value);
            else if (t == typeof(ushort)) writer.Write((ushort)value);
            else if (t == typeof(int)) writer.Write((int)value);
            else if (t == typeof(uint)) writer.Write((uint)value);
            else if (t == typeof(long)) writer.Write((long)value);
            else if (t == typeof(ulong)) writer.Write((ulong)value);
            else throw new Exception("Unknown type in structure.");
        }
    }
}