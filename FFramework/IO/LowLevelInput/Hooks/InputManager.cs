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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FFramework.LowLevelInput.Converters;

namespace FFramework.LowLevelInput.Hooks
{
    public class InputManager : IDisposable
    {
        public delegate void KeyStateChangedEventHandler(VirtualKeyCode key, KeyState state);

        private readonly object _lockObject;

        private LowLevelKeyboardHook _keyboardHook;
        private Dictionary<VirtualKeyCode, List<KeyStateChangedEventHandler>> _keyStateChangedCallbacks;

        private Dictionary<VirtualKeyCode, KeyState> _keyStates;
        private LowLevelMouseHook _mouseHook;

        public InputManager()
        {
            _lockObject = new object();
        }

        public InputManager(bool captureMouseMove, bool installMouseHook = true)
        {
            _lockObject = new object();

            Initialize(captureMouseMove, false, installMouseHook);
        }

        public InputManager(bool captureMouseMove, bool clearInjectedFlag, bool installMouseHook = true)
        {
            _lockObject = new object();

            Initialize(captureMouseMove, clearInjectedFlag, installMouseHook);
        }

        public bool IsInitialized { get; private set; }

        public bool CaptureMouseMove
        {
            get
            {
                var tmp = _mouseHook;

                if (tmp == null)
                    throw new InvalidOperationException("The " + nameof(InputManager) + " is not initialized.");

                return tmp.CaptureMouseMove;
            }
            set
            {
                var tmp = _mouseHook;

                if (tmp == null)
                    throw new InvalidOperationException("The " + nameof(InputManager) + " is not initialized.");

                tmp.CaptureMouseMove = value;
            }
        }

        public bool ClearInjectedFlag
        {
            get
            {
                var tmpKeyboard = _keyboardHook;
                var tmpMouse = _mouseHook;

                if (tmpKeyboard == null || tmpMouse == null)
                    throw new InvalidOperationException("The " + nameof(InputManager) + " is not initialized.");

                return tmpKeyboard.ClearInjectedFlag;
            }
            set
            {
                var tmpKeyboard = _keyboardHook;
                var tmpMouse = _mouseHook;

                if (tmpKeyboard == null || tmpMouse == null)
                    throw new InvalidOperationException("The " + nameof(InputManager) + " is not initialized.");

                tmpKeyboard.ClearInjectedFlag = value;
                tmpMouse.ClearInjectedFlag = value;
            }
        }

        public event LowLevelKeyboardHook.KeyboardEventHandler OnKeyboardEvent;
        public event LowLevelMouseHook.MouseEventHandler OnMouseEvent;

        ~InputManager()
        {
            Dispose(false);
        }

        public void Initialize(bool installMouseHook = true)
        {
            Initialize(true, false, installMouseHook);
        }

        public void Initialize(bool captureMouseMove, bool clearInjectedFlag, bool installMouseHook)
        {
            lock (_lockObject)
            {
                if (IsInitialized)
                    throw new InvalidOperationException("The " + nameof(InputManager) + " is already initialized.");

                _keyStateChangedCallbacks = new Dictionary<VirtualKeyCode, List<KeyStateChangedEventHandler>>();
                _keyStates = new Dictionary<VirtualKeyCode, KeyState>();

                foreach (KeyValuePair<VirtualKeyCode, string> pair in KeyCodeConverter.EnumerateVirtualKeyCodes())
                {
                    _keyStateChangedCallbacks.Add(pair.Key, new List<KeyStateChangedEventHandler>());
                    _keyStates.Add(pair.Key, KeyState.None);
                }

                _keyboardHook = new LowLevelKeyboardHook(clearInjectedFlag);
                _keyboardHook.OnKeyboardEvent += _keyboardHook_OnKeyboardEvent;
                _keyboardHook.InstallHook();

                if (installMouseHook)
                {
                    _mouseHook = new LowLevelMouseHook(captureMouseMove, clearInjectedFlag);
                    _mouseHook.OnMouseEvent += _mouseHook_OnMouseEvent;
                    _mouseHook.InstallHook();
                }

                IsInitialized = true;
            }
        }

        private void _mouseHook_OnMouseEvent(VirtualKeyCode key, KeyState state, int x, int y)
        {
            if (key == VirtualKeyCode.Invalid && !CaptureMouseMove) return;

            state = state == KeyState.Down && _keyStates[key] == KeyState.Down
                ? KeyState.Pressed
                : state;

            _keyStates[key] = state;

            var mouseEvents = OnMouseEvent;

            if (mouseEvents != null)
                Task.Factory.StartNew(() => mouseEvents.Invoke(key, state, x, y));
            
            Task.Factory.StartNew(() =>
            {
                List<KeyStateChangedEventHandler> curCallbacks = _keyStateChangedCallbacks[key];

                if (curCallbacks == null) return;
                if (curCallbacks.Count == 0) return;

                foreach (var callback in curCallbacks)
                    callback(key, state);
            });
        }

        private void _keyboardHook_OnKeyboardEvent(VirtualKeyCode key, KeyState state)
        {
            if (key == VirtualKeyCode.Invalid) return;

            state = state == KeyState.Down && _keyStates[key] == KeyState.Down
                ? KeyState.Pressed
                : state;

            _keyStates[key] = state;

            var keyboardEvents = OnKeyboardEvent;

            if (keyboardEvents != null)
                Task.Factory.StartNew(() => keyboardEvents.Invoke(key, state));

            Task.Factory.StartNew(() =>
            {
                List<KeyStateChangedEventHandler> curCallbacks = _keyStateChangedCallbacks[key];

                if (curCallbacks == null) return;
                if (curCallbacks.Count == 0) return;

                foreach (var callback in curCallbacks)
                    callback(key, state);
            });
        }

