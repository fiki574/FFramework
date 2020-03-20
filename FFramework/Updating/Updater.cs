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
    
    Credits: https://github.com/usertoroot
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FFramework.Updating
{
    public partial class Updater
    {
        private Thread thread;
        private List<Updatable> updates = new List<Updatable>();
        public bool Active { get; private set; }

        public Updater()
        {
            foreach (MethodInfo info in typeof(Updater).GetMethods())
            {
                UpdateMethodAttribute attr = UpdateMethodAttribute.GetAttribute(info);
                if (attr != null)
                    updates.Add(new Updatable(info.Name, attr.Interval, () => info.Invoke(this, new object[0])));
            }
        }

        public void Run()
        {
            Active = true;
            thread = new Thread(() =>
            {
                try
                {
                    while (Active)
                    {
                        Parallel.ForEach(updates, (u) => { u.Update(); });
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            });
            thread.Start();
        }

        public void Stop()
        {
            Active = false;
        }
    }
}