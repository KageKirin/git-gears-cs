using System;
using System.IO;
using System.Linq;
using GraphQL;
using GraphQL.Types;


namespace git_gears
{
public class GitHubGear : CommonGear, IGear
{
	public GitHubGear(string remote) : base(remote)
	{

	}

	public void Test()
	{

	}
}
}
