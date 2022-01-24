using System;

namespace GgpkParser.DataTypes
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class SpecificationAttribute : Attribute
    {
        public virtual string FileExtension { get; set; } = string.Empty;
        public virtual int Priority { get; set; } = 0;
    }
}
