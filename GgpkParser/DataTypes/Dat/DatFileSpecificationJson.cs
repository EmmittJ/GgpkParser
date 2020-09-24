using System.Collections.Generic;

namespace GgpkParser.DataTypes.Dat
{
    public class DatFileSpecificationJson
    {
        public Dictionary<string, DatFieldSpecificationJson> Fields { get; } = new Dictionary<string, DatFieldSpecificationJson>();
    }
}