using CommandLine;

namespace git_gears
{
[Verb("get-owner", HelpText = "Get the repo owner (user or organization).")]
public class GetOwnerOptions : CommonGetOptions
{
}
}
