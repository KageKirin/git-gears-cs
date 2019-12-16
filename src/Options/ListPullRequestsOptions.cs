using CommandLine;

namespace git_gears
{
[Verb("list-pullrequests", HelpText = "List the pullrequests associated with this remote.")]
public class ListPullRequestsOptions : CommonListOptions
{
}
}
