using System;

namespace git_gears
{
public class GetRepo
{
	public static int Execute(GetRepoOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		if (gear != null)
		{
			var repo = gear.GetRepo();
			if (repo != null)
			{
				Console.WriteLine($"Repo for {opts.Remote}");
				Console.WriteLine($"{repo.Name} -- {repo.Description}");
				Console.WriteLine($"{repo.Url}");
			}
			else
			{
				Console.WriteLine($"There is no repository to be found for {opts.Remote}");
			}
		}

		return 0;
	}
}
}
