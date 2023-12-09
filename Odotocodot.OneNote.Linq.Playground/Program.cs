using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;

namespace Odotocodot.OneNote.Linq.Playground
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).RunAll();
		}

		[MemoryDiagnoser]
		[InProcess]
		public class ParserBenchmark
		{
			private readonly string xml = File.ReadAllText(@"C:\Users\jdara\source\repos\Odotocodot.OneNote.Linq\Odotocodot.OneNote.Linq.Tests\Inputs\Notebooks.xml");
			private readonly Consumer consumer = new Consumer();

			private void Consume(IEnumerable<IOneNoteItem> items)
			{
				foreach (var item in items)
				{
					consumer.Consume(item);
				}
			}
			
			[Benchmark]
			public void SwitchCase()
			{
				 Consume(Parser.ParseNotebooks(xml));
			}			
			[Benchmark]
			public void Dictionary()
			{
				 Consume(ParserDict.ParseNotebooks(xml));
			}
		}

		[MemoryDiagnoser]
		[InProcess]
		public class AsParallelBenchmark
		{
			private readonly Consumer consumer = new Consumer();

			private void Consume(IEnumerable<IOneNoteItem> items)
			{
				foreach (var item in items)
				{
					consumer.Consume(item);
				}
			}
			
			[GlobalSetup]
			public void Setup()
			{
				OneNoteApplication.InitComObject();
			}

			[GlobalCleanup]
			public void Cleanup()
			{
				OneNoteApplication.ReleaseComObject();
			}

			
			[Benchmark]
			public void AsParallel()
			{
				 Consume(OneNoteApplication.GetNotebooks()
					.GetPages()
					.AsParallel()
					.OrderByDescending(pg => pg.LastModified));
			}
			[Benchmark]
			public void NonParallel()
			{
				Consume(OneNoteApplication.GetNotebooks()
					.GetPages()
					.OrderByDescending(pg => pg.LastModified));
			}
		}
	}
}