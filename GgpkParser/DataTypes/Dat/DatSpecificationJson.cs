using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GgpkParser.DataTypes.Dat
{
    public class DatSpecificationJson : Dictionary<string, DatFileSpecificationJson>
    {
        private static DatSpecificationJson? _datSpecJson { get; set; } = null;
        public static DatSpecificationJson Default
        {
            get
            {
                if (_datSpecJson is null)
                {
                    var directory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
                    var specFile = Path.Combine(directory, "stable.json");
                    _datSpecJson = JsonConvert.DeserializeObject<DatSpecificationJson>(File.ReadAllText(specFile));
                }

                return _datSpecJson;
            }
        }
    }
}
