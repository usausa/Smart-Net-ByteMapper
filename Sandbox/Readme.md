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

## Decimal

### Parse

|          Method |      Mean |      Error |    StdDev |  Gen 0 | Allocated |
|---------------- |----------:|-----------:|----------:|-------:|----------:|
|   ParseDefault8 | 239.74 ns | 65.0942 ns | 3.6779 ns | 0.0110 |      48 B |
|    ParseCustom8 |  77.33 ns |  3.7454 ns | 0.2116 ns |      - |       0 B |
|   ParseCustomB8 |  19.45 ns |  2.1832 ns | 0.1234 ns |      - |       0 B |
|  ParseDefault19 | 450.23 ns | 33.1618 ns | 1.8737 ns | 0.0167 |      72 B |
|   ParseCustom19 | 167.81 ns | 22.7759 ns | 1.2869 ns |      - |       0 B |
|  ParseCustomB19 |  32.05 ns |  0.6701 ns | 0.0379 ns |      - |       0 B |
|  ParseDefault28 | 647.23 ns | 20.9874 ns | 1.1858 ns | 0.0200 |      88 B |
|   ParseCustom28 | 241.74 ns | 17.1769 ns | 0.9705 ns |      - |       0 B |

### Format

|          Method |      Mean |      Error |    StdDev |  Gen 0 | Allocated |
|---------------- |----------:|-----------:|----------:|-------:|----------:|
|  FormatDefault8 | 223.44 ns | 16.9187 ns | 0.9559 ns | 0.0207 |      88 B |
|  FormatCustomB8 | 132.84 ns | 15.7912 ns | 0.8922 ns | 0.0188 |      80 B |
| FormatDefault19 | 335.74 ns | 28.0104 ns | 1.5826 ns | 0.0281 |     120 B |
| FormatCustomB19 | 289.07 ns | 17.2724 ns | 0.9759 ns | 0.0205 |      88 B |

## Fill

|           Method |     Mean |     Error |    StdDev | Allocated |
|----------------- |---------:|----------:|----------:|----------:|
|           Fill32 | 11.52 ns | 0.6409 ns | 0.0362 ns |       0 B |
|     FillUnsafe32 | 15.20 ns | 0.8971 ns | 0.0507 ns |       0 B |
| FillMemoryCopy32 | 21.53 ns | 2.1351 ns | 0.1206 ns |       0 B |
|           Fill64 | 40.24 ns | 1.2358 ns | 0.0698 ns |       0 B |
|     FillUnsafe64 | 36.76 ns | 3.9663 ns | 0.2241 ns |       0 B |
| FillMemoryCopy64 | 26.95 ns | 1.1605 ns | 0.0656 ns |       0 B |

## Encoding

|              Method |     Mean |    Error |    StdDev |  Gen 0 | Allocated |
|-------------------- |---------:|---------:|----------:|-------:|----------:|
|  GetBytesByEncoding | 32.48 ns | 4.998 ns | 0.2824 ns | 0.0095 |      40 B |
|    GetBytesByCustom | 11.76 ns | 1.303 ns | 0.0736 ns | 0.0095 |      40 B |
| GetStringByEncoding | 27.19 ns | 2.138 ns | 0.1208 ns | 0.0114 |      48 B |
|   GetStringByCustom | 14.46 ns | 1.997 ns | 0.1129 ns | 0.0114 |      48 B |

## Div10

|    Method |     Mean |     Error |    StdDev | Allocated |
|---------- |---------:|----------:|----------:|----------:|
|     Div10 | 6.366 ns | 0.4650 ns | 0.0263 ns |       0 B |
| FastDiv10 | 4.569 ns | 0.4567 ns | 0.0258 ns |       0 B |

* `FastDiv10` is signed only
