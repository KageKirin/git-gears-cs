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
		var url = GitUtils.GetRemoteUrl(opts.Remote);
		Console.WriteLine($"opts owner: {url.Owner}");

		GitUtils.TestPrintInfo();
		GitUtils.TestPrintConfig();

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");
		if (gear != null)
			gear.Test();

		return 0;
	}
}
}
