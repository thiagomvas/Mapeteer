
using BenchmarkDotNet.Running;
using Mapeteer.Benchmarks;

var summary = BenchmarkRunner.Run<MappingBenchmarks>();