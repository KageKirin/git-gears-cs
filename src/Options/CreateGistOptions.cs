using CommandLine;

namespace git_gears
{
[Verb("create-gist", HelpText = "Create a new gist.")]
public class CreateGistOptions : CommonCreateOptions
{
	[Option('t', "title", Required = false, HelpText = "gist title.", Default = "new gist")]
	public string Title
	{
		get;
		set;
	}

	[Option('m', "message", Required = false, HelpText = "gist body in Markdown.", Default = "edit this message")]
	public string Body
	{
		get;
		set;
	}

	[Option('d', "description", Required = false, HelpText = "gist description.", Default = "")]
	public string Description
	{
		get;
		set;
	}

	[Option('n', "filename", Required = false, HelpText = "gist filename.", Default = "gist.md")]
	public string Filename
	{
		get;
		set;
	}

	[Option("private", Required = false, HelpText = "make private gist.", Default = false)]
	public bool PrivateGist
	{
		get;
		set;
	}

	[Option("in-repo", Required = false,
			HelpText = "create Gist/Snippet in repo namespace instead of in user namespace (GitLab only).",
			Default = false)]
	public bool RepoGist
	{
		get;
		set;
	}
}
}
