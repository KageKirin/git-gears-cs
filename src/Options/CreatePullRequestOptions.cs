using CommandLine;

namespace git_gears
{
[Verb(
	"create-pullrequest",
	HelpText =
		@"Create a new pullrequest (aka mergerequest)
		from the given remote and branch into a target (remote) branch,
		respectively target repo:branch.
		Please note that this operation will fail if you haven't pushed the branch to the remote yet.")]
public class CreatePullRequestOptions : CommonCreateOptions
{
	[Option('t', "title", Required = false, HelpText = "pullrequest title.")]
	public string Title
	{
		get;
		set;
	}

	[Option('m', "message", Required = false, HelpText = "pullrequest body in Markdown.")]
	public string Body
	{
		get;
		set;
	}

	[Option("into", Required = false,
			HelpText = @"target 'branch' or 'repo:branch' to merge into.
			`repo` must be a known remote ",
			Default = "master")]
	public string TargetBranch
	{
		get;
		set;
	}
}
}
