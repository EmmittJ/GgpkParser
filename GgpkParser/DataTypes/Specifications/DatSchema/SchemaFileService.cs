using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GgpkParser.DataTypes.DatSchema
{
    public class SchemaFileService
    {
        private static SchemaFile? _datSpecJson { get; set; } = null;
        public static SchemaFile Default
        {
            get
            {
                if (_datSpecJson is null)
                {
                    var directory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
                    var specFile = Path.Combine(directory, "schema.min.json");
                    _datSpecJson = JsonConvert.DeserializeObject<SchemaFile>(File.ReadAllText(specFile));
                }

                return _datSpecJson;
            }
        }
    }
}
