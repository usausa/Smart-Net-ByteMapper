# Smart.IO.ByteMapper .NET - byte mapper library for .NET

## What is this?

* byte array <-> object mapper.
* (under construction)

## Benchmark

|                       Method |       Mean |       Error |     StdDev |  Gen 0 | Allocated |
|----------------------------- |-----------:|------------:|-----------:|-------:|----------:|
|                ReadIntBinary |   6.308 ns |   1.5587 ns |  0.0881 ns | 0.0057 |      24 B |
|                  ReadBoolean |   4.864 ns |   0.1343 ns |  0.0076 ns | 0.0057 |      24 B |
|                  ReadBytes10 |  13.378 ns |   3.1810 ns |  0.1797 ns | 0.0095 |      40 B |
|                  ReadBytes20 |  13.777 ns |   1.6255 ns |  0.0918 ns | 0.0114 |      48 B |
|       ReadSjisText20Single20 | 114.122 ns |   3.9026 ns |  0.2205 ns | 0.0170 |      72 B |
|          ReadSjisText20Wide5 |  81.905 ns |   5.9850 ns |  0.3382 ns | 0.0094 |      40 B |
|          ReadSjisText20Empty |  36.883 ns |  17.1153 ns |  0.9670 ns |      - |       0 B |
|     ReadNumberTextShort4Zero |  94.412 ns |   5.1714 ns |  0.2922 ns | 0.0132 |      56 B |
|      ReadNumberTextShort4Max | 110.853 ns |  13.0244 ns |  0.7359 ns | 0.0151 |      64 B |
|       ReadNumberTextInt8Zero |  93.061 ns |   4.1243 ns |  0.2330 ns | 0.0132 |      56 B |
|        ReadNumberTextInt8Max | 132.318 ns |   6.7653 ns |  0.3823 ns | 0.0172 |      72 B |
|     ReadNumberTextLong18Zero |  98.981 ns |   6.6714 ns |  0.3769 ns | 0.0132 |      56 B |
|      ReadNumberTextLong18Max | 198.732 ns |  27.3234 ns |  1.5438 ns | 0.0207 |      88 B |
|   ReadNumberTextDecimal8Zero | 111.449 ns |   7.9690 ns |  0.4503 ns | 0.0151 |      64 B |
|    ReadNumberTextDecimal8Max | 218.369 ns |  12.7117 ns |  0.7182 ns | 0.0188 |      80 B |
|  ReadNumberTextDecimal18Zero | 124.317 ns |   8.0663 ns |  0.4558 ns | 0.0169 |      72 B |
|   ReadNumberTextDecimal18Max | 409.370 ns |  35.1513 ns |  1.9861 ns | 0.0224 |      96 B |
|            ReadDateTimeText8 | 257.702 ns |   2.7524 ns |  0.1555 ns | 0.0172 |      72 B |
|           ReadDateTimeText14 | 384.261 ns | 256.6164 ns | 14.4993 ns | 0.0186 |      80 B |
|           ReadDateTimeText17 | 458.880 ns |   9.8543 ns |  0.5568 ns | 0.0205 |      88 B |
|               ReadText13Code |  29.843 ns |   2.6018 ns |  0.1470 ns | 0.0133 |      56 B |
|              ReadAscii13Code |  17.813 ns |   0.9506 ns |  0.0537 ns | 0.0133 |      56 B |
|        ReadIntegerShort4Zero |  11.834 ns |   0.7269 ns |  0.0411 ns | 0.0057 |      24 B |
|         ReadIntegerShort4Max |  13.894 ns |   1.2671 ns |  0.0716 ns | 0.0057 |      24 B |
|             ReadInteger8Zero |  12.544 ns |   0.7409 ns |  0.0419 ns | 0.0057 |      24 B |
|              ReadInteger8Max |  18.477 ns |   0.5558 ns |  0.0314 ns | 0.0057 |      24 B |
|               ReadLong18Zero |  17.548 ns |   0.3739 ns |  0.0211 ns | 0.0057 |      24 B |
|                ReadLong18Max |  28.339 ns |   2.2402 ns |  0.1266 ns | 0.0057 |      24 B |
|             ReadDecimal8Zero |  21.939 ns |   3.0054 ns |  0.1698 ns | 0.0076 |      32 B |
|              ReadDecimal8Max |  24.707 ns |   7.3059 ns |  0.4128 ns | 0.0076 |      32 B |
|            ReadDecimal18Zero |  26.653 ns |   0.9961 ns |  0.0563 ns | 0.0076 |      32 B |
|             ReadDecimal18Max |  34.520 ns |   4.2505 ns |  0.2402 ns | 0.0076 |      32 B |
|                ReadDateTime8 |  40.689 ns |  10.8581 ns |  0.6135 ns | 0.0057 |      24 B |
|               ReadDateTime14 |  53.432 ns |   1.9964 ns |  0.1128 ns | 0.0057 |      24 B |
|               ReadDateTime17 |  66.813 ns |   2.0077 ns |  0.1134 ns | 0.0056 |      24 B |

