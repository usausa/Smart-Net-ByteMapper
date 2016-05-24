# Work-Net-ByteMapper

Byte array object mapper.

# Example

## Object to map

```cs
private class Entity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime? Date { get; set; }

    public int? Amount { get; set; }

    public float Length { get; set; }

    public override string ToString()
    {
        return $"Id=[{Id}], Name=[{Name}], Date=[{Date}], Amount=[{Amount}], Length=[{Length}]";
    }
}
```

# Configuration

```cs
private class Profile : IMapperProfile
{
    public void Configure(IMapperConfigurationExpresion config)
    {
        config
            .DefaultEncording(Encoding.GetEncoding("Shift_JIS"))
            .DefaultFiller(0x20)
            .DefaultPadding(0x20)
            .DefaultDelimitter(new byte[] { 0x0D, 0x0A })
            .DefaultPadding(typeof(int), Padding.Left)
            .DefaultPadding(typeof(float), Padding.Left);

        config.CreateMap<Entity>(64)
            .ForMember(_ => _.Id, 5)
            .ForMember(_ => _.Name, 10)
            .ForMember(_ => _.Date, 14, c => c.DateTime("yyyyMMddHHmmss"))
            .ForMember(_ => _.Amount, 6, c => c.Padding(0x30))
            .ForMember(_ => _.Length, 6, c => c.Formatter("F2"));
    }
}

```

# Usage

```cs
// Create mapper
var builder = new MapperConfigBuilder(new Profile());
var mapper = new ByteMapper(builder.Build());

var encording = Encoding.GetEncoding("Shift_JIS");

//   _________0_________0_________0_________0_________0_________01234
var source = encording.GetBytes(
    "  666うさうさ                      123.45                     \r\n");

// bytes -> object
var entity = mapper.FromByte<Entity>(source);

// Id=[666], Name=[うさうさ], Date=[], Amount=[], Length=[123.45]
Debug.WriteLine(entity);

entity.Date = new DateTime(2015, 12, 31, 12, 23, 54);
entity.Amount = 123;
entity.Length = 999.99f;

// object -> bytes
var bytes = mapper.ToByte(entity);

// _________0_________0_________0_________0_________0_________01234
//   666うさうさ  20151231122354000123999.99                     \r\n
Debug.WriteLine(encording.GetString(bytes));
```
