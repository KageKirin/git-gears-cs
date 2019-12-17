using System;
using System.IO;
using System.Linq;

namespace git_gears
{
public class GearFactory
{
	enum GearApiType
	{
		GitHub,
		GitLab

	}
	static public IGear CreateGear(string remote)
	{
		GearApiType api =
			(from x in Enum.GetValues(typeof (GearApiType)).Cast<GearApiType>() where x.ToString().ToLower() ==
			 GitUtils.GetGearsConfigEntry("api", remote).ToLower() select x)
				.ToArray()
				.First();

		switch (api)
		{
		case GearApiType.GitHub:
			return new GitHubGear(remote);
		case GearApiType.GitLab:
			return new GitLabGear(remote);
		}
		return null;
	}
}
}
