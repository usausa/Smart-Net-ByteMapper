namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public static class AttributeParameter
    {
        public const string CodePage = nameof(CodePage);

        public const string EncodingName = nameof(EncodingName);

        public const string Culture = nameof(Culture);
    }

    public static class AttributeParameterHelper
    {
        public static Encoding GetEncoding(IMappingParameter parameters, int? codePage, string encodingName)
        {
            if (codePage.HasValue)
            {
                return Encoding.GetEncoding(codePage.Value);
            }

            if (!String.IsNullOrEmpty(encodingName))
            {
                return Encoding.GetEncoding(encodingName);
            }

            return parameters.GetParameter<Encoding>(Parameter.Encoding);
        }

        public static IFormatProvider GetProvider(IMappingParameter parameters, Culture? culture)
        {
            if (culture.HasValue)
            {
                return culture.Value.ToCultureInfo();
            }

            return parameters.GetParameter<IFormatProvider>(Parameter.Culture);
        }
    }
}
