# Smart.IO.ByteMapper .NET - byte mapper library for .NET

## What is this?

* byte array <-> object mapper.
* (under construction)

## Benchmark

|                  Method |       Mean |      Error |    StdDev |  Gen 0 | Allocated |
|------------------------ |-----------:|-----------:|----------:|-------:|----------:|
|           ReadIntBinary |   6.278 ns |  1.9699 ns | 0.1113 ns | 0.0057 |      24 B |
|             ReadBoolean |   5.157 ns |  7.5270 ns | 0.4253 ns | 0.0057 |      24 B |
|             ReadBytes10 |  14.012 ns |  8.8542 ns | 0.5003 ns | 0.0095 |      40 B |
|             ReadBytes20 |  13.482 ns |  1.8411 ns | 0.1040 ns | 0.0114 |      48 B |
|  ReadSjisText20Single20 | 116.062 ns |  6.2825 ns | 0.3550 ns | 0.0170 |      72 B |
|     ReadSjisText20Wide5 |  78.478 ns | 13.5375 ns | 0.7649 ns | 0.0094 |      40 B |
|     ReadSjisText20Empty |  38.207 ns | 20.7128 ns | 1.1703 ns |      - |       0 B |
|      ReadNumberText8Max | 139.799 ns | 56.9016 ns | 3.2150 ns | 0.0172 |      72 B |
|     ReadNumberText8Zero |  96.037 ns |  8.8517 ns | 0.5001 ns | 0.0132 |      56 B |
|      ReadDateTimeText14 |  59.814 ns | 21.1029 ns | 1.1924 ns | 0.0132 |      56 B |

|                  Method |       Mean |      Error |    StdDev |  Gen 0 | Allocated |
|------------------------ |-----------:|-----------:|----------:|-------:|----------:|
|          WriteIntBinary |   5.960 ns |  2.2335 ns | 0.1262 ns | 0.0057 |      24 B |
|            WriteBoolean |   5.405 ns |  0.7600 ns | 0.0429 ns | 0.0057 |      24 B |
|            WriteBytes10 |  12.564 ns |  0.8459 ns | 0.0478 ns |      - |       0 B |
|            WriteBytes20 |  12.823 ns |  0.4328 ns | 0.0245 ns |      - |       0 B |
| WriteSjisText20Single20 | 159.840 ns | 26.5105 ns | 1.4979 ns | 0.0112 |      48 B |
|    WriteSjisText20Wide5 | 121.420 ns |  8.6398 ns | 0.4882 ns | 0.0093 |      40 B |
|    WriteSjisText20Empty | 107.029 ns | 10.9104 ns | 0.6165 ns | 0.0132 |      56 B |
|     WriteNumberText8Max | 106.831 ns |  7.0948 ns | 0.4009 ns | 0.0267 |     112 B |
|    WriteNumberText8Zero | 114.038 ns | 17.7104 ns | 1.0007 ns | 0.0209 |      88 B |
|     WriteDateTimeText14 | 471.111 ns | 11.7860 ns | 0.6659 ns | 0.0563 |     240 B |

## Example

(under construction)
