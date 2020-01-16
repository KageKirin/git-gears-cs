using System;

namespace git_gears
{

public class BaseAction
{
	protected static void SanitizeOptions(CommonOptions opts)
	{
		opts.Remote = opts.Remote ?? GitUtils.GetCurrentRemote();
		opts.Branch = opts.Branch ?? GitUtils.GetCurrentBranch();
	}
}
}
