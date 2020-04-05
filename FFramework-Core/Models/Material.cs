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

using System.Collections.Generic;
using System.Linq;

namespace FFramework_Core.Models
{
    public class Material
    {
        public List<Channel> Channels = new List<Channel>();
        public List<object> Parameters = new List<object>();

        public Material()
        {
        }

        public void AddParameter(object o)
        {
            if (o.GetType() == typeof(TextureParameter))
            {
                var value = (TextureParameter)o;

                Channel channel = Channels.SingleOrDefault(c => c.Identifier == value.Identifier);
                if (channel == null)
                {
                    channel = new Channel(value.Identifier);
                    Channels.Add(channel);
                }

                channel.Values.Add(value.Value);
            }
            else if (o.GetType() == typeof(ColorParameter))
            {
                var value = (ColorParameter)o;

                Channel channel = Channels.SingleOrDefault(c => c.Identifier == value.Identifier);
                if (channel == null)
                {
                    channel = new Channel(value.Identifier);
                    Channels.Add(channel);
                }

                channel.Values.Add(value.Value);
            }
            else if (o.GetType() == typeof(ScalarParameter))
            {
                var value = (ScalarParameter)o;

                Channel channel = Channels.SingleOrDefault(c => c.Identifier == value.Identifier);
                if (channel == null)
                {
                    channel = new Channel(value.Identifier);
                    Channels.Add(channel);
                }

                channel.Values.Add(value.Value);
            }
            else if (o.GetType() == typeof(int))
            {
                var value = (int)o;
                Parameters.Add(value);
            }
            else if (o.GetType() == typeof(string))
            {
                var value = (string)o;
                Parameters.Add(value);
            }
            else if (o.GetType() == typeof(VectorParameter))
            {
                var value = (VectorParameter)o;

                Channel channel = Channels.SingleOrDefault(c => c.Identifier == value.Identifier);
                if (channel == null)
                {
                    channel = new Channel(value.Identifier);
                    Channels.Add(channel);
                }

                channel.Values.Add(value.Value);
            }
        }
    }
}