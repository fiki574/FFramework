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
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace FFramework_Core.Timing
{
	public class EventThread
	{
		public static int UpdateTime = 20;

		private Timer _Time;

		private volatile Dictionary<EventableObject, List<EventInfo>> _Events = new Dictionary<EventableObject, List<EventInfo>>(4095);

		private volatile List<EventInfo> _Insert = new List<EventInfo>(4095);

		private volatile List<EventInfo> _Remove = new List<EventInfo>(4095);

		public readonly int ThreadId;

		public bool IsBusy;

		private Stopwatch _Watch;

		private long Latency;

		private long Global;

		private int Counter;

		public long GetLatence => Latency;

		public EventThread(int id)
		{
			ThreadId = id;
            _Time = new Timer
            {
                Enabled = false
            };
            _Time.Elapsed += Update;
			_Time.Interval = UpdateTime;
			_Watch = new Stopwatch();
			_Time.Start();
		}

		private void CheckSleep()
		{
			long num = _Events.Count + _Insert.Count;
			if (num > 0 && !_Time.Enabled)
			{
				_Time.Enabled = true;
			}
			else if (num <= 0 && _Time.Enabled)
			{
				_Time.Enabled = false;
			}
		}

		public bool IsOk()
		{
			if (IsBusy)
			{
				return false;
			}
			if (Latency >= EventManager.MaxLatency)
			{
				return false;
			}
			if (_Insert.Count + _Events.Count > 4095)
			{
				return false;
			}
			return true;
		}

		private void Update(object obj, EventArgs Arg)
		{
			_Watch.Reset();
			_Watch.Start();
			try
			{
				lock (_Events)
				{
					lock (_Insert)
					{
						if (_Insert.Count > 0)
						{
							foreach (EventInfo item in _Insert)
							{
								item.Reset();
								if (_Events.ContainsKey(item._Obj))
								{
									_Events[item._Obj].Add(item);
								}
								else
								{
                                    List<EventInfo> list = new List<EventInfo>(1)
                                    {
                                        item
                                    };
                                    _Events.Add(item._Obj, list);
								}
							}
							_Insert.Clear();
						}
					}
					lock (_Remove)
					{
						if (_Remove.Count > 0)
						{
							foreach (EventInfo item2 in _Remove)
							{
								if (item2._Obj != null && _Events.ContainsKey(item2._Obj))
								{
									List<EventInfo> list2 = _Events[item2._Obj];
									if (item2._Method == null)
									{
										foreach (EventInfo item3 in list2)
										{
											item3.Stop();
										}
										list2.Clear();
									}
									else
									{
										List<EventInfo> list3 = new List<EventInfo>(list2.Count);
										foreach (EventInfo item4 in list2)
										{
											if (item4._Method == item2._Method)
											{
												list3.Add(item4);
											}
										}
										foreach (EventInfo item5 in list3)
										{
											list2.Remove(item5);
											item5.Stop();
										}
									}
								}
							}
							_Remove.Clear();
						}
					}
					if (_Events.Count > 0)
					{
						if (IsBusy)
						{
							_Watch.Stop();
							List<EventableObject> list4 = new List<EventableObject>();
							foreach (KeyValuePair<EventableObject, List<EventInfo>> @event in _Events)
							{
								if (@event.Value.Count <= 0)
								{
									list4.Add(@event.Key);
								}
							}
							foreach (EventableObject item6 in list4)
							{
								_Events.Remove(item6);
							}
							IsBusy = false;
						}
						foreach (List<EventInfo> value in _Events.Values)
						{
							if (value.Count > 0)
							{
								foreach (EventInfo item7 in value)
								{
									item7.Update();
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex.ToString());
			}
			finally
			{
				_Watch.Stop();
				Counter++;
				Global += _Watch.ElapsedMilliseconds;
				Latency = Global / Counter;
				if (Latency >= EventManager.MaxLatency && !IsBusy && _Events.Count > 1)
				{
					Busy();
				}
				if (Counter >= 100)
				{
					Counter = 0;
					Global = 0L;
					CheckSleep();
				}
			}
		}

		private EventInfo GetEventFromList(List<EventInfo> Le, EventInfo Comp)
		{
			foreach (EventInfo item in Le)
			{
				if (item == Comp || (item._Obj == Comp._Obj && item._Method == Comp._Method))
				{
					return item;
				}
			}
			return null;
		}

		public void AddEvent(EventInfo Event)
		{
			lock (_Insert)
			{
				Event.Thread = this;
				_Insert.Add(Event);
			}
			CheckSleep();
		}

		public void RemoveEvent(EventInfo Info)
		{
			lock (_Remove)
			{
				_Remove.Add(Info);
			}
		}

		public void Busy()
		{
			IsBusy = true;
			List<EventInfo> list = new List<EventInfo>();
			lock (_Events)
			{
				long num = 0L;
				foreach (List<EventInfo> value in _Events.Values)
				{
					num += value.Count;
				}
				if (num > 0)
				{
					num /= 2;
					foreach (List<EventInfo> value2 in _Events.Values)
					{
						foreach (EventInfo item in value2)
						{
							list.Add(item);
							num--;
							if (num <= 0)
							{
								break;
							}
						}
						foreach (EventInfo item2 in list)
						{
							value2.Remove(item2);
						}
						if (num <= 0)
						{
							break;
						}
					}
					EventManager.ThreadBusy(this, list);
					Latency = 0L;
				}
			}
		}
	}
}