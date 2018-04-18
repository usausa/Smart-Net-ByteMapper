# Smart.IO.ByteMapper .NET - byte mapper library for .NET

## What is this?

* byte array <-> object mapper.
* (under construction)

## Benchmark

|                  Method |       Mean |       Error |    StdDev |  Gen 0 | Allocated |
|------------------------ |-----------:|------------:|----------:|-------:|----------:|
|           ReadIntBinary |   6.342 ns |   0.6311 ns | 0.0357 ns | 0.0057 |      24 B |
|             ReadBoolean |   4.881 ns |   0.3752 ns | 0.0212 ns | 0.0057 |      24 B |
|             ReadBytes10 |  17.124 ns | 121.0408 ns | 6.8390 ns | 0.0095 |      40 B |
|             ReadBytes20 |  13.632 ns |   2.0013 ns | 0.1131 ns | 0.0114 |      48 B |
|  ReadSjisText20Single20 | 116.784 ns |  19.8037 ns | 1.1189 ns | 0.0170 |      72 B |
|     ReadSjisText20Wide5 |  80.428 ns |  12.7578 ns | 0.7208 ns | 0.0094 |      40 B |
|     ReadSjisText20Empty |  38.500 ns |   9.0957 ns | 0.5139 ns |      - |       0 B |
|      ReadNumberText8Max | 132.672 ns |   1.7227 ns | 0.0973 ns | 0.0172 |      72 B |
|     ReadNumberText8Zero |  95.364 ns |   8.0406 ns | 0.4543 ns | 0.0132 |      56 B |
|      ReadDateTimeText14 |  59.545 ns |   6.6951 ns | 0.3783 ns | 0.0132 |      56 B |

|                  Method |       Mean |       Error |    StdDev |  Gen 0 | Allocated |
|------------------------ |-----------:|------------:|----------:|-------:|----------:|
|          WriteIntBinary |   6.059 ns |   0.5068 ns | 0.0286 ns | 0.0057 |      24 B |
|            WriteBoolean |   5.395 ns |   0.5328 ns | 0.0301 ns | 0.0057 |      24 B |
|            WriteBytes10 |  12.896 ns |   0.5951 ns | 0.0336 ns |      - |       0 B |
|            WriteBytes20 |  13.246 ns |   0.9351 ns | 0.0528 ns |      - |       0 B |
| WriteSjisText20Single20 | 163.464 ns |   7.9497 ns | 0.4492 ns | 0.0112 |      48 B |
|    WriteSjisText20Wide5 |  88.934 ns |   6.7014 ns | 0.3786 ns | 0.0094 |      40 B |
|    WriteSjisText20Empty |  76.693 ns |   6.9007 ns | 0.3899 ns | 0.0132 |      56 B |
|     WriteNumberText8Max | 111.190 ns |  15.4677 ns | 0.8740 ns | 0.0267 |     112 B |
|    WriteNumberText8Zero |  89.438 ns |  17.3988 ns | 0.9831 ns | 0.0209 |      88 B |
|     WriteDateTimeText14 | 473.694 ns |  26.2385 ns | 1.4825 ns | 0.0563 |     240 B |

## Example

(under construction)
