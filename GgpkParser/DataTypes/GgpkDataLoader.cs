using GgpkParser.DataTypes.Specifications;
using System;
using System.Linq;

namespace GgpkParser.DataTypes
{
    public class GgpkDataLoader
    {
        public static IDataSpecification CreateDataSpecification(string name)
        {
            var types =
                from a in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                from t in a.GetTypes()
                where typeof(IDataSpecification).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract
                let attributes = t.GetCustomAttributes(typeof(SpecificationAttribute), true)
                where attributes != null && attributes.Length > 0
                let filtered = attributes.Cast<SpecificationAttribute>().Where(x => name.EndsWith(x.FileExtension) || string.IsNullOrWhiteSpace(x.FileExtension))
                where filtered != null && filtered.Any()
                select new { Type = t, Priority = filtered.Max(x => x.Priority) };

            var type = types.OrderByDescending(x => x.Priority).First();
            return Activator.CreateInstance(type.Type, name) as IDataSpecification ?? new RawBytesSpecification();
        }
    }
}
