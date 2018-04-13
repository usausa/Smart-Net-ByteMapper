# Smart.IO.ByteMapper .NET - byte mapper library for .NET

## What is this?

* byte array <-> object mapper.
* (under construction)

## Benchmark

|                    Method |       Mean |       Error |     StdDev |  Gen 0 | Allocated |
|-------------------------- |-----------:|------------:|-----------:|-------:|----------:|
|             ReadIntBinary |   6.360 ns |   0.1562 ns |  0.0088 ns | 0.0057 |      24 B |
|               ReadBoolean |   5.086 ns |   0.7016 ns |  0.0396 ns | 0.0057 |      24 B |
|               ReadBytes10 |  13.900 ns |   5.8754 ns |  0.3320 ns | 0.0095 |      40 B |
|               ReadBytes20 |  14.083 ns |   0.8887 ns |  0.0502 ns | 0.0114 |      48 B |
|        ReadText20Single20 | 119.885 ns |  57.0974 ns |  3.2261 ns | 0.0169 |      72 B |
|           ReadText20Wide5 |  80.149 ns |  12.1197 ns |  0.6848 ns | 0.0094 |      40 B |
|           ReadText20Empty |  38.531 ns |  10.8760 ns |  0.6145 ns |      - |       0 B |
|        ReadNumberText8Max | 189.934 ns |  14.0460 ns |  0.7936 ns | 0.0172 |      72 B |
|       ReadNumberText8Zero | 133.377 ns |  15.4912 ns |  0.8753 ns | 0.0131 |      56 B |
|   ReadNumberText8AsciiMax | 132.137 ns |  13.0129 ns |  0.7353 ns | 0.0172 |      72 B |
|  ReadNumberText8AsciiZero |  93.624 ns |  11.9156 ns |  0.6733 ns | 0.0132 |      56 B |
|        ReadDateTimeText14 | 126.926 ns |  23.3992 ns |  1.3221 ns | 0.0131 |      56 B |
|   ReadDateTimeText14Ascii |  58.983 ns |   1.0457 ns |  0.0591 ns | 0.0132 |      56 B |

|                    Method |       Mean |       Error |     StdDev |  Gen 0 | Allocated |
|-------------------------- |-----------:|------------:|-----------:|-------:|----------:|
|            WriteIntBinary |   6.575 ns |   0.4709 ns |  0.0266 ns | 0.0057 |      24 B |
|              WriteBoolean |   5.576 ns |   1.5412 ns |  0.0871 ns | 0.0057 |      24 B |
|              WriteBytes10 |  12.951 ns |   0.6523 ns |  0.0369 ns |      - |       0 B |
|              WriteBytes20 |  12.952 ns |   3.0936 ns |  0.1748 ns |      - |       0 B |
|       WriteText20Single20 | 162.456 ns |  27.7574 ns |  1.5683 ns | 0.0112 |      48 B |
|          WriteText20Wide5 | 123.685 ns |   4.7248 ns |  0.2670 ns | 0.0093 |      40 B |
|          WriteText20Empty | 106.518 ns |   4.4898 ns |  0.2537 ns | 0.0132 |      56 B |
|       WriteNumberText8Max | 176.154 ns |  20.8750 ns |  1.1795 ns | 0.0267 |     112 B |
|      WriteNumberText8Zero | 147.123 ns |  10.2245 ns |  0.5777 ns | 0.0207 |      88 B |
|  WriteNumberText8AsciiMax | 108.690 ns |  14.6154 ns |  0.8258 ns | 0.0267 |     112 B |
| WriteNumberText8AsciiZero | 113.292 ns |   3.3755 ns |  0.1907 ns | 0.0209 |      88 B |
|       WriteDateTimeText14 | 602.008 ns | 231.9666 ns | 13.1065 ns | 0.0563 |     240 B |
|  WriteDateTimeText14Ascii | 477.888 ns |  45.8598 ns |  2.5912 ns | 0.0563 |     240 B |


## Example

(under construction)
