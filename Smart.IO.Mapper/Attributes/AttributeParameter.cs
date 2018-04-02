namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Text;

    public static class AttributeParameter
    {
        public const string CodePage = nameof(CodePage);

        public const string EncodingName = nameof(EncodingName);

        public const string Culture = nameof(Culture);
    }

    public static class AttributeParameterHelper
    {
        public static Encoding GetEncoding(IMappingCreateContext context, int? codePage, string encodingName)
        {
            if (codePage.HasValue)
            {
                return Encoding.GetEncoding(codePage.Value);
            }

            if (!String.IsNullOrEmpty(encodingName))
            {
                return Encoding.GetEncoding(encodingName);
            }

            return context.GetParameter<Encoding>(Parameter.Encoding);
        }
    }
}
