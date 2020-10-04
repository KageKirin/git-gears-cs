using System;
using System.Threading.Tasks;

namespace git_gears
{
public class GetUser
{
	public static async Task<int> ExecuteAsync(GetUserOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		var url = GitUtils.GetRemoteUrl(opts.Remote);

		if (gear != null)
		{
			UserInfo? user = await gear.GetUserAsync(url.Owner);
			if (user.HasValue)
			{
				Console.WriteLine($"Owning user for {opts.Remote}");
				Console.WriteLine($"{user.Value.Name} -- {user.Value.Login} -- {user.Value.Url} -- {user.Value.Website}");
			}
			else
			{
				Console.WriteLine($"There is no owning user to be found for {opts.Remote}.");
				Console.WriteLine("Try `get-organization` or `get-owner`.");
			}
		}

		return 0;
	}
}
}
