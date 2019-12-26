using System;

namespace git_gears
{
public class GetPullRequest
{
	public static int Execute(GetPullRequestOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		var branch = opts.Branch ?? GitUtils.GetCurrentBranch();

		if (gear != null)
		{
			PullRequestInfo? pr = gear.GetPullRequest(branch);
			if (pr.HasValue)
			{
				Console.WriteLine($"PullRequest for {opts.Remote}/{branch}");
				Console.WriteLine($"#{pr.Value.Number} - {pr.Value.Title} - {pr.Value.Url}");
				Console.WriteLine($"merging {pr.Value.HeadRef} onto {pr.Value.BaseRef}");
				Console.WriteLine($"{pr.Value.Body}");
			}
			else
			{
				Console.WriteLine($"There is no pullrequest to be found for {opts.Remote} {opts.Branch}");
			}
		}

		return 0;
	}
}
}
