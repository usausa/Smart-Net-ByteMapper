# Benchmark

## Fill

|         Method |     Mean |    Error |    StdDev | Allocated |
|--------------- |---------:|---------:|----------:|----------:|
|    FillDefault | 40.16 ns | 6.283 ns | 0.3550 ns |       0 B |
| FillMemoryCopy | 27.10 ns | 3.712 ns | 0.2097 ns |       0 B |

## Encoding

|              Method |     Mean |      Error |     StdDev |  Gen 0 | Allocated |
|-------------------- |---------:|-----------:|-----------:|-------:|----------:|
|  GetBytesByEncoding | 33.84 ns |  28.007 ns |  1.5825 ns | 0.0095 |      40 B |
|    GetBytesByCustom | 11.77 ns |   3.049 ns |  0.1723 ns | 0.0095 |      40 B |
| GetStringByEncoding | 33.26 ns | 177.698 ns | 10.0403 ns | 0.0114 |      48 B |
|   GetStringByCustom | 31.22 ns |   4.956 ns |  0.2800 ns | 0.0114 |      48 B |
|  GetStringByCustom2 | 22.60 ns |   2.537 ns |  0.1433 ns | 0.0229 |      96 B |

* `GetStringByCustom` stackallock is slow ?

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
