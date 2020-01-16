using System;

namespace git_gears
{
public class CreateRepo : BaseAction
{
	public static int Execute(CreateRepoOptions opts)
	{
		SanitizeOptions((CommonOptions) opts);
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		if (gear != null)
		{
			RepoInfo? repo = gear.CreateRepo(new CreateRepoParams{
				description = opts.Description,
				homepage = opts.Homepage,
				isPublic = !opts.IsPrivate,
			});
			if (repo.HasValue)
			{
				Console.WriteLine($"Created new repo on {opts.Remote}");
				Console.WriteLine($"{repo.Value.Name} -- {repo.Value.Description} -- {repo.Value.Homepage}");
				Console.WriteLine($"{repo.Value.Url}");
			}
			else
			{
				Console.WriteLine($"Failed to create an repo on {opts.Remote}.");
			}
		}
		return 0;
	}
}
}
