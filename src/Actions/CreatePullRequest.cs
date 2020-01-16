using System;

namespace git_gears
{
public class CreatePullRequest : BaseAction
{
	public static int Execute(CreatePullRequestOptions opts)
	{
		SanitizeOptions((CommonOptions) opts);
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		var branch = opts.Branch ?? GitUtils.GetCurrentBranch();
		// TODO: parse opts.TargetBranch to extract repo from pattern repo:branch
		var title = opts.Title ?? $"merge {branch} -> {opts.TargetBranch}";
		var body = opts.Body ?? $"This PR merges `{branch}` into `{opts.TargetBranch}`.";
		// TODO: support for opts `--draft`

		if (gear != null)
		{
			PullRequestInfo? pr = gear.CreatePullRequest(new CreatePullRequestParams{
				branch = branch, targetBranch = opts.TargetBranch, title = title, body = body});
			if (pr.HasValue)
			{
				Console.WriteLine($"Created new pull request on {opts.Remote}");
				Console.WriteLine($"#{pr.Value.Number} - {pr.Value.Title} - {pr.Value.Url}");
				Console.WriteLine($"{pr.Value.Body}");
			}
			else
			{
				Console.WriteLine($"Failed to create an issue on {opts.Remote}.");
			}
		}
		return 0;
	}
}
}
