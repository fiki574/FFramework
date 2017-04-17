/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2017 Bruno Fištrek

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
using System.IO;
using System.Net;
using System.Threading;

namespace FFramework.HTTP
{
    public class HttpServer
    {
        public static List<string> files = new List<string>();
        private HttpListener m_listener;

        public HttpServer(int port = 8080)
        {
            try
            {
                if (Directory.Exists("Website"))
                {
                    LoadWebsiteFiles("Website");
                    m_listener = new HttpListener();
                    m_listener.Prefixes.Add("http://*:" + port + "/");
                }
                else throw new Exception("'Website' directory does not exist!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public void LoadWebsiteFiles(string start)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(start);
                DirectoryInfo[] dirs = dir.GetDirectories();
                FileInfo[] fs = dir.GetFiles();
                foreach (FileInfo f in fs) files.Add(f.FullName.Replace('\\', '/'));
                if (dirs.Length > 0) foreach (DirectoryInfo d in dirs) LoadWebsiteFiles(start + "\\" + d.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public void Start()
        {
            m_listener.Start();
            m_listener.BeginGetContext(OnGetContext, null);
        }

        public void Stop()
        {
            m_listener.Stop();
        }

        private void OnGetContext(IAsyncResult result)
        {
            try
            {
                var context = m_listener.EndGetContext(result);
                ThreadPool.QueueUserWorkItem(HandleRequest, context);
            }
            finally
            {
                m_listener.BeginGetContext(OnGetContext, null);
            }
        }

        private void HandleRequest(object oContext)
        {
            HttpListenerContext context = (HttpListenerContext)oContext;
            try
            {
                string[] raw = context.Request.RawUrl.Split('&');
                if (raw[0] == "/favicon.ico") return;
                context.Response.ContentEncoding = context.Request.ContentEncoding;
                context.Response.ContentType = MIME.GetMimeType(Path.GetExtension(raw[0]));
                string path = files.Find(f => f.ToString().Contains(raw[0]));
                using (StreamReader sr = new StreamReader(System.IO.File.OpenRead(path)))
                using (StreamWriter writer = new StreamWriter(context.Response.OutputStream, context.Response.ContentEncoding)) writer.Write(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                context.Response.Close();
            }
        }
    }
}
