﻿using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices {
    public static class InternalCall {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Register(IntPtr address);
    }
}