using System.Text;

using Example;
using Example.Web.MinimalApi;

#pragma warning disable CA1852

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddByteMapperFormatters(o =>
{
    o.SupportedMediaTypes.Add("text/x-fixedrecord");
});

builder.Services.AddControllers();

// Insert ByteMapper formatters into MVC options via DI
builder.Services.AddOptions<Microsoft.AspNetCore.Mvc.MvcOptions>()
    .Configure<Smart.IO.ByteMapper.AspNetCore.Formatters.ByteMapperOutputFormatter,
               Smart.IO.ByteMapper.AspNetCore.Formatters.ByteMapperInputFormatter>(
        (mvc, output, input) =>
        {
            mvc.OutputFormatters.Insert(0, output);
            mvc.InputFormatters.Insert(0, input);
            mvc.FormatterMappings.SetMediaTypeMappingForFormat("dat", "text/x-fixedrecord");
        });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapSampleEndpoints();

app.Run();
