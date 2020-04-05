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
using System.Reflection;
using System.Timers;

namespace FFramework_Core.Timing
{
	public class TimeInfo
	{
		public TimeManager _Obj;

		public TimedMethod _Method;

		public MethodInfo _Info;

		public int _Count;

		public Timer Time = new Timer();

		public TimeInfo(TimeManager Obj, TimedMethod Method, MethodInfo Info)
		{
			_Obj = Obj;
			_Method = Method;
			_Info = Info;
			_Count = Method.Count;
			if (Method.Time == 0)
			{
				Execute(null, null);
				return;
			}
			Time.Enabled = false;
			Time.Elapsed += Execute;
			Time.Interval = Method.Time;
			Time.Start();
		}

		public void Execute(object obj, EventArgs e)
		{
			try
			{
				if (_Obj != null)
				{
					_Info.Invoke(_Obj, null);
				}
			}
			catch
			{
			}
			finally
			{
				if (_Method.Count > 0)
				{
					_Count--;
				}
				if ((_Method.Count > 0 && _Count <= 0) || _Obj == null || _Method.Time == 0)
				{
					Time.Stop();
					Time = null;
					_Obj = null;
				}
			}
		}
	}
}