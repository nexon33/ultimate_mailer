using System;
using System.Runtime.InteropServices;

namespace ultimate.mailer
{
    internal static class NativeMethods
    {
        [DllImport("user32")]
        internal static extern bool HideCaret(IntPtr hWnd);
    }
}
