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
|   ParseDefault8 | 240.82 ns | 120.849 ns | 6.8282 ns | 0.0110 |      48 B |
|    ParseCustom8 |  79.71 ns |  10.897 ns | 0.6157 ns |      - |       0 B |
|   ParseCustomB8 |  19.92 ns |   8.618 ns | 0.4870 ns |      - |       0 B |
|  ParseDefault19 | 452.73 ns |  49.782 ns | 2.8127 ns | 0.0167 |      72 B |
|   ParseCustom19 | 174.50 ns |  36.478 ns | 2.0611 ns |      - |       0 B |
|  ParseCustomB19 |  31.99 ns |   3.371 ns | 0.1904 ns |      - |       0 B |
|  ParseDefault28 | 653.13 ns |  63.025 ns | 3.5610 ns | 0.0200 |      88 B |
|   ParseCustom28 | 251.08 ns |  27.184 ns | 1.5359 ns |      - |       0 B |

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
