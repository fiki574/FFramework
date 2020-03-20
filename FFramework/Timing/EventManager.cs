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
	public static class EventManager
	{
		public static long MaxLatency = 100L;

		private static List<EventThread> _Threads = new List<EventThread>();

		public static EventThread GetOrCreateThread(bool force)
		{
			EventThread eventThread = null;
			lock (_Threads)
			{
				_ = MaxLatency;
				if (!force)
				{
					foreach (EventThread thread in _Threads)
					{
						if (thread.IsOk())
						{
							eventThread = thread;
							_ = eventThread.GetLatence;
						}
					}
				}
				if (eventThread != null)
				{
					return eventThread;
				}
				eventThread = new EventThread(_Threads.Count + 1);
				_Threads.Add(eventThread);
				return eventThread;
			}
		}

		public static void AddEvent(EventableObject Obj, string MetodeName, long Time, int Count, object[] Args)
		{
			MethodInfo method = Obj.GetType().GetMethod(MetodeName);
			if (method != null)
			{
				AddEvent(Obj, method, Time, Count, Args);
			}
		}

		public static void AddEvent(EventableObject Obj, MethodInfo Info, long Time, int Count, object[] Args)
		{
			EventThread orCreateThread = GetOrCreateThread(force: false);
			EventInfo @event = new EventInfo(Obj, Info, Time, Count, Args);
			orCreateThread.AddEvent(@event);
		}

		public static void RemoveEvent(EventableObject Obj)
		{
			lock (_Threads)
			{
				foreach (EventThread thread in _Threads)
				{
					thread.RemoveEvent(new EventInfo(Obj, null, 0L, 0, null));
				}
			}
		}

		public static void RemoveEvent(EventableObject Obj, MethodInfo Info)
		{
			lock (_Threads)
			{
				foreach (EventThread thread in _Threads)
				{
					thread.RemoveEvent(new EventInfo(Obj, Info, 0L, 0, null));
				}
			}
		}

		public static void RemoveEvent(EventInfo Info)
		{
			if (Info.Thread != null)
			{
				Info.Thread.RemoveEvent(Info);
			}
			else
			{
				lock (_Threads)
				{
					foreach (EventThread thread in _Threads)
					{
						thread.RemoveEvent(Info);
					}
				}
			}
		}

		public static void ThreadBusy(EventThread Thread, List<EventInfo> Dispacth)
		{
			EventThread orCreateThread = GetOrCreateThread(force: true);
			foreach (EventInfo item in Dispacth)
			{
				orCreateThread.AddEvent(item);
			}
		}
	}
}