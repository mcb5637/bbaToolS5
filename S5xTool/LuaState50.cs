using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace S5xTool
{
    class LuaState50
    {
        public enum LuaResult
        {
            OK,
            ERRRUN,
            ERRFILE,
            ERRSYNTAX,
            ERRMEM,
            ERRERR
        }

        private readonly IntPtr State;
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_open", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Lua_open();
        public LuaState50()
        {
            State = Lua_open();
        }
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_close", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_close(IntPtr l);
        ~LuaState50()
        {
            Lua_close(State);
        }

        [DllImport("lua50/lua50.dll", EntryPoint = "lua_pushnumber", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_pushnumber(IntPtr l, double n);
        public void PushNumber(double n)
        {
            Lua_pushnumber(State, n);
        }

        [DllImport("lua50/lua50.dll", EntryPoint = "lua_tonumber", CallingConvention = CallingConvention.Cdecl)]
        private static extern double Lua_tonumber(IntPtr l, int ind);
        public double ToNumber(int ind)
        {
            return Lua_tonumber(State, ind);
        }
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_tostring", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Lua_tostring(IntPtr l, int ind);
        public string ToString(int ind)
        {
            return Marshal.PtrToStringAnsi(Lua_tostring(State, ind));
        }


        [DllImport("lua50/lua50.dll", EntryPoint = "luaL_loadbuffer", CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaResult LuaL_loadbuffer(IntPtr l, IntPtr buff, int siz, IntPtr name);
        public LuaResult LoadBuffer(string buff, string name)
        {
            IntPtr b = Marshal.StringToHGlobalAnsi(buff);
            IntPtr n = Marshal.StringToHGlobalAnsi(name);
            try
            {
                return LuaL_loadbuffer(State, b, Encoding.GetEncoding(1252).GetByteCount(buff), n);
            }
            finally
            {
                Marshal.FreeHGlobal(b);
                Marshal.FreeHGlobal(n);
            }
        }
        public LuaResult LoadBuffer(byte[] buff, string name)
        {
            Array.Resize(ref buff, buff.Length + 1);
            buff[buff.Length - 1] = 0;
            IntPtr b = Marshal.AllocHGlobal(buff.Length);
            Marshal.Copy(buff, 0, b, buff.Length);
            IntPtr n = Marshal.StringToHGlobalAnsi(name);
            try
            {
                return LuaL_loadbuffer(State, b, buff.Length - 1, n);
            }
            finally
            {
                Marshal.FreeHGlobal(b);
                Marshal.FreeHGlobal(n);
            }
        }

        [DllImport("lua50/lua50.dll", EntryPoint = "lua_pcall", CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaResult Lua_pcall(IntPtr l, int nargs, int nres, int errfunc);
        public LuaResult PCall(int nargs, int nres, int errfunc=0)
        {
            return Lua_pcall(State, nargs, nres, errfunc);
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int LuaWriter(IntPtr L, IntPtr buff, int size, int data);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_dump", CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaResult Lua_dump(IntPtr l, IntPtr wr, int data);
        public byte[] Dump()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);
            int wr(IntPtr L, IntPtr buff, int size, int data)
            {
                byte[] buff2 = new byte[size];
                Marshal.Copy(buff, buff2, 0, size);
                w.Write(buff2);
                return 0;
            }
            Lua_dump(State, Marshal.GetFunctionPointerForDelegate((LuaWriter)wr), 0);
            w.Flush();
            return s.ToArray();
        }
    }
}
