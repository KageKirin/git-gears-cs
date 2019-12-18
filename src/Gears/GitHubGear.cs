using System;
using System.IO;
using System.Linq;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;

namespace git_gears
{
public class GitHubGear : CommonGear, IGear
{
	public GitHubGear(string remote) : base(remote)
	{
	}

	public void Test()
	{
		var graphQLHttpRequest = new GraphQLRequest {
			Query = @"
				query($_owner: String!, $_name: String!){
					repository(owner: $_owner, name: $_name)
					{
						id,
						name,
						url
					}
				}",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name  = RepoUrl.RepoName,
			}
		};
		Console.WriteLine($"{graphQLHttpRequest.Query}");
		Console.WriteLine($"{graphQLHttpRequest.Variables}");
		GraphQLResponse graphQLHttpResponse = Client.PostAsync(graphQLHttpRequest).Result;
		Console.WriteLine($"{graphQLHttpResponse.Data != null}");
		
		if (graphQLHttpResponse.Data != null)
		{
			Console.WriteLine($"{graphQLHttpResponse.Data.ToString()}");
		}
	}
}
}
