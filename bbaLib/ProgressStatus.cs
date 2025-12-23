namespace bbaLib
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

        public string? AdditionalString
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
        ReadBbaHeader,
        ReadBbaHashTable,
        ReadBbaDirectory,
        ReadBbaFileCatalog,
        WriteFolderFile,
        ReadFolderFile,
        WriteBbaFiles,
        WriteBbaDirectory,
        WriteBbaHashTable
    }
}
