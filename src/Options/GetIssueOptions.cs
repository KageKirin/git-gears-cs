using CommandLine;

namespace git_gears
{
[Verb("get-issue", HelpText = "Get the indicated issue.")]
public class GetIssueOptions : CommonGetOptions
{
	[Option("number", Required = true, HelpText = "issue number.")]
	public int Number
	{
		get;
		set;
	}
}
}
