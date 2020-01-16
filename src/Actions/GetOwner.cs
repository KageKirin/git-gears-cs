using System;

namespace git_gears
{
public class GetOwner : BaseAction
{
	public static int Execute(GetOwnerOptions opts)
	{
		SanitizeOptions((CommonOptions)opts);
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		var url = GitUtils.GetRemoteUrl(opts.Remote);

		if (gear != null)
		{
			OwnerInfo? owner = gear.GetOwner(url.Owner);
			if (owner.HasValue)
			{
				Console.WriteLine($"Owner for {opts.Remote}");
				Console.WriteLine($"{owner.Value.Name} -- {owner.Value.Login} -- {owner.Value.Url}");
			}
			else
			{
				Console.WriteLine($"There is no owner to be found for {opts.Remote}.");
				Console.WriteLine("This is an unexpected result.");
			}
		}

		return 0;
	}
}
}
