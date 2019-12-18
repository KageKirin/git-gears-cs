using System;
using System.IO;
using System.Linq;

namespace git_gears
{
public static class GearFactory
{
	static public IGear CreateGear(string remote)
	{
		GitUtils.GearsApiType api = GitUtils.GetGearsApiType(remote);

		switch (api)
		{
		case GitUtils.GearsApiType.GitHub:
			return new GitHubGear(remote);
		case GitUtils.GearsApiType.GitLab:
			return new GitLabGear(remote);
		}
		return null;
	}
}
}
