/*
    Copyright � 2002, The KPD-Team
    All rights reserved.
    http://www.mentalis.org/
  Redistribution and use in source and binary forms, with or without
  modification, are permitted provided that the following conditions
  are met:
    - Redistributions of source code must retain the above copyright
       notice, this list of conditions and the following disclaimer. 
    - Neither the name of the KPD-Team, nor the names of its contributors
       may be used to endorse or promote products derived from this
       software without specific prior written permission. 
  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
  FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
  THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
  STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
  OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Net.Sockets;
using System.Text;

namespace FFramework_Core.Network.ProxySocket.Authentication
{
	internal sealed class AuthUserPass : AuthMethod
    {
		public AuthUserPass(Socket server, string user, string pass) : base(server)
        {
			Username = user;
			Password = pass;
		}

        private byte[] GetAuthenticationBytes()
        {
			byte[] buffer = new byte[3 + Username.Length + Password.Length];
			buffer[0] = 1;
			buffer[1] = (byte)Username.Length;
			Array.Copy(Encoding.ASCII.GetBytes(Username), 0, buffer, 2, Username.Length);
			buffer[Username.Length + 2] = (byte)Password.Length;
			Array.Copy(Encoding.ASCII.GetBytes(Password), 0, buffer, Username.Length + 3, Password.Length);
			return buffer;
		}

		public override void Authenticate()
        {
			Server.Send(GetAuthenticationBytes());
			byte[] buffer = new byte[2];
			int received = 0;
			while (received != 2)
				received += Server.Receive(buffer, received, 2 - received, SocketFlags.None);

            if (buffer[1] != 0)
            {
				Server.Close();
				throw new ProxyException("Username/password combination rejected.");
			}
			return;
		}
		
		public override void BeginAuthenticate(HandShakeComplete callback)
        {
			CallBack = callback;
			Server.BeginSend(GetAuthenticationBytes(), 0, 3 + Username.Length + Password.Length, SocketFlags.None, new AsyncCallback(OnSent), Server);
			return;
		}

        private void OnSent(IAsyncResult ar)
        {
			try
            {
				Server.EndSend(ar);
				Buffer = new byte[2];
				Server.BeginReceive(Buffer, 0, 2, SocketFlags.None, new AsyncCallback(OnReceive), Server);
			}
            catch (Exception e)
            {
				CallBack(e);
			}
		}

        private void OnReceive(IAsyncResult ar)
        {
			try
            {
				Received += Server.EndReceive(ar);
				if (Received == Buffer.Length)
					if (Buffer[1] == 0)
						CallBack(null);
					else
						throw new ProxyException("Username/password combination not accepted.");
				else
					Server.BeginReceive(Buffer, Received, Buffer.Length - Received, SocketFlags.None, new AsyncCallback(this.OnReceive), Server);
			}
            catch (Exception e)
            {
				CallBack(e);
			}
		}
		
		private string Username
        {
			get
            {
				return m_Username;
			}
			set
            {
                m_Username = value ?? throw new ArgumentNullException();
			}
		}
		
		private string Password
        {
			get
            {
				return m_Password;
			}
			set
            {
                m_Password = value ?? throw new ArgumentNullException();
			}
		}
		
		private string m_Username;
		private string m_Password;
	}
}