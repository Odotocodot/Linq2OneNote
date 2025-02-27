using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using System.IO;

namespace Odotocodot.OneNote.Linq.Playground
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var xml = File.ReadAllText("Notebooks.xml");
            //var notebooks = XmlParser2.Parse(xml);
            //foreach (var item in notebooks.Traverse())
            //{
            //    System.Console.WriteLine(item.Name);
            //}

            //Console.WriteLine("=====================================");

            //notebooks = XmlParser.ParseNotebooks(xml);
            //foreach (var item in notebooks.Traverse())
            //{
            //    System.Console.WriteLine(item.Name);
            //}

            BenchmarkRunner.Run<ParserBenchmarks>();
        }

        [MarkdownExporter]
        [MinColumn, MaxColumn]
        [MemoryDiagnoser]
        public class ParserBenchmarks
        {
            private string xml;
            private readonly Consumer consumer = new Consumer();
            [GlobalSetup]
            public void GlobalSetup()
            {
                xml = File.ReadAllText("Notebooks.xml");
            }

            [Benchmark(Baseline = true)]
            public void XmlParserV1()
            {
                XmlParser.ParseNotebooks(xml).GetPages().Consume(consumer);
            }

            [Benchmark]
            public void XmlParserV2()
            {
                XmlParser2.ParseNotebooks(xml).GetPages().Consume(consumer);
            }
        }


    }
}