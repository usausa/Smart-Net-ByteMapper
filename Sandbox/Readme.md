# Benchmark

## Fill

         Method |     Mean |    Error |    StdDev | Allocated |
--------------- |---------:|---------:|----------:|----------:|
    FillDefault | 40.74 ns | 5.973 ns | 0.3375 ns |       0 B |
 FillMemoryCopy | 20.13 ns | 2.189 ns | 0.1237 ns |       0 B |

## Encoding

|              Method |     Mean |     Error |    StdDev |  Gen 0 | Allocated |
|-------------------- |---------:|----------:|----------:|-------:|----------:|
|  GetBytesByEncoding | 33.52 ns | 4.9345 ns | 0.2788 ns | 0.0095 |      40 B |
|    GetBytesByCustom | 12.18 ns | 0.8216 ns | 0.0464 ns | 0.0095 |      40 B |
| GetStringByEncoding | 28.23 ns | 2.5924 ns | 0.1465 ns | 0.0114 |      48 B |
|   GetStringByCustom | 15.24 ns | 0.7134 ns | 0.0403 ns | 0.0114 |      48 B |

## DateTime

(TODO)

## Integer

(TODO)

## Decimal

(TODO)

## Div10

|    Method |     Mean |     Error |    StdDev | Allocated |
|---------- |---------:|----------:|----------:|----------:|
|     Div10 | 6.548 ns | 0.4304 ns | 0.0243 ns |       0 B |
| FastDiv10 | 4.614 ns | 0.6804 ns | 0.0384 ns |       0 B |

* `FastDiv10` is signed only
