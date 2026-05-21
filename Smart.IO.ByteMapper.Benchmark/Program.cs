using System.Reflection;

using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);

// Workaround for top-level statement partial class requirement
internal partial class Program
{
}
