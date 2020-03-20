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

using System.Collections.Generic;
using System.Reflection;

namespace FFramework.Timing
{
	public abstract class TimeManager
	{
		public delegate void Function();

		public List<TimeInfo> _Events = new List<TimeInfo>();

		public Dictionary<string, MethodInfo> _Methods = new Dictionary<string, MethodInfo>();

		public TimeManager()
		{
			MethodInfo[] methods = GetType().GetMethods();
			foreach (MethodInfo methodInfo in methods)
			{
				TimedMethod[] array = (TimedMethod[])methodInfo.GetCustomAttributes(typeof(TimedMethod), inherit: true);
				if (array.Length > 0)
				{
					AddNewTimer(array[0], methodInfo);
				}
			}
		}

		public void StopTimers()
		{
			foreach (TimeInfo @event in _Events)
			{
				@event._Obj = null;
				@event._Count = 0;
			}
			_Events.Clear();
		}

		private void AddNewTimer(TimedMethod Methode, MethodInfo Info)
		{
			if (!_Methods.ContainsKey(Info.Name))
			{
				_Methods.Add(Info.Name, Info);
			}
			_Events.Add(new TimeInfo(this, Methode, Info));
		}

		public void StartTimer(Function Func, int time, int count)
		{
            TimedMethod timedMethod = new TimedMethod
            {
                Count = count,
                Time = time
            };
            AddNewTimer(timedMethod, Func.Method);
		}
	}
}