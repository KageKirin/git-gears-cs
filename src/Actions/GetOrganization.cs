using System;
using System.Threading.Tasks;

namespace git_gears
{
public class GetOrganization
{
	public static async Task<int> ExecuteAsync(GetOrganizationOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		var url = GitUtils.GetRemoteUrl(opts.Remote);

		if (gear != null)
		{
			OrganizationInfo? org = await gear.GetOrganizationAsync(url.Owner);
			if (org.HasValue)
			{
				Console.WriteLine($"Owning organization for {opts.Remote}");
				Console.WriteLine(
					$"{org.Value.Name} -- {org.Value.Description} -- {org.Value.Url} -- {org.Value.Website}");
			}
			else
			{
				Console.WriteLine($"There is no owning organization to be found for {opts.Remote}.");
				Console.WriteLine("Try `get-user` or `get-owner`.");
			}
		}

		return 0;
	}
}
}
