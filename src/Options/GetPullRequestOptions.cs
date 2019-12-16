using CommandLine;

namespace git_gears
{
[Verb("get-pullrequest", HelpText = "Get the pullrequest associated with this branch.")]
public class GetPullRequestOptions : CommonGetOptions
{
}
}
