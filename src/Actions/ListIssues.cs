using System;
using System.Threading.Tasks;

namespace git_gears
{
public class ListIssues
{
	public static async Task<int> ExecuteAsync(ListIssuesOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");
		if (gear != null)
		{
			Console.WriteLine("-- ListIssues --");
			var issues = await gear.ListIssuesAsync();
			foreach (var i in issues)
			{
				Console.WriteLine($"#{i.Number} - {i.Title} - {i.Url}");
			}
		}

		return 0;
	}
}
}
