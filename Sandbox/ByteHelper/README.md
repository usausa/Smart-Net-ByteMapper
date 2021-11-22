# Bit

|            Method |      Mean |     Error |    StdDev |    Median | Allocated |
|------------------ |----------:|----------:|----------:|----------:|----------:|
|   FloatToUIntBit1 | 2.1380 ns | 0.3534 ns | 0.0194 ns | 2.1490 ns |         - |
|   FloatToUIntBit2 | 0.0092 ns | 0.0879 ns | 0.0048 ns | 0.0110 ns |         - |
|   UIntToFloatBit1 | 2.1537 ns | 0.0382 ns | 0.0021 ns | 2.1526 ns |         - |
|   UIntToFloatBit2 | 0.0024 ns | 0.0754 ns | 0.0041 ns | 0.0000 ns |         - |
| DoubleToULongBit1 | 2.5191 ns | 0.0273 ns | 0.0015 ns | 2.5193 ns |         - |
| DoubleToULongBit2 | 0.0059 ns | 0.0853 ns | 0.0047 ns | 0.0039 ns |         - |
| ULongToDoubleBit1 | 2.5252 ns | 0.0887 ns | 0.0049 ns | 2.5277 ns |         - |
| ULongToDoubleBit2 | 0.6435 ns | 0.1284 ns | 0.0070 ns | 0.6399 ns |         - |

# DateTime

|          Method |     Mean |    Error |   StdDev | Allocated |
|---------------- |---------:|---------:|---------:|----------:|
|  FormatCustom8B | 16.43 ns | 0.247 ns | 0.014 ns |         - |
| FormatCustom17B | 27.94 ns | 0.461 ns | 0.025 ns |         - |

# Decimal

|          Method |      Mean |     Error |   StdDev |  Gen 0 | Allocated |
|---------------- |----------:|----------:|---------:|-------:|----------:|
|  FormatDefault8 |  80.76 ns | 15.892 ns | 0.871 ns | 0.0048 |      80 B |
| FormatDefault19 | 148.07 ns | 26.598 ns | 1.458 ns | 0.0072 |     120 B |
| FormatDefault28 | 205.00 ns | 13.493 ns | 0.740 ns | 0.0086 |     144 B |
|   FormatCustom8 |  41.84 ns |  2.376 ns | 0.130 ns | 0.0043 |      72 B |
|  FormatCustom19 |  59.25 ns |  1.749 ns | 0.096 ns | 0.0052 |      88 B |
|  FormatCustom28 |  74.65 ns |  6.278 ns | 0.344 ns | 0.0057 |      96 B |

|         Method |     Mean |    Error |   StdDev | Allocated |
|--------------- |---------:|---------:|---------:|----------:|
| ParseCustom28X | 48.69 ns | 0.868 ns | 0.048 ns |         - |

# Div

|    Method |     Mean |     Error |    StdDev | Allocated |
|---------- |---------:|----------:|----------:|----------:|
|     Div10 | 3.879 ns | 0.1308 ns | 0.0072 ns |         - |
| FastDiv10 | 1.788 ns | 0.1865 ns | 0.0102 ns |         - |

* `FastDiv10` is signed only

# Encoding

|              Method |      Mean |     Error |    StdDev |  Gen 0 | Allocated |
|-------------------- |----------:|----------:|----------:|-------:|----------:|
|  GetBytesByEncoding | 11.220 ns | 1.2373 ns | 0.0678 ns | 0.0024 |      40 B |
|    GetBytesByCustom |  5.880 ns | 0.2573 ns | 0.0141 ns | 0.0024 |      40 B |
| GetStringByEncoding | 14.453 ns | 0.4254 ns | 0.0233 ns | 0.0029 |      48 B |
|   GetStringByCustom |  7.129 ns | 0.6350 ns | 0.0348 ns | 0.0029 |      48 B |

# Fill

|           Method |      Mean |     Error |    StdDev | Allocated |
|----------------- |----------:|----------:|----------:|----------:|
|           Fill32 |  8.216 ns | 1.8160 ns | 0.0995 ns |         - |
|     FillUnsafe32 |  8.598 ns | 0.4585 ns | 0.0251 ns |         - |
| FillMemoryCopy32 | 18.885 ns | 0.4422 ns | 0.0242 ns |         - |
|           Fill64 | 14.656 ns | 0.9513 ns | 0.0521 ns |         - |
|     FillUnsafe64 | 16.087 ns | 1.7575 ns | 0.0963 ns |         - |
| FillMemoryCopy64 | 22.832 ns | 0.2066 ns | 0.0113 ns |         - |

# Integer

|        Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|-------------- |---------:|---------:|---------:|-------:|----------:|
|  ParseDefault | 23.41 ns | 0.459 ns | 0.025 ns | 0.0024 |      40 B |
|   ParseCustom | 10.18 ns | 0.361 ns | 0.020 ns |      - |         - |
| FormatDefault | 25.85 ns | 6.131 ns | 0.336 ns | 0.0043 |      72 B |
|  FormatCustom | 10.45 ns | 0.644 ns | 0.035 ns | 0.0019 |      32 B |

