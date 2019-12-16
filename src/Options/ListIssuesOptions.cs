using CommandLine;

namespace git_gears
{
[Verb("list-issues", HelpText = "List the issues with the given remote.")]
public class ListIssuesOptions : CommonListOptions
{
}
}
