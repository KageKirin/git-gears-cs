using System;

namespace git_gears
{
public class GetGist
{
	public static int Execute(GetGistOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		if (gear != null)
		{
			GistInfo? gist = gear.GetGist(opts.Name);
			if (gist.HasValue)
			{
				Console.WriteLine($"Gist {opts.Name} for {opts.Remote}");
				Console.WriteLine($"{gist.Value.Description} - {gist.Value.Name} - {gist.Value.Id}");
			}
			else
			{
				Console.WriteLine($"There is no gist named {opts.Name} to be found on {opts.Remote}.");
			}
		}
		return 0;
	}
}
}
