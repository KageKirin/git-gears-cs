using System;

namespace git_gears
{
public class CreateGist : BaseAction
{
	public static int Execute(CreateGistOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

		var gear = GearFactory.CreateGear(opts.Remote);
		Console.WriteLine($"gear: {gear.ToString()}");

		if (gear != null)
		{
			GistInfo? gist = gear.CreateGist(new CreateGistParams{
				title = opts.Title,				//
				description = opts.Description, //
				filename = opts.Filename,		//
				body = opts.Body,				//
				isPublic = !opts.PrivateGist,	//
				isRepoGist = opts.RepoGist		//
			});
			if (gist.HasValue)
			{
				Console.WriteLine($"Created new gist on {opts.Remote}");
				Console.WriteLine($"{gist.Value.Description} - {gist.Value.Name} - {gist.Value.Id} - {gist.Value.Url}");
			}
			else
			{
				Console.WriteLine($"Failed to create a gist on {opts.Remote}.");
			}
		}
		return 0;
	}
}
}
