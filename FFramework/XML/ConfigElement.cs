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

namespace FFramework.XML
{
	public class ConfigElement
	{
		private readonly Dictionary<string, ConfigElement> _children = new Dictionary<string, ConfigElement>();

		private readonly ConfigElement _parent;

		private string _value;

		public ConfigElement this[string key]
		{
			get
			{
				lock (_children)
				{
					if (!_children.ContainsKey(key))
					{
						_children.Add(key, GetNewConfigElement(this));
					}
				}
				return _children[key];
			}
			set
			{
				lock (_children)
				{
					_children[key] = value;
				}
			}
		}

		public ConfigElement Parent => _parent;

		public bool HasChildren => _children.Count > 0;

		public Dictionary<string, ConfigElement> Children => _children;

		public ConfigElement(ConfigElement parent)
		{
			_parent = parent;
		}

		private static ConfigElement GetNewConfigElement(ConfigElement parent)
		{
			return new ConfigElement(parent);
		}

		public string GetString()
		{
			return _value ?? "";
		}

		public string GetString(string defaultValue)
		{
			return _value ?? defaultValue;
		}

		public int GetInt()
		{
			return int.Parse(_value ?? "0");
		}

		public int GetInt(int defaultValue)
		{
			if (_value == null)
			{
				return defaultValue;
			}
			return int.Parse(_value);
		}

		public long GetLong()
		{
			return long.Parse(_value ?? "0");
		}

		public long GetLong(long defaultValue)
		{
			if (_value == null)
			{
				return defaultValue;
			}
			return long.Parse(_value);
		}

		public bool GetBoolean()
		{
			return bool.Parse(_value ?? "false");
		}

		public bool GetBoolean(bool defaultValue)
		{
			if (_value == null)
			{
				return defaultValue;
			}
			return bool.Parse(_value);
		}

		public void Set(object value)
		{
			if (value == null)
			{
				value = "";
			}
			_value = value.ToString();
		}
	}
}