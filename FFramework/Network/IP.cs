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
*/

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using NetFwTypeLib;

namespace FFramework.Network
{
    public static class IP
    {
        public static string GetPublicIP()
        {
            try
            {
                return (new WebClient()).DownloadString("http://bot.whatismyipaddress.com/");
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        public static string GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();

            return "127.0.0.1";
        }

        public static void CreateInboundFirewallRule(int port, string name, int protocol)
        {
            Type tNetFwPolicy = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 fwPolicy = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy);
            INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            inboundRule.Enabled = true;
            inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            inboundRule.Protocol = protocol;
            inboundRule.LocalPorts = port.ToString();
            inboundRule.Name = name;
            inboundRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;

            bool add = true;
            foreach (INetFwRule rule in fwPolicy.Rules)
                if (rule.Name == name)
                {
                    add = false;
                    break;
                }

            if (add)
                fwPolicy.Rules.Add(inboundRule);
        }

        public static void ForwardPort(int port, string protocol, string ip, string description)
        {
            NATUPNPLib.UPnPNATClass upnpnat = new NATUPNPLib.UPnPNATClass();
            NATUPNPLib.IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
            bool add = true;
            foreach (NATUPNPLib.IStaticPortMapping m in mappings)
                if (m.Description == description)
                {
                    add = false;
                    break;
                }

            if (add)
                mappings.Add(port, protocol, port, ip, true, description);
        }

        private static Random random = new Random();
        public static string GenerateApiKey(int length)
        {
            return new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}