|                       Method |       Mean |       Error |     StdDev |  Gen 0 | Allocated |
|----------------------------- |-----------:|------------:|-----------:|-------:|----------:|
|               WriteIntBinary |   6.319 ns |   0.6598 ns |  0.0373 ns | 0.0057 |      24 B |
|                 WriteBoolean |   5.558 ns |   0.5231 ns |  0.0296 ns | 0.0057 |      24 B |
|                 WriteBytes10 |  12.910 ns |   0.3403 ns |  0.0192 ns |      - |       0 B |
|                 WriteBytes20 |  12.866 ns |   0.7765 ns |  0.0439 ns |      - |       0 B |
|      WriteSjisText20Single20 | 156.843 ns |   3.6737 ns |  0.2076 ns | 0.0112 |      48 B |
|         WriteSjisText20Wide5 |  86.179 ns |  39.6817 ns |  2.2421 ns | 0.0094 |      40 B |
|         WriteSjisText20Empty |  74.409 ns |   9.0488 ns |  0.5113 ns | 0.0132 |      56 B |
|    WriteNumberTextShort4Zero |  87.239 ns |   7.2761 ns |  0.4111 ns | 0.0209 |      88 B |
|     WriteNumberTextShort4Max |  94.024 ns |   4.3919 ns |  0.2482 ns | 0.0228 |      96 B |
|      WriteNumberTextInt8Zero |  88.200 ns |   9.3780 ns |  0.5299 ns | 0.0209 |      88 B |
|       WriteNumberTextInt8Max | 104.536 ns |  12.8731 ns |  0.7274 ns | 0.0247 |     104 B |
|    WriteNumberTextLong18Zero |  96.157 ns |   7.2108 ns |  0.4074 ns | 0.0209 |      88 B |
|     WriteNumberTextLong18Max | 151.065 ns |   7.6594 ns |  0.4328 ns | 0.0322 |     136 B |
|  WriteNumberTextDecimal8Zero | 108.482 ns |   3.7677 ns |  0.2129 ns | 0.0228 |      96 B |
|   WriteNumberTextDecimal8Max | 147.024 ns |   9.8697 ns |  0.5577 ns | 0.0284 |     120 B |
| WriteNumberTextDecimal18Zero | 114.763 ns |  16.8624 ns |  0.9528 ns | 0.0228 |      96 B |
|  WriteNumberTextDecimal18Max | 209.545 ns |  22.1247 ns |  1.2501 ns | 0.0343 |     144 B |
|           WriteDateTimeText8 | 338.550 ns | 173.4578 ns |  9.8007 ns | 0.0491 |     208 B |
|          WriteDateTimeText14 | 414.802 ns |  28.1930 ns |  1.5930 ns | 0.0529 |     224 B |
|          WriteDateTimeText17 | 583.249 ns |  33.8503 ns |  1.9126 ns | 0.0639 |     272 B |
|              WriteText13Code |  46.324 ns |   1.3687 ns |  0.0773 ns | 0.0095 |      40 B |
|             WriteAscii13Code |  23.670 ns |   0.5271 ns |  0.0298 ns | 0.0095 |      40 B |
|       WriteIntegerShort4Zero |  14.454 ns |   2.0485 ns |  0.1157 ns | 0.0057 |      24 B |
|        WriteIntegerShort4Max |  18.504 ns |   0.6868 ns |  0.0388 ns | 0.0057 |      24 B |
|            WriteInteger8Zero |  15.894 ns |   2.4368 ns |  0.1377 ns | 0.0057 |      24 B |
|             WriteInteger8Max |  28.697 ns |   3.6754 ns |  0.2077 ns | 0.0057 |      24 B |
|              WriteLong18Zero |  20.933 ns |   1.3638 ns |  0.0771 ns | 0.0057 |      24 B |
|               WriteLong18Max |  51.400 ns |   3.2784 ns |  0.1852 ns | 0.0057 |      24 B |
|            WriteDecimal8Zero |  33.915 ns |   1.2151 ns |  0.0687 ns | 0.0171 |      72 B |
|             WriteDecimal8Max |  95.451 ns |   2.7782 ns |  0.1570 ns | 0.0170 |      72 B |
|           WriteDecimal18Zero |  40.238 ns |   0.9040 ns |  0.0511 ns | 0.0171 |      72 B |
|            WriteDecimal18Max | 253.925 ns |  23.7586 ns |  1.3424 ns | 0.0167 |      72 B |
|               WriteDateTime8 |  52.601 ns |   1.0960 ns |  0.0619 ns | 0.0057 |      24 B |
|              WriteDateTime14 |  76.149 ns |  14.6172 ns |  0.8259 ns | 0.0056 |      24 B |
|              WriteDateTime17 | 103.888 ns |   7.1657 ns |  0.4049 ns | 0.0056 |      24 B |

## Example

(under construction)
