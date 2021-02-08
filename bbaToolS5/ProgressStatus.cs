using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    public class ProgressStatus
    {
        internal ProgressStatus()
        {

        }

        public int Progress
        {
            get;
            internal set;
        }

        public ProgressStatusStep Step
        {
            get;
            internal set;
        }

        public int AdditionalData
        {
            get;
            internal set;
        }

        public string AdditionalString
        {
            get;
            internal set;
        }

        public override string ToString()
        {
            string add = "";
            if (AdditionalString != null)
                add = AdditionalString;
            else if (AdditionalData != 0)
                add = AdditionalData.ToString();
            return $"{Step} {Progress}% {add}";
        }
    }

    public enum ProgressStatusStep
    {
        ReadBba_Header,
        ReadBba_HashTable,
        ReadBba_Directory,
        ReadBba_FileCatalog,
        WriteFolder_File,
        ReadFolder_File,
        WriteBba_Files,
        WriteBba_Directory,
        WriteBba_HashTable
    }
}
