using System;

namespace git_gears
{
public class CreatePullRequest
{
	public static int Execute(CreatePullRequestOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		var branch = opts.Branch ?? GitUtils.GetCurrentBranch();
		// TODO: parse opts.TargetBranch to extract repo from pattern repo:branch

		if (gear != null)
		{
			PullRequestInfo? pr = gear.CreatePullRequest(new CreatePullRequestParams{
				branch = branch, targetBranch = opts.TargetBranch, title = opts.Title, body = opts.Body});
			if (pr.HasValue)
			{
				Console.WriteLine($"Created new issue on {opts.Remote}");
				Console.WriteLine($"#{pr.Value.Number} - {pr.Value.Title} - {pr.Value.Url}");
				Console.WriteLine($"{pr.Value.Body}");
			}
			else
			{
				Console.WriteLine($"Failed to create an issue on {opts.Remote}");
			}
		}
		return 0;
	}
}
}
