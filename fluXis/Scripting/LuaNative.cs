using System;
using System.Runtime.InteropServices;

namespace fluXis.Scripting;

public class LuaNative
{
    private const string dll_name = "lua54";

    [DllImport(dll_name, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int luaopen_base(IntPtr state);

    [DllImport(dll_name, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int luaopen_table(IntPtr state);

    [DllImport(dll_name, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int luaopen_string(IntPtr state);

    [DllImport(dll_name, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int luaopen_math(IntPtr state);
}
