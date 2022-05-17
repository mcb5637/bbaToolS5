using bbaToolS5;
using LuaSharp;
using System.IO;
using System.Text;

namespace S5xTool
{
    internal class ArchiveAccess
    {
        internal readonly BbaArchive A;

        public ArchiveAccess(BbaArchive a = null)
        {
            if (a == null)
                a = new BbaArchive();
            A = a;
        }

        [LuaUserdataFunction("LoadBba")]
        public int LoadBba(LuaState l)
        {
            try
            {
                A.ReadBba(l.ToString(2));
            }
            catch (IOException e)
            {
                throw new LuaException(e.Message);
            }
            return 0;
        }
        [LuaUserdataFunction("LoadFolder")]
        public int LoadFolder(LuaState l)
        {
            try
            {
                A.ReadFromFolder(l.ToString(2));
            }
            catch (IOException e)
            {
                throw new LuaException(e.Message);
            }
            return 0;
        }
        [LuaUserdataFunction("WriteBba")]
        public int WriteBba(LuaState l)
        {
            try
            {
                A.WriteToBba(l.ToString(2));
            }
            catch (IOException e)
            {
                throw new LuaException(e.Message);
            }
            return 0;
        }
        [LuaUserdataFunction("WriteFolder")]
        public int WriteFolder(LuaState l)
        {
            try
            {
                A.WriteToFolder(l.ToString(2));
            }
            catch (IOException e)
            {
                throw new LuaException(e.Message);
            }
            return 0;
        }
        [LuaUserdataFunction("AddFileFromFilesystem")]
        public int AddFileFromFilesystem(LuaState l)
        {
            try
            {
                A.AddFileFromFilesystem(l.ToString(2), l.ToString(3));
            }
            catch (IOException e)
            {
                throw new LuaException(e.Message);
            }
            return 0;
        }
        [LuaUserdataFunction("AddFileFromString")]
        public int AddFileFromString(LuaState l)
        {
            try
            {
                A.AddFileFromMem(StringMarshaler.EncodingUsed.GetBytes(l.ToString(2)), l.ToString(3));
            }
            catch (IOException e)
            {
                throw new LuaException(e.Message);
            }
            return 0;
        }
        [LuaUserdataFunction("GetFile")]
        public int GetFile(LuaState l)
        {
            try
            {
                BbaFile f = A.GetFileByName(l.ToString(2));
                if (f == null)
                    throw new LuaException("file doesnt exist in the archive");
                l.Push(StringMarshaler.EncodingUsed.GetString(f.GetBytes()));
            }
            catch (IOException e)
            {
                throw new LuaException(e.Message);
            }
            return 1;
        }
        [LuaUserdataFunction("RemoveFile")]
        public int RemoveFile(LuaState l)
        {
            try
            {
                BbaFile f = A.GetFileByName(l.ToString(2));
                if (f == null)
                    throw new LuaException("file doesnt exist in the archive");
                A.RemoveFile(f.InternalPath);
            }
            catch (IOException e)
            {
                throw new LuaException(e.Message);
            }
            return 0;
        }
        [LuaUserdataFunction("RenameFile")]
        public int RenameFile(LuaState l)
        {
            try
            {
                BbaFile f = A.GetFileByName(l.ToString(2));
                if (f == null)
                    throw new LuaException("file doesnt exist in the archive");
                A.RenameFile(f.InternalPath, l.ToString(3));
            }
            catch (IOException e)
            {
                throw new LuaException(e.Message);
            }
            return 0;
        }
        [LuaUserdataFunction("GetFileNames")]
        public int GetFileNames(LuaState l)
        {
            try
            {
                l.NewTable();
                int i = 1;
                foreach (BbaFile f in A)
                {
                    l.Push(i);
                    l.Push(f.InternalPath);
                    l.SetTable(-3);
                    i++;
                }
            }
            catch (IOException e)
            {
                throw new LuaException(e.Message);
            }
            return 1;
        }
        [LuaUserdataFunction("Clear")]
        public int Clear(LuaState l)
        {
            A.Clear();
            return 0;
        }
    }
}
