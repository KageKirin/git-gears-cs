using CommandLine;

namespace git_gears
{
[Verb("create-repo", HelpText = "Create a new repo.")]
public class CreateRepoOptions : CommonCreateOptions
{
	[Option('d', "description", Required = false, HelpText = "repo description.", Default = "")]
	public string Description
	{
		get;
		set;
	}

	[Option("homepage", Required = false, HelpText = "repo homepage.")]
	public string Homepage
	{
		get;
		set;
	}

	[Option("private", Required = false, HelpText = "make private repo.", Default = false)]
	public bool IsPrivate
	{
		get;
		set;
	}
}
}
