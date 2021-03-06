using System;
using System.Threading.Tasks;

namespace git_gears
{
public class ListPullRequests
{
	public static async Task<int> ExecuteAsync(ListPullRequestsOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");
		if (gear != null)
		{
			Console.WriteLine("-- ListPullRequests --");
			var prs = await gear.ListPullRequestsAsync();
			foreach (var i in prs)
			{
				Console.WriteLine($"#{i.Number} - {i.Title} - {i.Url}");
			}
		}

		return 0;
	}
}
}
