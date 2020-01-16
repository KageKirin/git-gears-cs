using System;

namespace git_gears
{
public class ListIssues : BaseAction
{
	public static int Execute(ListIssuesOptions opts)
	{
		SanitizeOptions((CommonOptions) opts);
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");
		if (gear != null)
		{
			Console.WriteLine("-- ListIssues --");
			var issues = gear.ListIssues();
			foreach(var i in issues)
			{
				Console.WriteLine($"#{i.Number} - {i.Title} - {i.Url}");
			}
		}

		return 0;
	}
}
}
