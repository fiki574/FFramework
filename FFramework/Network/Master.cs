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
using System.Net;
using System.Threading;
using System.Net.Sockets;
using FFramework.Threading;

namespace FFramework.Network
{
    public class Master
    {
        private TcpListener m_listener;
        private ThreadSafeList<Session> m_sessions;
        private bool IsRunning;
        private int SleepTime;

        public Master(int port = 6574, int hz = 20)
        {
            SleepTime = 1000 / hz;
            m_sessions = new ThreadSafeList<Session>();
            m_listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            m_listener.Start();
            IsRunning = true;
        }

        public void Run()
        {

            try
            {
                while (IsRunning)
                {
                    TcpClient client = m_listener.AcceptTcpClient();
                    m_sessions.Add(new Session(client));
                    Thread.Sleep(SleepTime);
                }
            }
            catch
            {
                throw new Exception("Exception occured in Master.Run()");
            }
        }

        public void RemoveSession(Session session)
        {
            m_sessions.Remove(session);
        }

        public ThreadSafeList<Session> GetSessions()
        {
            return m_sessions;
        }

        public void Stop()
        {
            IsRunning = false;
            m_sessions.Clear();
            m_sessions = null;
            m_listener.Stop();
            m_listener = null;
        }
    }
}