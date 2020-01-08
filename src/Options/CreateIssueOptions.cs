using CommandLine;

namespace git_gears
{
[Verb("create-issue", HelpText = "Create a new issue.")]
public class CreateIssueOptions : CommonCreateOptions
{
	[Option('t', "title", Required = false, HelpText = "issue title.", Default = "new issue")]
	public string Title
	{
		get;
		set;
	}

	[Option('m', "message", Required = false, HelpText = "issue body in Markdown.", Default = "edit this message")]
	public string Body
	{
		get;
		set;
	}
}
}
