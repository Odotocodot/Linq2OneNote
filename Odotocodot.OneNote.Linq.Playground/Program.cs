using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Odotocodot.OneNote.Linq.Parsers;
using System.IO;

namespace Odotocodot.OneNote.Linq.Playground
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<XmlParserBenchmarks>();
        }

        [MarkdownExporter]
        [MinColumn, MaxColumn]
        [MemoryDiagnoser]
        public class XmlParserBenchmarks
        {
            private string xml;
            private readonly Consumer consumer = new();
            private readonly XElementXmlParser xElementParser = new();
            private readonly XmlReaderXmlParser xmlReaderParser = new();
            private readonly XmlReaderSubTreeXmlParser subTreeParser = new();
            [GlobalSetup]
            public void GlobalSetup()
            {
                xml = File.ReadAllText("Inputs\\Notebooks.xml");
            }

            [Benchmark(Baseline = true)]
            public void XElement()
            {
                xElementParser.ParseNotebooks(xml).GetPages().Consume(consumer);
            }

            [Benchmark]
            public void XmlReader()
            {
                xmlReaderParser.ParseNotebooks(xml).GetPages().Consume(consumer);
            }
            [Benchmark]
            public void XmlReaderUsingSubTree()
            {
                subTreeParser.ParseNotebooks(xml).GetPages().Consume(consumer);
            }
        }


    }
}