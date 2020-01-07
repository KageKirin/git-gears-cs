using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

using GraphQL.Client;

namespace git_gears
{
public abstract class CommonGear
{
	public CommonGear(string remote)
	{
		Endpoint = GitUtils.GetGearsEndpointUrl(remote);
		RestEndpoint = GitUtils.GetGearsRestEndpointUrl(remote);
		Token = GitUtils.GetGearsAuthBearerToken(remote);
		UserAgent = $"git-gears/1.0.0 {GitUtils.GetConfigEntry("user.name")}";
		RepoUrl = GitUtils.GetRemoteUrl(remote);

		Client = new GraphQLClient(Endpoint);
		Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
		Client.DefaultRequestHeaders.Add("User-Agent", $"git-gears/1.0.0 {GitUtils.GetConfigEntry("user.name")}");
	}

	protected string Endpoint
	{
		get;
	}

	protected string RestEndpoint
	{
		get;
	}

	protected string UserAgent
	{
		get;
	}

	protected string Token
	{
		get;
	}

	protected GraphQLClient Client
	{
		get;
	}

	protected GitUrl RepoUrl
	{
		get;
	}
}
}
