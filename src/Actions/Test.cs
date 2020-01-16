using System;

namespace git_gears
{
public class Test : BaseAction
{
	public static int Execute(TestOptions opts)
	{
		SanitizeOptions((CommonOptions) opts);
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
		{
			Console.WriteLine("-- Test --");
			gear.Test();
			Console.WriteLine("-- GetUser --");
			Console.WriteLine($"{gear.GetUser(url.Owner)}");
			Console.WriteLine("-- GetOwner --");
			Console.WriteLine($"{gear.GetOwner(url.Owner)}");
			Console.WriteLine("-- GetOrganization --");
			Console.WriteLine($"{gear.GetOrganization(url.Owner)}");

			Console.WriteLine("-- GetRepo --");
			Console.WriteLine($"{gear.GetRepo()}");
			Console.WriteLine("-- GetPullRequest --");
			Console.WriteLine($"{gear.GetPullRequest(GitUtils.GetCurrentBranch())}");
			Console.WriteLine("-- ListPullRequests --");
			Console.WriteLine($"{gear.ListPullRequests()}");
			Console.WriteLine("-- ListIssues --");
			Console.WriteLine($"{gear.ListIssues()}");
			Console.WriteLine("-- ListGists --");
			Console.WriteLine($"{gear.ListGists()}");
			Console.WriteLine("-- ListRepos --");
			Console.WriteLine($"{gear.ListRepos()}");
		}

		return 0;
	}
}
}
