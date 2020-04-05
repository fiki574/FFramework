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

using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace FFramework_Crypto.Network
{
    public class RC4
    {
        private readonly RC4Engine ServerToClient = new RC4Engine(), ClientToServer = new RC4Engine();
        public bool Initialized;

        public void SetKey(byte[] key)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            byte[] sha1Key = sha1.ComputeHash(key);
            var keyParam = new KeyParameter(sha1Key);
            ClientToServer.Init(true, keyParam);
            ServerToClient.Init(false, keyParam);
            Initialized = true;
        }

        public void EncryptClientData(byte[] data, int offset, int size)
        {
            ClientToServer.ProcessBytes(data, offset, size, data, offset);
        }

        public void DecryptServerData(byte[] data, int offset, int size)
        {
            ServerToClient.ProcessBytes(data, offset, size, data, offset);
        }
    }
}