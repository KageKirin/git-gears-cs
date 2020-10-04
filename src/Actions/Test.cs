using System;
using System.Threading.Tasks;

namespace git_gears
{
public class Test
{
	public static async Task<int> ExecuteAsync(TestOptions opts)
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
		{
			Console.WriteLine("-- Test --");
			await gear.TestAsync();
			Console.WriteLine("-- GetUser --");
			Console.WriteLine($"{await gear.GetUserAsync(url.Owner)}");
			Console.WriteLine("-- GetOwner --");
			Console.WriteLine($"{await gear.GetOwnerAsync(url.Owner)}");
			Console.WriteLine("-- GetOrganization --");
			Console.WriteLine($"{await gear.GetOrganizationAsync(url.Owner)}");

			Console.WriteLine("-- GetRepo --");
			Console.WriteLine($"{await gear.GetRepoAsync()}");
			Console.WriteLine("-- GetPullRequest --");
			Console.WriteLine($"{await gear.GetPullRequestAsync(GitUtils.GetCurrentBranch())}");
			Console.WriteLine("-- ListPullRequests --");
			Console.WriteLine($"{await gear.ListPullRequestsAsync()}");
			Console.WriteLine("-- ListIssues --");
			Console.WriteLine($"{await gear.ListIssuesAsync()}");
			Console.WriteLine("-- ListGists --");
			Console.WriteLine($"{await gear.ListGistsAsync()}");
			Console.WriteLine("-- ListRepos --");
			Console.WriteLine($"{await gear.ListReposAsync()}");
		}

		return 0;
	}
}
}
