using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Onelooker
{
    class ClipboardManager
    {
        public delegate void ClipboardUpdateEventHandler(string text);

        private static ClipboardManager _instance;
        public event ClipboardUpdateEventHandler ClipboardUpdate;
        private InvisibleForm _form;

        private ClipboardManager() {
            _form = new InvisibleForm(this);
        }

        public static ClipboardManager GetInstance()
        {
            if (_instance == null)
                _instance = new ClipboardManager();
            return _instance;
        }

        /// <summary>
        /// Raises the <see cref="ClipboardUpdate"/> event.
        /// </summary>
        /// <param name="e">Event arguments for the event.</param>
        internal void OnClipboardUpdate()
        {
            if (ClipboardUpdate != null)
            {
                ClipboardUpdate(Clipboard.GetText());
            }
        }

        private class InvisibleForm : Form
        {
            private ClipboardManager _manager;

            public InvisibleForm(ClipboardManager manager)
            {
                this._manager = manager;
                NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
                NativeMethods.AddClipboardFormatListener(Handle);
            }
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
                {
                    _manager.OnClipboardUpdate();
                }
                base.WndProc(ref m);
            }
        }

        private static class NativeMethods
        {
            // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
            public const int WM_CLIPBOARDUPDATE = 0x031D;
            public static IntPtr HWND_MESSAGE = new IntPtr(-3);

            // See http://msdn.microsoft.com/en-us/library/ms632599%28VS.85%29.aspx#message_only
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AddClipboardFormatListener(IntPtr hwnd);

            // See http://msdn.microsoft.com/en-us/library/ms633541%28v=vs.85%29.aspx
            // See http://msdn.microsoft.com/en-us/library/ms649033%28VS.85%29.aspx
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        }
    }
}
