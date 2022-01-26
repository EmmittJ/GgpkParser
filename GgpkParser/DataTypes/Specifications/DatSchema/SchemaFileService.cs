using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace GgpkParser.DataTypes.DatSchema
{
    public class SchemaFileService
    {
        private const string SchemaFileName = "schema.min.json";
        private static SchemaFile? _datSpecJson = null;
        public static SchemaFile Default
        {
            get
            {
                var pwd = Assembly.GetExecutingAssembly().Location;
                if (_datSpecJson is null && new FileInfo(pwd) is { Directory: DirectoryInfo directory })
                {
                    var specFile = Path.Combine(directory.FullName, SchemaFileName);
                    _datSpecJson = JsonConvert.DeserializeObject<SchemaFile>(File.ReadAllText(specFile));
                }

                return _datSpecJson ?? throw new FileNotFoundException($"{SchemaFileName} was not found in {pwd}");
            }
        }
    }
}
