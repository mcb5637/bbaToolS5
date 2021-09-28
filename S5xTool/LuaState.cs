using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace S5xTool
{
    public class LuaError : Exception
    {
        public LuaError()
        {
        }

        public LuaError(string message) : base(message)
        {
        }

        public LuaError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LuaError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class LuaUserdataFunction : Attribute
    {
        public string Name;

        public LuaUserdataFunction(string name)
        {
            Name = name;
        }
    }
    public enum LuaType
    {
        Nil,
        Boolean,
        LightUserData,
        Number,
        String,
        Table,
        Function,
        UserData,
        Thread
    }

    public abstract class LuaState
    {
        public abstract int REGISTRYINDEX { get; }
        public abstract int GLOBALSINDEX { get; }
        public abstract int Top { get; set; }

        public abstract int NOREF { get; }

        public abstract void Call(int nargs, int nres);
        public abstract void CheckIndex(int i, bool acceptZero = false, bool acceptPseudo = true, bool intop = true);
        public abstract void CheckStack(int size);
        public abstract void CheckType(int i, params LuaType[] t);
        public abstract byte[] Dump();
        public abstract bool Equal(int i1, int i2);
        public abstract bool GetMetatable(int i);
        public abstract void GetTable(int i);
        public abstract void Insert(int i);
        public abstract IEnumerable<int> IPairs(int i);
        public abstract bool LessThan(int i1, int i2);
        public abstract void LoadBuffer(string buff, string name);
        public abstract void LoadBuffer(byte[] buff, string name);
        public abstract void NewTable();
        public abstract IntPtr NewUserdata(int size);
        public abstract IEnumerable<LuaType> Pairs(int i);
        public abstract void PCall(int nargs, int nres);
        public abstract void Pop(int i);
        public abstract void Push(bool b);
        public abstract void Push(double n);
        public abstract void Push(string s);
        public abstract void Push();
        public abstract void Push(Func<LuaState, int> f, int n = 0);
        public abstract void PushValue(int i);
        public abstract bool RawEqual(int i1, int i2);
        public abstract void RawGet(int i);
        public abstract void RawGetI(int i, int key);
        public abstract void RawSet(int i);
        public abstract void RawSetI(int i, int key);
        public abstract int Ref();
        public abstract void Remove(int i);
        public abstract void Replace(int i);
        public abstract bool SetMetatable(int i);
        public abstract void SetTable(int i);
        public abstract bool ToBoolean(int i);
        public abstract double ToNumber(int ind);
        public abstract string ToString(int ind);
        public abstract IntPtr ToUserdata(int ind);
        public abstract LuaType Type(int i);
        public abstract void UnRef(int r);
        public abstract int UPVALUEINDEX(int i);




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
        public void RegisterTypeForUserdata<T>()
        {
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
                        T o = FromUserdata<T>(1);
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
                Int32 r = Marshal.ReadInt32(ToUserdata(1));
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
        /// <see cref="RegisterTypeForUserdata{T}">for info on how to add methods to it</see>
        public void PushObjectAsUserdata<T>(T ob)
        {
            CheckStack(2);
            if (ob == null)
                throw new NullReferenceException();
            if (!TypeMetatables.ContainsKey(typeof(T)))
                RegisterTypeForUserdata<T>();
            int r = TypeMetatables[typeof(T)];
            UserdataObjects[NextKey] = ob;
            IntPtr ud = NewUserdata(sizeof(Int32));
            Marshal.WriteInt32(ud, NextKey);
            RawGetI(REGISTRYINDEX, r);
            SetMetatable(-2);

            NextKey++;
        }
        public T FromUserdata<T>(int i)
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
                return (T)UserdataObjects[Marshal.ReadInt32(ToUserdata(i))];
            }
            finally
            {
                Top = t;
            }
        }

        public abstract string GetStackTrace(int from = 0, int to = -1);
        public abstract int GetCurrentFuncStackSize();
    }
}