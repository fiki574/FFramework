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

using System;
using System.Diagnostics;
using System.Reflection;

namespace FFramework_Core.Timing
{
	public class EventInfo
	{
		public EventThread Thread;

		public EventableObject _Obj;

		private object[] _Args;

		public MethodInfo _Method;

		private readonly long _BaseTime;

		private readonly int _BaseCount;

		private int _Count;

		private long _Time;

		private bool _Done;

		private Stopwatch _Watch;

		public EventInfo(EventableObject Obj, MethodInfo Method, long Time, int Count, object[] Params)
		{
			_Obj = Obj;
			_Method = Method;
			_BaseCount = Count;
			_BaseTime = Time;
			_Args = Params;
			_Count = _BaseCount;
			if (Method != null)
			{
				_Watch = new Stopwatch();
			}
		}

		public void Reset()
		{
			_Time = _BaseTime;
			_Watch.Reset();
			_Watch.Start();
		}

		public void Stop()
		{
			_Done = true;
			_Obj = null;
			_Args = null;
			_Count = 0;
			_Time = 0L;
			if (_Watch != null)
			{
				_Watch.Stop();
			}
		}

		public void Update()
		{
			if (!_Done)
			{
				long elapsedMilliseconds = _Watch.ElapsedMilliseconds;
				if (elapsedMilliseconds >= _Time)
				{
					Execute();
				}
			}
		}

		public void Execute()
		{
			if (!_Done)
			{
				try
				{
					_Method.Invoke(_Obj, _Args);
				}
				catch (Exception ex)
				{
                    Console.WriteLine(ex.ToString());
					EventManager.RemoveEvent(this);
				}
				Reset();
				if (_BaseCount > 0)
				{
					_Count--;
				}
				if ((_BaseCount > 0 && _Count <= 0) || _Obj == null || _BaseTime == 0)
				{
					EventManager.RemoveEvent(this);
				}
			}
		}
	}
}