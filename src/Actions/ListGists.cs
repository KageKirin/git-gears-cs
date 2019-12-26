using System;

namespace git_gears
{
public class ListGists
{
	public static int Execute(ListGistsOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");
		if (gear != null)
		{
			Console.WriteLine("-- ListGists --");
			var gists = gear.ListGists();
			foreach(var i in gists)
			{
				Console.WriteLine($"{i.Description} - {i.Name} - {i.Id}");
			}
		}

		return 0;
	}
}
}
