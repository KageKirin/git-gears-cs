using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

using GraphQL.Client;
using DalSoft.RestClient;
using Flurl.Http;

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

		HttpClient = new HttpClient();
		HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
		HttpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);

		GqlClient = new GraphQLClient(Endpoint);
		GqlClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
		GqlClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);

		RestClient = new RestClient(RestEndpoint,
			new Headers {
				{ "User-Agent", UserAgent },
				{ "Authorization", $"bearer {Token}" },
			}
		);
		RestClient.Authorization(AuthenticationSchemes.Bearer, $"bearer {Token}");

		FlurlClient = new FlurlClient(RestEndpoint);
		FlurlClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
		FlurlClient.HttpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
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

	protected HttpClient HttpClient
	{
		get;
	}

	protected GraphQLClient GqlClient
	{
		get;
	}

	protected RestClient RestClient
	{
		get;
	}

	protected FlurlClient FlurlClient
	{
		get;
	}

	protected GitUrl RepoUrl
	{
		get;
	}
}
}
