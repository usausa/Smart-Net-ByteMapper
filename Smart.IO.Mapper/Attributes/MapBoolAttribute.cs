namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public sealed class MapBoolAttribute : AbstractMapMemberAttribute
    {
        private byte? trueValue;

        private byte? falseValue;

        private byte? nullValue;

        public byte TrueValue
        {
            get => throw new NotSupportedException();
            set => trueValue = value;
        }

        public byte FalseValue
        {
            get => throw new NotSupportedException();
            set => falseValue = value;
        }

        public byte NullValue
        {
            get => throw new NotSupportedException();
            set => nullValue = value;
        }

        public MapBoolAttribute(int offset)
            : base(offset)
        {
        }

        public override int CalcSize(Type type)
        {
            return 1;
        }

        public override IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            if (type == typeof(bool))
            {
                return new BoolConverter(
                    trueValue ?? parameters.GetParameter<byte>(Parameter.TrueValue),
                    falseValue ?? parameters.GetParameter<byte>(Parameter.FalseValue));
            }

            if (type == typeof(bool?))
            {
                return new NullableBoolConverter(
                    trueValue ?? parameters.GetParameter<byte>(Parameter.TrueValue),
                    falseValue ?? parameters.GetParameter<byte>(Parameter.FalseValue),
                    nullValue ?? parameters.GetParameter<byte>(Parameter.Filler));
            }

            return null;
        }
    }
}
