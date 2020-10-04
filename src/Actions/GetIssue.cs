using System;
using System.Threading.Tasks;

namespace git_gears
{
public class GetIssue
{
	public static async Task<int> ExecuteAsync(GetIssueOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		if (gear != null)
		{
			IssueInfo? issue = await gear.GetIssueAsync(opts.Number);
			if (issue.HasValue)
			{
				Console.WriteLine($"Issue {opts.Number} for {opts.Remote}");
				Console.WriteLine($"#{issue.Value.Number} - {issue.Value.Title} - {issue.Value.Url}");
				Console.WriteLine($"{issue.Value.Body}");
			}
			else
			{
				Console.WriteLine($"There is no issue to be found for {opts.Remote} #{opts.Number}.");
			}
		}
		return 0;
	}
}
}
