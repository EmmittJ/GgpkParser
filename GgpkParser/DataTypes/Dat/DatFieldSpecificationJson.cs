namespace GgpkParser.DataTypes.Dat
{
    public class DatFieldSpecificationJson
    {
        public string Type { get; set; }
        public string Key { get; set; }
        public string KeyId { get; set; }
        public string KeyOffset { get; set; }
        public string Enum { get; set; }
        public bool Unique { get; set; }
        public string FilePath { get; set; }
        public string FileExt { get; set; }
        public string Display { get; set; }
        public string DisplayType { get; set; }
        public string Description { get; set; }
        
        private DatFieldType? _fieldType = null;
        public DatFieldType FieldType => _fieldType ??= new DatFieldType(Type);
    }
}