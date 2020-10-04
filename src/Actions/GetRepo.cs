using System;
using System.Threading.Tasks;

namespace git_gears
{
public class GetRepo
{
	public static async Task<int> ExecuteAsync(GetRepoOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		if (gear != null)
		{
			RepoInfo? repo = await gear.GetRepoAsync();
			if (repo.HasValue)
			{
				Console.WriteLine($"Repo for {opts.Remote}");
				Console.WriteLine($"{repo.Value.Name} -- {repo.Value.Description} -- {repo.Value.Homepage}");
				Console.WriteLine($"{repo.Value.Url}");
			}
			else
			{
				Console.WriteLine($"There is no repository to be found for {opts.Remote}.");
			}
		}

		return 0;
	}
}
}
