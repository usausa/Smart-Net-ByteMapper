namespace Smart.IO.ByteMapper.Benchmark;

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

#pragma warning disable CA1812
internal sealed class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        // JitOptimizationsValidator is not compatible with InProcessEmitToolchain
        // AddValidator(JitOptimizationsValidator.FailOnError);
        AddDiagnoser(MemoryDiagnoser.Default);
        AddColumn(StatisticColumn.OperationsPerSecond);
        AddJob(Job.MediumRun
            .WithJit(Jit.RyuJit)
            .WithPlatform(Platform.X64)
            .WithToolchain(InProcessEmitToolchain.Instance));
    }
}
#pragma warning restore CA1812
