using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_owner: String!, $_name: String!)
			{
				repository(owner: $_owner, name: $_name)
				{
					id,
					name,
					url
				}
			} ",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
			}
		};
		// clang-format on
		Console.WriteLine($"{gqlRequest.Query}");
		Console.WriteLine($"{gqlRequest.Variables}");
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		Console.WriteLine($"{gqlResponse.Data != null}");

		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
		}
	}

	///////////////////////////////////////////////////////////////////////////

	public IssueInfo? GetIssue()
	{
		return new IssueInfo();
	}

	public IEnumerable<IssueInfo>ListIssues()
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_owner: String!, $_name: String!, $_count: Int !)
			{
				repository(owner: $_owner, name: $_name)
				{
					id,
					name,
					url,
					issues(first: $_count)
					{
						nodes
						{
							number,
							bodyText,
							state,
							title,
							url,
						}
					}
				}
			} ",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
				_count = 100
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");

			if (gqlResponse.Data.repository.issues.nodes != null)
			{
				var list = new List<IssueInfo>();
				foreach(var i in gqlResponse.Data.repository.issues.nodes)
				{
					list.Add(ToIssueInfo(i));
				}
				return list as IEnumerable<IssueInfo>;
			}
		}
		return null;
	}

	IssueInfo ToIssueInfo(dynamic gqlData)
	{
		var issue = new IssueInfo();
		issue.Number = gqlData.number;
		// TODO: state
		issue.Url = gqlData.url;
		issue.Title = gqlData.title;
		issue.Body = gqlData.bodyText;
		return issue;
	}

	///////////////////////////////////////////////////////////////////////////

	public PullRequestInfo? GetPullRequest()
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_owner: String!, $_name: String!, $_branch: String!)
			{
				repository(owner: $_owner, name: $_name)
				{
					id,
					name,
					url,
					pullRequests(headRefName: $_branch, orderBy: {direction : DESC, field : UPDATED_AT}, first : 1)
					{
						nodes
						{
							baseRefName,
							headRefName,
							number,
							bodyText,
							permalink,
							resourcePath,
							state,
							title,
							url,
						}
					}
				}
			} ",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
				_branch = "getlist-impl", // TODO
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			if (gqlResponse.Data.repository.pullRequests.nodes != null)
			{
				return ToPullRequestInfo(gqlResponse.Data.repository.pullRequests.nodes[0]);
			}
		}
		return null;
	}

	public IEnumerable<PullRequestInfo>ListPullRequests()
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_owner: String!, $_name: String!, $_count: Int !)
			{
				repository(owner: $_owner, name: $_name)
				{
					id,
					name,
					url,
					pullRequests(orderBy: {direction : DESC, field : UPDATED_AT}, first: $_count)
					{
						nodes
						{
							baseRefName,
							headRefName,
							number,
							bodyText,
							permalink,
							resourcePath,
							state,
							title,
							url,
						}
					}
				}
			} ",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
				_count = 100
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			var list = new List<PullRequestInfo>();

			if (gqlResponse.Data.repository.pullRequests.nodes != null)
			{
				foreach(var n in gqlResponse.Data.repository.pullRequests.nodes)
				{
					list.Add(ToPullRequestInfo(n));
				}
			}

			return list as IEnumerable<PullRequestInfo>;
		}
		return null;
	}

	private PullRequestInfo? ToPullRequestInfo(dynamic gqlData)
	{
		var pr = new PullRequestInfo();
		pr.Url = gqlData.url;
		pr.Number = gqlData.number;
		pr.BaseRef = gqlData.baseRefName;
		pr.HeadRef = gqlData.headRefName;
		pr.Permalink = gqlData.permalink;
		pr.ResourcePath = gqlData.resourcePath;
		// pr.State = gqlData.state; //TODO: Enum.Parse
		pr.Title = gqlData.title;
		pr.Body = gqlData.bodyText;

		return pr;
	}

	///////////////////////////////////////////////////////////////////////////

	public RepoInfo? GetRepo()
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_owner: String!, $_name: String!)
			{
				repository(owner: $_owner, name: $_name)
				{
					id,
					url,
					name,
					description,
					nameWithOwner,
				}
			} ",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToRepoInfo(gqlResponse.Data);
		}
		return null;
	}

	public IEnumerable<RepoInfo>ListRepos()
	{
		return new List<RepoInfo>() as IEnumerable<RepoInfo>;
	}

	private RepoInfo ToRepoInfo(dynamic gqlData)
	{
		var repo = new RepoInfo();
		repo.Id = gqlData.repository.id;
		repo.Url = gqlData.repository.url;
		repo.Name = gqlData.repository.name;
		repo.Path = gqlData.repository.nameWithOwner;
		repo.Description = gqlData.repository.description;
		return repo;
	}

	///////////////////////////////////////////////////////////////////////////

	public GistInfo? GetGist()
	{
		return new GistInfo();
	}

	public IEnumerable<GistInfo>ListGists()
	{
		return new List<GistInfo>() as IEnumerable<GistInfo>;
	}

	///////////////////////////////////////////////////////////////////////////
}
}
