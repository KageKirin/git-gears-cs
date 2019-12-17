using System;

namespace git_gears
{
public class Test
{
	public static int Execute(TestOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
		Console.WriteLine($"remote: {GitUtils.GetCurrentRemote()}");
		Console.WriteLine($"branch: {GitUtils.GetCurrentBranch()}");

		Console.WriteLine($"opts remote: {opts.Remote}");
		Console.WriteLine($"opts branch: {opts.Branch}");

		GitUtils.TestPrintInfo();
		GitUtils.TestPrintConfig();

		return 0;
	}
}
}
