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
		RepoUrl = GitUtils.GetRemoteUrl(remote);

		Client = new GraphQLClient(Endpoint);
		Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
		Client.DefaultRequestHeaders.Add("User-Agent", $"git-gears/1.0.0 {GitUtils.GetConfigEntry("user.name")}");

		//ProductHeaderValue header = new ProductHeaderValue("git-gears", $"git-gears/1.0.0 {GitUtils.GetConfigEntry("user.name")} <{GitUtils.GetConfigEntry("user.email")}>");
		//ProductInfoHeaderValue userAgent = new ProductInfoHeaderValue(header);
		//Client.DefaultRequestHeaders.UserAgent.Add(userAgent);
		Console.WriteLine($"user-agent: {Client.DefaultRequestHeaders.UserAgent}");
	}

	protected string Endpoint
	{
		get;
	}

	protected string RestEndpoint
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
