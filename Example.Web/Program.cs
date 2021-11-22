using System.Text;

using Example.Web.Models;

using Smart.AspNetCore.Formatters;
using Smart.IO.ByteMapper;

#pragma warning disable CA1812
var builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var mapperFactory = new MapperFactoryConfig()
    .UseOptionsDefault()
    .DefaultEncoding(Encoding.GetEncoding("Shift_JIS"))
    .CreateMapByExpression<SampleData>(59, c => c
        .ForMember(x => x.Code, m => m.Ascii(13))
        .ForMember(x => x.Name, m => m.Text(20))
        .ForMember(x => x.Qty, m => m.Integer(6))
        .ForMember(x => x.Price, m => m.Decimal(10, 2))
        .ForMember(x => x.Date, m => m.DateTime("yyyyMMdd")))
    .CreateMapByExpression<SampleData>("short", 35, c => c
        .ForMember(x => x.Code, m => m.Ascii(13))
        .ForMember(x => x.Name, m => m.Text(20)))
    .ToMapperFactory();

builder.Services.AddControllers(options =>
{
    var config = new ByteMapperFormatterConfig { MapperFactory = mapperFactory };
    config.SupportedMediaTypes.Add("text/x-fixedrecord");

    options.OutputFormatters.Add(new ByteMapperOutputFormatter(config));
    options.InputFormatters.Add(new ByteMapperInputFormatter(config));

    options.FormatterMappings.SetMediaTypeMappingForFormat("dat", "text/x-fixedrecord");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
