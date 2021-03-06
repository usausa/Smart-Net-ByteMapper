# Benchmark

## DateTime

### Parse

|        Method |      Mean |     Error |    StdDev |  Gen 0 | Allocated |
|-------------- |----------:|----------:|----------:|-------:|----------:|
|  ParseDefault | 379.74 ns | 57.056 ns | 3.2238 ns | 0.0129 |      56 B |
|   ParseCustom |  51.18 ns |  3.987 ns | 0.2253 ns |      - |       0 B |

### Format

|        Method |      Mean |     Error |    StdDev |  Gen 0 | Allocated |
|-------------- |----------:|----------:|----------:|-------:|----------:|
| FormatDefault | 598.85 ns | 52.007 ns | 2.9385 ns | 0.0582 |     248 B |
|  FormatCustom | 108.48 ns |  5.897 ns | 0.3332 ns | 0.0113 |      48 B |

----

## Integer

### Parse

|        Method |      Mean |      Error |    StdDev |  Gen 0 | Allocated |
|-------------- |----------:|-----------:|----------:|-------:|----------:|
|  ParseDefault | 125.24 ns | 26.1181 ns | 1.4757 ns | 0.0112 |      48 B |
|   ParseCustom |  16.07 ns |  1.7261 ns | 0.0975 ns |      - |       0 B |

### Format

|        Method |      Mean |      Error |    StdDev |  Gen 0 | Allocated |
|-------------- |----------:|-----------:|----------:|-------:|----------:|
| FormatDefault | 111.20 ns | 11.0991 ns | 0.6271 ns | 0.0190 |      80 B |
|  FormatCustom |  19.83 ns |  0.1357 ns | 0.0077 ns | 0.0076 |      32 B |

----

## Long

### Parse

|          Method |      Mean |      Error |    StdDev |  Gen 0 | Allocated |
|---------------- |----------:|-----------:|----------:|-------:|----------:|
|   ParseDefault8 | 139.90 ns | 162.509 ns | 9.1821 ns | 0.0112 |      48 B |
|    ParseCustom8 |  16.47 ns |   2.993 ns | 0.1691 ns |      - |       0 B |
|  ParseDefault19 | 197.16 ns |  23.340 ns | 1.3187 ns | 0.0150 |      64 B |
|   ParseCustom19 |  35.65 ns |   2.222 ns | 0.1255 ns |      - |       0 B |

### Format

|          Method |      Mean |      Error |    StdDev |  Gen 0 | Allocated |
|---------------- |----------:|-----------:|----------:|-------:|----------:|
|  FormatDefault8 | 117.30 ns |  32.805 ns | 1.8535 ns | 0.0190 |      80 B |
|   FormatCustom8 |  20.70 ns |   2.589 ns | 0.1463 ns | 0.0076 |      32 B |
| FormatDefault19 | 165.52 ns |   9.062 ns | 0.5120 ns | 0.0265 |     112 B |
|  FormatCustom19 |  42.28 ns |   3.881 ns | 0.2193 ns | 0.0114 |      48 B |

----

## Decimal

### Parse

|          Method |      Mean |      Error |    StdDev |  Gen 0 | Allocated |
|---------------- |----------:|-----------:|----------:|-------:|----------:|
|   ParseDefault8 | 247.12 ns |  47.502 ns | 2.6840 ns | 0.0110 |      48 B |
|    ParseCustom8 |  82.14 ns |   5.824 ns | 0.3290 ns |      - |       0 B |
|   ParseCustom8X |  25.73 ns |   3.392 ns | 0.1916 ns |      - |       0 B |
|   ParseCustomB8 |  25.72 ns | 144.815 ns | 8.1823 ns |      - |       0 B |
|  ParseDefault19 | 454.80 ns |  20.927 ns | 1.1824 ns | 0.0167 |      72 B |
|   ParseCustom19 | 177.24 ns |  25.952 ns | 1.4664 ns |      - |       0 B |
|  ParseCustom19X |  52.56 ns |   5.046 ns | 0.2851 ns |      - |       0 B |
|  ParseCustomB19 |  33.46 ns |   6.144 ns | 0.3471 ns |      - |       0 B |
|  ParseDefault28 | 656.71 ns |  29.138 ns | 1.6463 ns | 0.0200 |      88 B |
|   ParseCustom28 | 258.03 ns |  41.224 ns | 2.3293 ns |      - |       0 B |
|  ParseCustom28X |  83.47 ns |   2.200 ns | 0.1243 ns |      - |       0 B |

### Format

