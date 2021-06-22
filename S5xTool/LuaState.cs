using System;
using System.Collections.Generic;
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
        public abstract T GetObject<T>(int i);
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
        public abstract void PushObject<T>(T ob);
        public abstract void PushValue(int i);
        public abstract bool RawEqual(int i1, int i2);
        public abstract void RawGet(int i);
        public abstract void RawSet(int i);
        public abstract int Ref();
        public abstract void RegisterType<T>();
        public abstract void Remove(int i);
        public abstract void Replace(int i);
        public abstract bool SetMetatable(int i);
        public abstract void SetTable(int i);
        public abstract bool ToBoolean(int i);
        public abstract double ToNumber(int ind);
        public abstract string ToString(int ind);
        public abstract LuaType Type(int i);
        public abstract void UnRef(int r);
        public abstract int UPVALUEINDEX(int i);
    }
}