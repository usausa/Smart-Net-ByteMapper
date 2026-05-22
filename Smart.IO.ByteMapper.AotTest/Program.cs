using Smart.IO.ByteMapper.AotTest.Mappers;
using Smart.IO.ByteMapper.AotTest.Models;
using Smart.IO.ByteMapper.Converters;

Console.WriteLine("=== Smart.IO.ByteMapper AOT Test ===");

// --- Core: ByteMapper read/write ---
var record = new SampleRecord
{
    Code = "ITEM0001",
    Name = "Test Product",
    Qty = 100,
    Flag = true,
    Value = 12345
};

var buffer = SampleRecordMappers.WriteAlloc(record);
Console.WriteLine($"Written {buffer.Length} bytes");

var readBack = SampleRecordMappers.ReadNew(buffer);
Console.WriteLine($"Code={readBack.Code}, Name={readBack.Name}, Qty={readBack.Qty}, Flag={readBack.Flag}, Value={readBack.Value}");

if (readBack.Code != record.Code)
{
    Console.Error.WriteLine($"Code mismatch: {readBack.Code}");
    return;
}

if (readBack.Name.Trim() != record.Name.Trim())
{
    Console.Error.WriteLine($"Name mismatch: {readBack.Name}");
    return;
}

if (readBack.Qty != record.Qty)
{
    Console.Error.WriteLine($"Qty mismatch: {readBack.Qty}");
    return;
}

if (readBack.Flag != record.Flag)
{
    Console.Error.WriteLine($"Flag mismatch: {readBack.Flag}");
    return;
}

if (readBack.Value != record.Value)
{
    Console.Error.WriteLine($"Value mismatch: {readBack.Value}");
    return;
}

// --- Fast converters ---
// FastIntegerConverter<int>
var intConverter = new FastIntegerConverter<int>(10);
Span<byte> intBuf = stackalloc byte[10];
intBuf.Fill(0x20);
intConverter.Write(intBuf, 42);
var intResult = intConverter.Read(intBuf);
Console.WriteLine($"FastIntegerConverter: {intResult}");
if (intResult != 42)
{
    Console.Error.WriteLine($"FastIntegerConverter mismatch: {intResult}");
    return;
}

// FastDecimalConverter
var decConverter = new FastDecimalConverter(12, scale: 2);
Span<byte> decBuf = stackalloc byte[12];
decBuf.Fill(0x20);
decConverter.Write(decBuf, 123.45m);
var decResult = decConverter.Read(decBuf);
Console.WriteLine($"FastDecimalConverter: {decResult}");
if (decResult != 123.45m)
{
    Console.Error.WriteLine($"FastDecimalConverter mismatch: {decResult}");
    return;
}

// FastDateTimeConverter
var dtConverter = new FastDateTimeConverter("yyyyMMddHHmmss");
Span<byte> dtBuf = stackalloc byte[14];
var dt = new DateTime(2025, 1, 15, 10, 30, 0);
dtConverter.Write(dtBuf, dt);
var dtResult = dtConverter.Read(dtBuf);
Console.WriteLine($"FastDateTimeConverter: {dtResult}");
if (dtResult != dt)
{
    Console.Error.WriteLine($"FastDateTimeConverter mismatch: {dtResult}");
    return;
}

// FastUnicodeConverter
var unicodeConverter = new FastUnicodeConverter(20);
Span<byte> unicodeBuf = stackalloc byte[20];
unicodeBuf.Fill(0x20);
unicodeConverter.Write(unicodeBuf, "Hello");
var unicodeResult = unicodeConverter.Read(unicodeBuf);
Console.WriteLine($"FastUnicodeConverter: {unicodeResult}");
if (unicodeResult != "Hello")
{
    Console.Error.WriteLine($"FastUnicodeConverter mismatch: {unicodeResult}");
    return;
}

Console.WriteLine("=== All tests passed ===");
