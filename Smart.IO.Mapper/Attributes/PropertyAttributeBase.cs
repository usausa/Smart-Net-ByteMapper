namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class PropertyAttributeBase : Attribute, IPropertyAttribute
    {
        public int Offset { get; }

        public string[] Profiles { get; set; }

        protected PropertyAttributeBase(int offset)
        {
            Offset = offset;
        }
    }
}
