namespace Odotocodot.OneNote.Linq.Playground
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).RunAll();
		}
	}
}