/*
    MIT License

    Copyright (c) 2018 Michel

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using FFramework.LowLevelInput.Hooks;

namespace FFramework.LowLevelInput.Converters
{
    public static class KeyStateConverter
    {
        private static readonly string[] KeyStateMap =
        {
            "None",
            "Up",
            "Down",
            "Pressed"
        };

        public static KeyState ToKeyState(string name)
        {
            if (string.IsNullOrEmpty(name)) return KeyState.None;
            if (string.IsNullOrWhiteSpace(name)) return KeyState.None;

            string tmp = name.ToLower();

            for (int i = 0; i < KeyStateMap.Length; i++)
                if (tmp == KeyStateMap[i].ToLower()) return (KeyState) i;

            return KeyState.None;
        }

        public static KeyState ToKeyState(int state)
        {
            if (state < 0) return KeyState.None;
            if (state >= KeyStateMap.Length) return KeyState.None;

            return (KeyState) state;
        }

        public static string ToString(KeyState state)
        {
            int index = (int) state;

            if (index < 0) return "None";

            return index >= KeyStateMap.Length ? "None" : KeyStateMap[index];
        }

        public static string ToString(int index)
        {
            if (index < 0) return "None";

            return index >= KeyStateMap.Length ? "None" : KeyStateMap[index];
        }
    }
}