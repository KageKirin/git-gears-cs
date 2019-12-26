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
		var graphQLHttpRequest =
			new GraphQLRequest{Query = @" query($_fullPath
												: ID !){project(fullPath
																: $_fullPath){id, name, webUrl}} ",
							   Variables = new {_fullPath = $"{RepoUrl.Owner}/{RepoUrl.RepoName}"}};
		Console.WriteLine($"{graphQLHttpRequest.Query}");
		Console.WriteLine($"{graphQLHttpRequest.Variables}");
		GraphQLResponse graphQLHttpResponse = Client.PostAsync(graphQLHttpRequest).Result;
		Console.WriteLine($"{graphQLHttpResponse.Data != null}");

		if (graphQLHttpResponse.Data != null)
		{
			Console.WriteLine($"{graphQLHttpResponse.Data.ToString()}");
		}
	}

	public Gist GetGist()
	{
		return new Gist();
	}

	public Issue GetIssue()
	{
		return new Issue();
	}

	public PullRequest GetPullRequest()
	{
		return new PullRequest();
	}

	public Repo GetRepo()
	{
		return new Repo();
	}

	public IEnumerable<Gist>ListGists()
	{
		return new List<Gist>() as IEnumerable<Gist>;
	}

	public IEnumerable<Issue>ListIssues()
	{
		return new List<Issue>() as IEnumerable<Issue>;
	}

	public IEnumerable<PullRequest>ListPullRequests()
	{
		return new List<PullRequest>() as IEnumerable<PullRequest>;
	}

	public IEnumerable<Repo>ListRepos()
	{
		return new List<Repo>() as IEnumerable<Repo>;
	}
}
}