|          Method |      Mean |      Error |    StdDev |  Gen 0 | Allocated |
|---------------- |----------:|-----------:|----------:|-------:|----------:|
|  FormatDefault8 | 211.54 ns |   7.305 ns | 0.4128 ns | 0.0207 |      88 B |
|  FormatCustomB8 |  79.41 ns |  10.691 ns | 0.6040 ns | 0.0190 |      80 B |
|  FormatCustomC8 |  77.29 ns |   9.103 ns | 0.5143 ns | 0.0190 |      80 B |
| FormatDefault19 | 343.12 ns |  36.753 ns | 2.0766 ns | 0.0281 |     120 B |
| FormatCustomB19 | 240.63 ns |  21.837 ns | 1.2338 ns | 0.0205 |      88 B |
| FormatDefault28 | 466.52 ns |  14.816 ns | 0.8371 ns | 0.0343 |     144 B |

----

## Fill

|           Method |     Mean |     Error |    StdDev | Allocated |
|----------------- |---------:|----------:|----------:|----------:|
|           Fill32 | 11.52 ns | 0.6409 ns | 0.0362 ns |       0 B |
|     FillUnsafe32 | 15.20 ns | 0.8971 ns | 0.0507 ns |       0 B |
| FillMemoryCopy32 | 21.53 ns | 2.1351 ns | 0.1206 ns |       0 B |
|           Fill64 | 40.24 ns | 1.2358 ns | 0.0698 ns |       0 B |
|     FillUnsafe64 | 36.76 ns | 3.9663 ns | 0.2241 ns |       0 B |
| FillMemoryCopy64 | 26.95 ns | 1.1605 ns | 0.0656 ns |       0 B |

----

## Reverse

|          Method |     Mean |     Error |    StdDev | Allocated |
|---------------- |---------:|----------:|----------:|----------:|
|        Reverse4 | 2.840 ns | 0.6022 ns | 0.0340 ns |       0 B |
|  ReverseUnsafe4 | 2.967 ns | 0.4509 ns | 0.0255 ns |       0 B |
|        Reverse8 | 6.564 ns | 0.6297 ns | 0.0356 ns |       0 B |
|  ReverseUnsafe8 | 6.183 ns | 0.4885 ns | 0.0276 ns |       0 B |
|       Reverse16 | 9.556 ns | 0.2764 ns | 0.0156 ns |       0 B |
| ReverseUnsafe16 | 8.568 ns | 1.2655 ns | 0.0715 ns |       0 B |

----

## Bit

|            Method |     Mean |     Error |    StdDev | Allocated |
|------------------ |---------:|----------:|----------:|----------:|
|   FloatToUIntBit1 | 5.534 ns | 3.9423 ns | 0.2227 ns |       0 B |
|   FloatToUIntBit2 | 1.793 ns | 1.5540 ns | 0.0878 ns |       0 B |
|   UIntToFloatBit1 | 5.433 ns | 1.2301 ns | 0.0695 ns |       0 B |
|   UIntToFloatBit2 | 1.734 ns | 0.9628 ns | 0.0544 ns |       0 B |
| DoubleToULongBit1 | 5.068 ns | 2.7036 ns | 0.1528 ns |       0 B |
| DoubleToULongBit2 | 1.703 ns | 0.7677 ns | 0.0434 ns |       0 B |
| ULongToDoubleBit1 | 4.863 ns | 0.0807 ns | 0.0046 ns |       0 B |
| ULongToDoubleBit2 | 3.597 ns | 2.2669 ns | 0.1281 ns |       0 B |

----

## Encoding

|              Method |     Mean |    Error |    StdDev |  Gen 0 | Allocated |
|-------------------- |---------:|---------:|----------:|-------:|----------:|
|  GetBytesByEncoding | 32.48 ns | 4.998 ns | 0.2824 ns | 0.0095 |      40 B |
|    GetBytesByCustom | 11.76 ns | 1.303 ns | 0.0736 ns | 0.0095 |      40 B |
| GetStringByEncoding | 27.19 ns | 2.138 ns | 0.1208 ns | 0.0114 |      48 B |
|   GetStringByCustom | 14.46 ns | 1.997 ns | 0.1129 ns | 0.0114 |      48 B |

----

## Div10

|    Method |     Mean |     Error |    StdDev | Allocated |
|---------- |---------:|----------:|----------:|----------:|
|     Div10 | 6.366 ns | 0.4650 ns | 0.0263 ns |       0 B |
| FastDiv10 | 4.569 ns | 0.4567 ns | 0.0258 ns |       0 B |

* `FastDiv10` is signed only
