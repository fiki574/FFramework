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

namespace FFramework.Updating
{
    public class Updatable
    {
        private Action update;
        public TimeSpan Interval { get; }
        public DateTime LastUpdate { get; private set; }
        public string Name { get; }

        public Updatable(string name, TimeSpan interval, Action update)
        {
            Name = name;
            Interval = interval;
            this.update = update;
            LastUpdate = DateTime.Now;
        }

        public void Update()
        {
            DateTime nextUpdate = LastUpdate.Add(Interval);
            if (nextUpdate < DateTime.Now)
            {
                LastUpdate = DateTime.Now;
                try
                {
                    update();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}