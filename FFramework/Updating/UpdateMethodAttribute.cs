﻿/*
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
using System.Reflection;

namespace FFramework.Updating
{
    public class UpdateMethodAttribute : Attribute
    {
        public TimeSpan Interval { get; }

        public UpdateMethodAttribute(int intervalSec)
        {
            Interval = TimeSpan.FromSeconds(intervalSec);
        }

        public static UpdateMethodAttribute GetAttribute(MethodInfo m)
        {
            UpdateMethodAttribute att = m.GetCustomAttribute<UpdateMethodAttribute>();
            return att;
        }
    }
}