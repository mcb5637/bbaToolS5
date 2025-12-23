namespace bbaLib
{
    internal class BbaFileFromFilesystem : BbaFile
    {
        internal required string SourceFilePath;

        public override Stream GetStream()
        {
            return new FileStream(SourceFilePath, FileMode.Open);
        }

        public override byte[] GetBytes()
        {
            return File.ReadAllBytes(SourceFilePath);
        }

        public override string ToString()
        {
            return $"{InternalPath} <- {SourceFilePath}";
        }

        internal override BbaFile Clone()
        {
            return new BbaFileFromFilesystem()
            {
                InternalPath = InternalPath,
                SourceFilePath = SourceFilePath
            };
        }
    }
}
