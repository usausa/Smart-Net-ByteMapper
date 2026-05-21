using System.Text;

using Example;
using Example.Web.MinimalApi;

using Smart.IO.ByteMapper.AspNetCore;
using Smart.IO.ByteMapper.AspNetCore.Formatters;

#pragma warning disable CA1852

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// Build the registry using the source-generated bootstrap
var registry = __ByteMapperAspNetCoreBootstrap.Build();
var formatterOptions = new ByteMapperFormatterOptions();
formatterOptions.SupportedMediaTypes.Add("text/x-fixedrecord");

builder.Services.AddSingleton(registry);
builder.Services.AddSingleton(formatterOptions);

builder.Services.AddControllers(options =>
{
    options.OutputFormatters.Add(new ByteMapperOutputFormatter(registry, formatterOptions));
    options.InputFormatters.Add(new ByteMapperInputFormatter(registry, formatterOptions));
    options.FormatterMappings.SetMediaTypeMappingForFormat("dat", "text/x-fixedrecord");
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

app.MapSampleEndpoints(registry);

app.Run();
