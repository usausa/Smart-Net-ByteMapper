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

|         Method |      Mean |     Error |    StdDev |  Gen 0 | Allocated |
|--------------- |----------:|----------:|----------:|-------:|----------:|
|  ParseDefault8 | 236.64 ns | 32.761 ns | 1.8511 ns | 0.0112 |      48 B |
|   ParseCustom8 |  77.40 ns |  7.044 ns | 0.3980 ns |      - |       0 B |
| ParseDefault28 | 644.46 ns | 25.973 ns | 1.4675 ns | 0.0200 |      88 B |
|  ParseCustom28 | 247.61 ns | 19.769 ns | 1.1170 ns |      - |       0 B |

### Format

(TODO)

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