        public void Terminate()
        {
            lock (_lockObject)
            {
                if (!IsInitialized)
                    throw new InvalidOperationException("The " + nameof(InputManager) +
                                                        " needs to be initialized before it can be terminated.");

                if (_keyboardHook != null)
                {
                    _keyboardHook.Dispose();
                    _keyboardHook = null;
                }

                if (_mouseHook != null)
                {
                    _mouseHook.Dispose();
                    _mouseHook = null;
                }

                OnKeyboardEvent = null;
                OnMouseEvent = null;

                _keyStateChangedCallbacks = null;
                _keyStates = null;

                IsInitialized = false;
            }
        }

        public KeyState GetState(VirtualKeyCode key)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("The " + nameof(InputManager) +
                                                    " needs to be initialized before it can execute this method.");

            if (key == VirtualKeyCode.Invalid) return KeyState.None;

            return _keyStates[key];
        }

        public void SetState(VirtualKeyCode key, KeyState state)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("The " + nameof(InputManager) +
                                                    " needs to be initialized before it can execute this method.");

            if (key == VirtualKeyCode.Invalid) return;

            _keyStates[key] = state;
        }

        public bool IsPressed(VirtualKeyCode key)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("The " + nameof(InputManager) +
                                                    " needs to be initialized before it can execute this method.");

            if (key == VirtualKeyCode.Invalid) return false;

            var state = _keyStates[key];

            return state == KeyState.Down || state == KeyState.Pressed;
        }

        public bool WasPressed(VirtualKeyCode key)
        {
            var state = GetState(key);

            if (state == KeyState.Down || state == KeyState.Pressed)
            {
                SetState(key, KeyState.Up);

                return true;
            }

            return false;
        }

        public void RegisterEvent(VirtualKeyCode key, KeyStateChangedEventHandler handler)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("The " + nameof(InputManager) +
                                                    " needs to be initialized before it can execute this method.");

            if (key == VirtualKeyCode.Invalid)
                throw new ArgumentException("VirtualKeyCode.INVALID is not supported by this method.", nameof(key));

            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (_lockObject)
            {
                _keyStateChangedCallbacks[key].Add(handler);
            }
        }

        public bool UnregisterEvent(VirtualKeyCode key, KeyStateChangedEventHandler handler)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("The " + nameof(InputManager) +
                                                    " needs to be initialized before it can execute this method.");

            if (key == VirtualKeyCode.Invalid)
                throw new ArgumentException("VirtualKeyCode.INVALID is not supported by this method.", nameof(key));

            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (_lockObject)
            {
                return _keyStateChangedCallbacks[key].Remove(handler);
            }
        }

        public bool WaitForEvent(VirtualKeyCode key, KeyState state = KeyState.None, int timeout = -1)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("The " + nameof(InputManager) +
                                                    " needs to be initialized before it can execute this method.");

            if (key == VirtualKeyCode.Invalid)
                throw new ArgumentException("VirtualKeyCode.INVALID is not supported by this method.", nameof(key));

            var threadLock = new object();

            KeyStateChangedEventHandler handler = (curKey, curState) =>
            {
                if (curKey != key) return;

                if (curState != state && state != KeyState.None) return;

                if (!Monitor.TryEnter(threadLock)) return;

                Monitor.PulseAll(threadLock);
                Monitor.Exit(threadLock);
            };

            bool result;

            RegisterEvent(key, handler);

            Monitor.Enter(threadLock);

            if (timeout < 0)
            {
                Monitor.Wait(threadLock);
                result = true;
            }
            else
            {
                result = Monitor.Wait(threadLock, timeout);
            }

            UnregisterEvent(key, handler);

            return result;
        }

        public VirtualKeyCode GetHotkey(int timeout = -1)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("The " + nameof(InputManager) +
                                                    " needs to be initialized before it can execute this method.");

            var threadLock = new object();
            VirtualKeyCode hotkey = VirtualKeyCode.Invalid;

            LowLevelMouseHook.MouseEventHandler mouseEventHandler = (VirtualKeyCode key, KeyState state, int x, int y) =>
            {
                if (state != KeyState.Down) return;

                hotkey = key;

                if (!Monitor.TryEnter(threadLock)) return;

                // someone else has the lock
                Monitor.PulseAll(threadLock);
                Monitor.Exit(threadLock);
            };

            LowLevelKeyboardHook.KeyboardEventHandler keyboardEventHandler = (VirtualKeyCode key, KeyState state) =>
            {
                if (state != KeyState.Down) return;

                hotkey = key;

                if (!Monitor.TryEnter(threadLock)) return;

                // someone else has the lock
                Monitor.PulseAll(threadLock);
                Monitor.Exit(threadLock);
            };

            this.OnMouseEvent += mouseEventHandler;
            this.OnKeyboardEvent += keyboardEventHandler;

            bool result;
            
            Monitor.Enter(threadLock);

            if (timeout < 0)
            {
                Monitor.Wait(threadLock);
                result = true;
            }
            else
            {
                result = Monitor.Wait(threadLock, timeout);
            }

            this.OnMouseEvent -= mouseEventHandler;
            this.OnKeyboardEvent -= keyboardEventHandler;

            return hotkey;
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            try
            {
                if (IsInitialized) Terminate();
            }
            catch
            {
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}