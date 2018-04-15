# Benchmark

## DateTime

### Parse

|       Method |      Mean |    Error |    StdDev |  Gen 0 | Allocated |
|------------- |----------:|---------:|----------:|-------:|----------:|
| ParseDefault | 397.22 ns | 59.95 ns | 3.3875 ns | 0.0129 |      56 B |
|  ParseCustom |  56.97 ns | 13.66 ns | 0.7721 ns |      - |       0 B |

### Format

(TODO)

## Integer

### Parse

(TODO)

### Format

(TODO)

## Decimal

### Parse

(TODO)

### Format

(TODO)

## Fill

|           Method |     Mean |     Error |    StdDev | Allocated |
|----------------- |---------:|----------:|----------:|----------:|
|           Fill32 | 13.98 ns | 13.868 ns | 0.7836 ns |       0 B |
|     FillUnsafe32 | 15.31 ns |  1.360 ns | 0.0768 ns |       0 B |
| FillMemoryCopy32 | 22.94 ns |  6.249 ns | 0.3531 ns |       0 B |
|           Fill64 | 38.54 ns | 19.524 ns | 1.1031 ns |       0 B |
|     FillUnsafe64 | 37.44 ns |  2.674 ns | 0.1511 ns |       0 B |
| FillMemoryCopy64 | 29.01 ns |  6.686 ns | 0.3778 ns |       0 B |

## Encoding

|              Method |     Mean |     Error |    StdDev |  Gen 0 | Allocated |
|-------------------- |---------:|----------:|----------:|-------:|----------:|
|  GetBytesByEncoding | 33.52 ns | 4.9345 ns | 0.2788 ns | 0.0095 |      40 B |
|    GetBytesByCustom | 12.18 ns | 0.8216 ns | 0.0464 ns | 0.0095 |      40 B |
| GetStringByEncoding | 28.23 ns | 2.5924 ns | 0.1465 ns | 0.0114 |      48 B |
|   GetStringByCustom | 15.24 ns | 0.7134 ns | 0.0403 ns | 0.0114 |      48 B |

## Div10

|    Method |     Mean |     Error |    StdDev | Allocated |
|---------- |---------:|----------:|----------:|----------:|
|     Div10 | 6.548 ns | 0.4304 ns | 0.0243 ns |       0 B |
| FastDiv10 | 4.614 ns | 0.6804 ns | 0.0384 ns |       0 B |

* `FastDiv10` is signed only
