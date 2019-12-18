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
		Token = GitUtils.GetGearsAuthBearerToken(remote);
		RepoUrl = GitUtils.GetRemoteUrl(remote);
		Client = new GraphQLClient(Endpoint);
		Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
	}

	protected string Endpoint
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
