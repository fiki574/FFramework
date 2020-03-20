/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2019/2020 Bruno Fištrek

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

using System.Reflection;

namespace FFramework.Timing
{
	public abstract class EventableObject
	{
		public EventableObject()
		{
			MethodInfo[] methods = GetType().GetMethods();
			foreach (MethodInfo methodInfo in methods)
			{
				TimedMethod[] array = (TimedMethod[])methodInfo.GetCustomAttributes(typeof(TimedMethod), inherit: true);
				if (array.Length > 0)
				{
					EventManager.AddEvent(this, methodInfo, array[0].Time, array[0].Count, null);
				}
			}
		}
	}
}