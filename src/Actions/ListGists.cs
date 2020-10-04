using System;
using System.Threading.Tasks;

namespace git_gears
{
public class ListGists
{
	public static async Task<int> ExecuteAsync(ListGistsOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");
		if (gear != null)
		{
			Console.WriteLine("-- ListGists --");
			var gists = await gear.ListGistsAsync();
			foreach(var i in gists)
			{
				Console.WriteLine($"{i.Description} - {i.Name} - {i.Id} - {i.Url}");
			}
		}

		return 0;
	}
}
}
