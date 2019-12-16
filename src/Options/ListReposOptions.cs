using CommandLine;

namespace git_gears
{
[Verb("list-repos", HelpText = "List the repos visible to the current user.")]
public class ListReposOptions : CommonListOptions
{
}
}
