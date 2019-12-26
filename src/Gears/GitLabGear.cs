using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;

namespace git_gears
{
public class GitLabGear : CommonGear, IGear
{
	public GitLabGear(string remote) : base(remote)
	{
	}

	public void Test()
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query
			{
				currentUser
				{
					username,
					name
				}
			}",
			//Variables = new {_fullPath = $"{RepoUrl.Owner}/{RepoUrl.RepoName}"}};
		};
		// clang-format on
		Console.WriteLine($"{gqlRequest.Query}");
		// Console.WriteLine($"{gqlRequest.Variables}");
		GraphQLResponse graphQLHttpResponse = Client.PostAsync(gqlRequest).Result;
		Console.WriteLine($"{graphQLHttpResponse.Data != null}");

		if (graphQLHttpResponse.Data != null)
		{
			Console.WriteLine($"{graphQLHttpResponse.Data.ToString()}");
		}
	}

	public GistInfo? GetGist(string name)
	{
		return null;
	}

	public IssueInfo? GetIssue(int number)
	{
		return null;
	}

	public PullRequestInfo? GetPullRequest(string branch)
	{
		return null;
	}

	public RepoInfo? GetRepo()
	{
		return null;
	}

	public IEnumerable<GistInfo>ListGists()
	{
		return new List<GistInfo>() as IEnumerable<GistInfo>;
	}

	public IEnumerable<IssueInfo>ListIssues()
	{
		return new List<IssueInfo>() as IEnumerable<IssueInfo>;
	}

	public IEnumerable<PullRequestInfo>ListPullRequests()
	{
		return new List<PullRequestInfo>() as IEnumerable<PullRequestInfo>;
	}

	public IEnumerable<RepoInfo>ListRepos()
	{
		return new List<RepoInfo>() as IEnumerable<RepoInfo>;
	}
}
}
