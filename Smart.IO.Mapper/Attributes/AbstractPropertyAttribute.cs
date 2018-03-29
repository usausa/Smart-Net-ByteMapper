namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Reflection;

    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Mappings;
    using Smart.Reflection;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AbstractPropertyAttribute : Attribute, IPropertyMappingAttribute
    {
        public string[] Profiles { get; set; }

        public int Offset { get; }

        protected AbstractPropertyAttribute(int offset)
        {
            Offset = offset;
        }

        public IMapping CreateMapping(IMappingCreateContext context, PropertyInfo pi)
        {
            var delegateFactory = context.Components.Get<IDelegateFactory>();

            return new MemberMapping(
                Offset,
                CreateConverter(context, pi),
                delegateFactory.CreateGetter(pi),
                delegateFactory.CreateSetter(pi));
        }

        protected abstract IByteConverter CreateConverter(IMappingCreateContext context, PropertyInfo pi);
    }
}
