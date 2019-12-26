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

	public GistInfo? GetGist()
	{
		return new GistInfo();
	}

	public IssueInfo? GetIssue()
	{
		return new IssueInfo();
	}

	public PullRequestInfo? GetPullRequest()
	{
		return new PullRequestInfo();
	}

	public RepoInfo? GetRepo()
	{
		return new RepoInfo();
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
