using System;

namespace git_gears
{
public class ListRepos : BaseAction
{
	public static int Execute(ListReposOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");
		if (gear != null)
		{
			Console.WriteLine("-- ListRepos --");
			var repos = gear.ListRepos();
			foreach(var i in repos)
			{
				Console.WriteLine($"{i.Name} - {i.Description} - {i.Url}");
			}
		}

		return 0;
	}
}
}
