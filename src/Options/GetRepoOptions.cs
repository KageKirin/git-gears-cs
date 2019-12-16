using CommandLine;

namespace git_gears
{
[Verb("get-repo", HelpText = "Get the host repo for this remote.")]
public class GetRepoOptions : CommonGetOptions
{
}
}
