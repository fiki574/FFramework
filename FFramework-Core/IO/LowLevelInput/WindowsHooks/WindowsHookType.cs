﻿/*
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

namespace FFramework_Core.LowLevelInput.WindowsHooks
{ 
    public enum WindowsHookType
    { 
        MsgFilter = -1,
        JournalRecord = 0,
        JournalPlayback = 1,
        Keyboard = 2,
        GetMessage = 3,
        CallWndProc = 4,
        Cbt = 5,
        SysMsgFilter = 6,
        Mouse = 7,
        Undocumented = 8,
        Debug = 9,
        Shell = 10,
        ForegroundIdle = 11,
        CallWndProcRet = 12,
        LowLevelKeyboard = 13,
        LowLevelMouse = 14
    }
}