|     Method | Padding | ZeroFill |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|----------- |-------- |--------- |---------:|---------:|---------:|-------:|----------:|
| **FormatOld0** |    **Left** |    **False** | **12.69 ns** | **0.124 ns** | **0.007 ns** | **0.0019** |      **32 B** |
|    Format0 |    Left |    False | 12.75 ns | 1.863 ns | 0.102 ns | 0.0019 |      32 B |
| FormatOld8 |    Left |    False | 12.44 ns | 0.616 ns | 0.034 ns | 0.0019 |      32 B |
|    Format8 |    Left |    False | 12.70 ns | 0.641 ns | 0.035 ns | 0.0019 |      32 B |
| **FormatOld0** |    **Left** |     **True** | **12.92 ns** | **0.269 ns** | **0.015 ns** | **0.0019** |      **32 B** |
|    Format0 |    Left |     True | 12.50 ns | 0.240 ns | 0.013 ns | 0.0019 |      32 B |
| FormatOld8 |    Left |     True | 12.86 ns | 0.255 ns | 0.014 ns | 0.0019 |      32 B |
|    Format8 |    Left |     True | 12.74 ns | 0.466 ns | 0.026 ns | 0.0019 |      32 B |
| **FormatOld0** |   **Right** |    **False** | **14.26 ns** | **1.427 ns** | **0.078 ns** | **0.0019** |      **32 B** |
|    Format0 |   Right |    False | 14.12 ns | 1.300 ns | 0.071 ns | 0.0019 |      32 B |
| FormatOld8 |   Right |    False | 14.03 ns | 2.185 ns | 0.120 ns | 0.0019 |      32 B |
|    Format8 |   Right |    False | 15.17 ns | 1.619 ns | 0.089 ns | 0.0019 |      32 B |
| **FormatOld0** |   **Right** |     **True** | **12.78 ns** | **0.322 ns** | **0.018 ns** | **0.0019** |      **32 B** |
|    Format0 |   Right |     True | 12.86 ns | 1.711 ns | 0.094 ns | 0.0019 |      32 B |
| FormatOld8 |   Right |     True | 12.43 ns | 0.314 ns | 0.017 ns | 0.0019 |      32 B |
|    Format8 |   Right |     True | 12.76 ns | 0.495 ns | 0.027 ns | 0.0019 |      32 B |

# Long

|          Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|---------------- |---------:|---------:|---------:|-------:|----------:|
|   ParseDefault8 | 23.33 ns | 0.886 ns | 0.049 ns | 0.0024 |      40 B |
|    ParseCustom8 | 10.39 ns | 0.133 ns | 0.007 ns |      - |         - |
|  ParseDefault19 | 31.74 ns | 2.601 ns | 0.143 ns | 0.0038 |      64 B |
|   ParseCustom19 | 27.62 ns | 1.448 ns | 0.079 ns |      - |         - |
|  FormatDefault8 | 26.03 ns | 4.074 ns | 0.223 ns | 0.0043 |      72 B |
|   FormatCustom8 | 12.88 ns | 2.494 ns | 0.137 ns | 0.0019 |      32 B |
| FormatDefault19 | 36.23 ns | 2.722 ns | 0.149 ns | 0.0067 |     112 B |
|  FormatCustom19 | 24.85 ns | 0.699 ns | 0.038 ns | 0.0029 |      48 B |

|   Method | Padding | ZeroFill |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|--------- |-------- |--------- |---------:|---------:|---------:|-------:|----------:|
|  **Format8** |    **Left** |    **False** | **12.75 ns** | **0.319 ns** | **0.017 ns** | **0.0019** |      **32 B** |
| Format19 |    Left |    False | 24.69 ns | 0.084 ns | 0.005 ns | 0.0029 |      48 B |
|  **Format8** |    **Left** |     **True** | **12.87 ns** | **0.392 ns** | **0.021 ns** | **0.0019** |      **32 B** |
| Format19 |    Left |     True | 25.19 ns | 0.866 ns | 0.047 ns | 0.0029 |      48 B |
|  **Format8** |   **Right** |    **False** | **14.57 ns** | **0.833 ns** | **0.046 ns** | **0.0019** |      **32 B** |
| Format19 |   Right |    False | 28.39 ns | 0.819 ns | 0.045 ns | 0.0029 |      48 B |
|  **Format8** |   **Right** |     **True** | **12.92 ns** | **1.504 ns** | **0.082 ns** | **0.0019** |      **32 B** |
| Format19 |   Right |     True | 25.20 ns | 3.923 ns | 0.215 ns | 0.0029 |      48 B |

# Reverse

|          Method |     Mean |     Error |    StdDev | Allocated |
|---------------- |---------:|----------:|----------:|----------:|
|        Reverse4 | 1.748 ns | 0.2285 ns | 0.0125 ns |         - |
|  ReverseUnsafe4 | 1.753 ns | 1.3057 ns | 0.0716 ns |         - |
|        Reverse8 | 3.107 ns | 0.5006 ns | 0.0274 ns |         - |
|  ReverseUnsafe8 | 2.705 ns | 1.3742 ns | 0.0753 ns |         - |
|       Reverse16 | 6.184 ns | 0.7919 ns | 0.0434 ns |         - |
| ReverseUnsafe16 | 4.546 ns | 0.7908 ns | 0.0433 ns |         - |

# Utf16

|          Method |      Mean |     Error |    StdDev |    Median |
|---------------- |----------:|----------:|----------:|----------:|
| GetStringCustom | 10.238 ns | 0.0798 ns | 0.0747 ns | 10.268 ns |
| CopyBytesCustom |  2.469 ns | 0.0709 ns | 0.1104 ns |  2.549 ns |
