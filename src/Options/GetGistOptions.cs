using CommandLine;

namespace git_gears
{
[Verb("get-gist", HelpText = "Get the indicated gist.")]
public class GetGistOptions : CommonGetOptions
{
	[Option("name", Required = true, HelpText = "gist name.")]
	public string Name
	{
		get;
		set;
	}
}
}
