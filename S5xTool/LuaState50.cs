﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace S5xTool
{
    public class LuaState50 : LuaState
    {
        private enum LuaResult
        {
            OK,
            ERRRUN,
            ERRFILE,
            ERRSYNTAX,
            ERRMEM,
            ERRERR
        }
        public override int REGISTRYINDEX => -10000;
        public override int GLOBALSINDEX => -10001;
        public override int UPVALUEINDEX(int i)
        {
            return GLOBALSINDEX - i;
        }
        public override int NOREF => -1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int LuaCFunc(IntPtr L);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int LuaWriter(IntPtr L, IntPtr buff, int size, int data);

        // state handling
        private readonly IntPtr State;
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_open", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Lua_open();
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_close", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_close(IntPtr l);
        [DllImport("lua50/lua50.dll", EntryPoint = "luaopen_base", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Luaopen_base(IntPtr l);
        [DllImport("lua50/lua50.dll", EntryPoint = "luaopen_string", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Luaopen_string(IntPtr l);
        [DllImport("lua50/lua50.dll", EntryPoint = "luaopen_table", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Luaopen_table(IntPtr l);
        [DllImport("lua50/lua50.dll", EntryPoint = "luaopen_math", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Luaopen_math(IntPtr l);
        [DllImport("lua50/lua50.dll", EntryPoint = "luaopen_io", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Luaopen_io(IntPtr l);
        [DllImport("lua50/lua50.dll", EntryPoint = "luaopen_debug", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Luaopen_debug(IntPtr l);
        public LuaState50()
        {
            State = Lua_open();
            Luaopen_base(State);
            Luaopen_string(State);
            Luaopen_table(State);
            Luaopen_math(State);
            Luaopen_io(State);
            Luaopen_debug(State);
        }
        private readonly LinkedList<GCHandle> Handles = new LinkedList<GCHandle>();
        ~LuaState50()
        {
            foreach (GCHandle h in Handles)
                h.Free();
            Lua_close(State);
        }

        // basic stack funcs
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_gettop", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_gettop(IntPtr l);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_settop", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_settop(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_checkstack", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_checkstack(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_pushvalue", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_pushvalue(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_remove", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_remove(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_insert", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_insert(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_replace", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_replace(IntPtr l, int i);
        public override int Top
        {
            get => Lua_gettop(State);
            set
            {
                CheckIndex(value, true, false, false);
                Lua_settop(State, value);
            }
        }
        public override void CheckIndex(int i, bool acceptZero=false, bool acceptPseudo=true, bool intop=true)
        {
            int top = Top;
            if (i < 0)
            {
                if (-i <= top)
                    return;
                if (acceptZero && -i == top + 1)
                    return;
                if (acceptPseudo)
                {
                    if (i <= REGISTRYINDEX && i >= (GLOBALSINDEX - CurrentUpvalues))
                        return;
                }
                throw new LuaError($"index {i} is no valid stack pos or pseudoindex");
            }
            else if (i > 0)
            {
                if (i > top)
                {
                    if (intop)
                        throw new LuaError($"index {i} is not currently used");
                    else
                        CheckStack(i - top);
                }
            }
            else if (!acceptZero)
                throw new LuaError("index is 0");
        }
        public override void CheckStack(int size)
        {
            if (Lua_checkstack(State, size) == 0)
                throw new LuaError($"lua stack overflow at size {size}");
        }
        public override void Pop(int i)
        {
            Top = -i - 1;
        }
        public override void PushValue(int i)
        {
            CheckIndex(i, false, false, true);
            CheckStack(1);
            Lua_pushvalue(State, i);
        }
        public override void Remove(int i)
        {
            CheckIndex(i, false, false, true);
            Lua_remove(State, i);
        }
        public override void Insert(int i)
        {
            CheckIndex(i, false, false, true);
            Lua_insert(State, i);
        }
        public override void Replace(int i)
        {
            CheckIndex(i, false, false, true);
            Lua_replace(State, i);
        }

        // type checks
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_type", CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaType Lua_type(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_equal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_equal(IntPtr l, int i, int i2);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_rawequal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_rawequal(IntPtr l, int i, int i2);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_lessthan", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_lessthan(IntPtr l, int i, int i2);
        public override LuaType Type(int i)
        {
            CheckIndex(i);
            return Lua_type(State, i);
        }
        public override bool Equal(int i1, int i2)
        {
            CheckIndex(i1);
            CheckIndex(i2);
            return Lua_equal(State, i1, i2) != 0;
        }
        public override bool RawEqual(int i1, int i2)
        {
            CheckIndex(i1);
            CheckIndex(i2);
            return Lua_rawequal(State, i1, i2) != 0;
        }
        public override bool LessThan(int i1, int i2)
        {
            CheckIndex(i1);
            CheckIndex(i2);
            return Lua_lessthan(State, i1, i2) != 0;
        }

        // get values from stack
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_toboolean", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_toboolean(IntPtr l, int ind);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_tonumber", CallingConvention = CallingConvention.Cdecl)]
        private static extern double Lua_tonumber(IntPtr l, int ind);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_tostring", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Lua_tostring(IntPtr l, int ind);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_tocfunction", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Lua_tocfunction(IntPtr l, int ind);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_touserdata", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Lua_touserdata(IntPtr l, int ind);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_topointer", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Lua_topointer(IntPtr l, int ind);
        // tothread
        public override void CheckType(int i, params LuaType[] t)
        {
            LuaType ty = Type(i);
            if (!t.Contains(ty))
                throw new LuaError($"wrong type at {i}, expected {string.Join(",", t)}, found {ty}");
        }
        public override bool ToBoolean(int i)
        {
            CheckIndex(i);
            CheckType(i, LuaType.Boolean);
            return Lua_toboolean(State, i) != 0;
        }
        public override double ToNumber(int ind)
        {
            CheckIndex(ind);
            CheckType(ind, LuaType.Number);
            return Lua_tonumber(State, ind);
        }
        public override string ToString(int ind)
        {
            CheckIndex(ind);
            CheckType(ind, LuaType.String);
            return Marshal.PtrToStringAnsi(Lua_tostring(State, ind));
        }
        // cfunc, udata, thread, pointer

        // push to stack
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_pushboolean", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_pushboolean(IntPtr l, int n);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_pushnumber", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_pushnumber(IntPtr l, double n);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_pushstring", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_pushstring(IntPtr l, IntPtr p);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_pushnil", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_pushnil(IntPtr l);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_pushcclosure", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_pushcclosure(IntPtr l, IntPtr f, int n);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_pushlightuserdata", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_pushlightuserdata(IntPtr l, IntPtr p);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_newtable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_newtable(IntPtr l);
        public override void Push(bool b)
        {
            CheckStack(1);
            Lua_pushboolean(State, b ? 1 : 0);
        }
        public override void Push(double n)
        {
            CheckStack(1);
            Lua_pushnumber(State, n);
        }
        public override void Push(string s)
        {
            CheckStack(1);
            IntPtr b = Marshal.StringToHGlobalAnsi(s);
            try
            {
                Lua_pushstring(State, b);
            }
            finally
            {
                Marshal.FreeHGlobal(b);
            }
        }
        public override void Push()
        {
            CheckStack(1);
            Lua_pushnil(State);
        }
        private int CurrentUpvalues = 0;
        public override void Push(Func<LuaState, int> f, int n=0)
        {
            if (Top < n)
                throw new LuaError("not enough upvalues for c closure");
            CheckStack(1);
            LuaCFunc p = (IntPtr s) =>
            {
                int u = CurrentUpvalues;
                try
                {
                    CurrentUpvalues = n;
                    int i = f(this);
                    if (i > Top)
                        throw new LuaError("c func has noth enough values on the stack for return");
                    return i;
                }
                catch (Exception e)
                {
                    Push(e.Message);
                    Lua_error(State);
                    return 0;
                }
                finally
                {
                    CurrentUpvalues = u;
                }
            };
            Handles.AddLast(GCHandle.Alloc(p));
            Lua_pushcclosure(State, Marshal.GetFunctionPointerForDelegate(p), n);
        }
        public override void NewTable()
        {
            CheckStack(1);
            Lua_newtable(State);
        }
        // lud

        // metatable / udata
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_getmetatable", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_getmetatable(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_setmetatable", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_setmetatable(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_newuserdata", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Lua_newuserdata(IntPtr l, int size);
        public override bool GetMetatable(int i)
        {
            CheckIndex(i);
            CheckStack(1);
            return Lua_getmetatable(State, i) != 0;
        }
        public override bool SetMetatable(int i)
        {
            CheckIndex(i);
            if (Top < 1)
                throw new LuaError("setmetatable nothing on the stack");
            return Lua_setmetatable(State, i) != 0;
        }
        public override IntPtr NewUserdata(int size)
        {
            CheckStack(1);
            return Lua_newuserdata(State, size);
        }

        // load lua
        [DllImport("lua50/lua50.dll", EntryPoint = "luaL_loadbuffer", CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaResult LuaL_loadbuffer(IntPtr l, IntPtr buff, int siz, IntPtr name);
        private void CheckError(LuaResult r)
        {
            if (r != LuaResult.OK)
            {
                string s = ToString(-1);
                Pop(1);
                if (s.StartsWith("lua error:"))
                    throw new LuaError(s);
                else
                    throw new LuaError($"lua error: {r} {s}");
            }
        }
        public override void LoadBuffer(string buff, string name)
        {
            IntPtr b = Marshal.StringToHGlobalAnsi(buff);
            IntPtr n = Marshal.StringToHGlobalAnsi(name);
            try
            {
                LuaResult r = LuaL_loadbuffer(State, b, Encoding.GetEncoding(1252).GetByteCount(buff), n);
                CheckError(r);
            }
            finally
            {
                Marshal.FreeHGlobal(b);
                Marshal.FreeHGlobal(n);
            }
        }
        public override void LoadBuffer(byte[] buff, string name)
        {
            Array.Resize(ref buff, buff.Length + 1);
            buff[buff.Length - 1] = 0;
            IntPtr b = Marshal.AllocHGlobal(buff.Length);
            Marshal.Copy(buff, 0, b, buff.Length);
            IntPtr n = Marshal.StringToHGlobalAnsi(name);
            try
            {
                LuaResult r = LuaL_loadbuffer(State, b, buff.Length - 1, n);
                CheckError(r);
            }
            finally
            {
                Marshal.FreeHGlobal(b);
                Marshal.FreeHGlobal(n);
            }
        }

        // tableaccess
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_gettable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_gettable(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_rawget", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_rawget(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_settable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_settable(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_rawset", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_rawset(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_next", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_next(IntPtr l, int i);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_rawgeti", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_rawgeti(IntPtr l, int i, int k);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_rawseti", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_rawseti(IntPtr l, int i, int k);
        public override void GetTable(int i)
        {
            CheckIndex(i);
            if (Top < 1)
                throw new LuaError("no key on stack");
            Lua_gettable(State, i);
        }
        public override void RawGet(int i)
        {
            CheckIndex(i);
            if (Top < 1)
                throw new LuaError("no key on stack");
            Lua_rawget(State, i);
        }
        public override void SetTable(int i)
        {
            CheckIndex(i);
            if (Top < 2)
                throw new LuaError("no key/value on stack");
            Lua_settable(State, i);
        }
        public override void RawSet(int i)
        {
            CheckIndex(i);
            if (Top < 2)
                throw new LuaError("no key/value on stack");
            Lua_rawset(State, i);
        }
        public override IEnumerable<LuaType> Pairs(int i) // iterate over table, key is at -2, value at -1, enumerable is type of value, only access them, dont change them
        {
            int t = Top + 1;
            CheckIndex(i);
            CheckStack(2);
            Push();
            while (Lua_next(State, i) != 0)
            {
                yield return Type(-1);
                Pop(1); // remove val, keep key for next
                if (Top != t)
                    throw new LuaError("pairs stack top mismatch");
            }
            // after traversal no key gets pushed
        }
        public override IEnumerable<int> IPairs(int i) // iterate over a table in array stile, from t[1] up to the first nil found, enumerable is index/key, -1 is value, only access them, dont change them
        {
            int t = Top;
            CheckIndex(i);
            CheckStack(1);
            int ind = 1;
            while (true)
            {
                Lua_rawgeti(State, i, ind);
                if (Lua_type(State, -1) == LuaType.Nil)
                {
                    Pop(1);
                    break;
                }
                yield return ind;
                Pop(1);
                if (Top != t)
                    throw new LuaError("ipairs stack top mismatch");
                ind++;
            }
        }

        // calling
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_pcall", CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaResult Lua_pcall(IntPtr l, int nargs, int nres, int errfunc);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_call", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_call(IntPtr l, int nargs, int nres);
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_error", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Lua_error(IntPtr l);
        public override void PCall(int nargs, int nres)
        {
            if (Top < nargs + 1)
                throw new LuaError($"pcall not enough vaues on the stack");
            CheckError(Lua_pcall(State, nargs, nres, 0));
        }
        public override void Call(int nargs, int nres) // use only inside a luacfunc if you want to forward lua errors
        {
            if (Top < nargs + 1)
                throw new LuaError($"call not enough vaues on the stack");
            Lua_call(State, nargs, nres);
        }

        // ref
        [DllImport("lua50/lua50.dll", EntryPoint = "luaL_ref", CallingConvention = CallingConvention.Cdecl)]
        private static extern int LuaL_ref(IntPtr l, int t);
        [DllImport("lua50/lua50.dll", EntryPoint = "luaL_unref", CallingConvention = CallingConvention.Cdecl)]
        private static extern void LuaL_unref(IntPtr l, int t, int re);

        public override int Ref()
        {
            if (Top < 1)
                throw new LuaError($"ref nothing on the stack");
            return LuaL_ref(State, REGISTRYINDEX);
        }
        public override void UnRef(int r)
        {
            LuaL_unref(State, REGISTRYINDEX, r);
        }

        // dump
        [DllImport("lua50/lua50.dll", EntryPoint = "lua_dump", CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaResult Lua_dump(IntPtr l, IntPtr wr, int data);
        public override byte[] Dump()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);
            LuaWriter wr = (IntPtr L, IntPtr buff, int size, int data) =>
            {
                byte[] buff2 = new byte[size];
                Marshal.Copy(buff, buff2, 0, size);
                w.Write(buff2);
                return 0;
            };
            Lua_dump(State, Marshal.GetFunctionPointerForDelegate(wr), 0);
            GC.KeepAlive(wr);
            w.Flush();
            return s.ToArray();
        }

        // object as udata

        private readonly Dictionary<Type, int> TypeMetatables = new Dictionary<Type, int>();
        private readonly Dictionary<Int32, object> UserdataObjects = new Dictionary<Int32, object>();
        private Int32 NextKey = 0;
        /// <summary>
        /// registers a type in this lua state to use as userdata.
        /// annotate methods with LuaUserdataFunction to make them accesible in lua.
        /// method signature should be public int Func(LuaState50 l).
        /// first paramether from lua (this/self) gets removed automatically.
        /// 
        /// this is the expensive part of pushing an object, but it is only needed once.
        /// you can call this method way in advance of actually pushing the objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public override void RegisterType<T>() {
            if (TypeMetatables.ContainsKey(typeof(T)))
                return;
            string n = typeof(T).FullName;
            int t = Top;

            NewTable();
            Push("TypeName");
            Push(n);
            SetTable(-3);
            Push("__index");
            NewTable();

            foreach (MethodInfo m in typeof(T).GetMethods())
            {
                LuaUserdataFunction f = m.GetCustomAttribute<LuaUserdataFunction>();
                if (f != null)
                {
                    Func<T, LuaState, int> del = (Func<T, LuaState, int>)Delegate.CreateDelegate(typeof(Func<T, LuaState, int>), m);
                    Func<LuaState, int> d2 = (LuaState s) =>
                    {
                        T o = GetObject<T>(1);
                        s.Remove(1);
                        return del(o, s);
                    };
                    Push(f.Name);
                    Push(d2);
                    SetTable(-3);
                }
            }

            SetTable(-3);

            Push("__gc");
            Push((s) =>
            {
                Int32 r = Marshal.ReadInt32(Lua_touserdata(State, 1));
                UserdataObjects.Remove(r);
                return 0;
            });
            SetTable(-3);

            TypeMetatables[typeof(T)] = Ref();

            Top = t;
        }
        /// <summary>
        /// pushes a c# object to acces it from lua. object keeps being referenced until lua gcs it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <see cref="RegisterType{T}">for info on how to add methods to it</see>
        public override void PushObject<T>(T ob)
        {
            CheckStack(2);
            if (ob == null)
                throw new NullReferenceException();
            if (!TypeMetatables.ContainsKey(typeof(T)))
                RegisterType<T>();
            int r = TypeMetatables[typeof(T)];
            UserdataObjects[NextKey] = ob;
            IntPtr ud = Lua_newuserdata(State, sizeof(Int32));
            Marshal.WriteInt32(ud, NextKey);
            Lua_rawgeti(State, REGISTRYINDEX, r);
            SetMetatable(-2);

            NextKey++;
        }
        public override T GetObject<T>(int i)
        {
            CheckIndex(i);
            CheckType(i, LuaType.UserData);
            int t = Top;
            try
            {
                if (!GetMetatable(i))
                    throw new LuaError("udata has no metatable");
                Push("TypeName");
                GetTable(-2);
                Push(typeof(T).FullName);
                if (!Equal(-1, -2))
                    throw new LuaError("udata type does not match");
                Pop(3);
                return (T)UserdataObjects[Marshal.ReadInt32(Lua_touserdata(State, i))];
            }
            finally
            {
                Top = t;
            }
        }
    }
}
