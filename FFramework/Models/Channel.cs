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
    
    Credits: https://github.com/usertoroot
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace FFramework.Models
{
    public class Channel
    {
        public uint Identifier
        {
            get;
            private set;
        }

        public string IdentifierString
        {
            get;
            private set;
        }

        public List<object> Values
        {
            get;
            private set;
        }

        public Channel(uint identifier)
        {
            Identifier = identifier;
            var bytes = BitConverter.GetBytes(Identifier);
            var rotatedBytes = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            IdentifierString = Encoding.ASCII.GetString(rotatedBytes);
            Values = new List<object>();
        }

        public override string ToString()
        {
            return IdentifierString;
        }
    }
}