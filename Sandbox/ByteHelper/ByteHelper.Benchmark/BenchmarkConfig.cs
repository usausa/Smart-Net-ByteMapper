namespace ByteHelper;

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;

public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddColumn(
            StatisticColumn.Mean,
            //StatisticColumn.Min,
            //StatisticColumn.Max,
            //StatisticColumn.P90,
            //StatisticColumn.StdDev,
            StatisticColumn.Error);
        AddDiagnoser(MemoryDiagnoser.Default);
        AddExporter(MarkdownExporter.Default, MarkdownExporter.GitHub);
        AddExporter(CsvExporter.Default);
        //AddJob(Job.MediumRun);
        AddJob(Job.ShortRun);
    }
}
