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
		GqlClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v4+json");
	}

	public void Test()
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query
			{
				viewer
				{
					login
				}
			}",
			//Variables = new {}
		};
		// clang-format on
		Console.WriteLine($"{gqlRequest.Query}");
		// Console.WriteLine($"{gqlRequest.Variables}");
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		Console.WriteLine($"{gqlResponse.Data != null}");

		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
		}
	}

	///////////////////////////////////////////////////////////////////////////

	public IssueInfo? GetIssue(int number)
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_owner: String!, $_name: String!, $_number: Int !)
			{
				repository(owner: $_owner, name: $_name)
				{
					id,
					name,
					url,
					issue(number: $_number)
					{
						number,
						bodyText,
						state,
						title,
						url,
					}
				}
			} ",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
				_number = number,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			if (gqlResponse.Data.repository.issue != null)
			{
				return ToIssueInfo(gqlResponse.Data.repository.issue);
			}
		}
		return null;
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
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
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

	public PullRequestInfo? GetPullRequest(string branch)
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
				_branch = branch,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			if (gqlResponse.Data.repository.pullRequests.nodes != null
			&&  gqlResponse.Data.repository.pullRequests.nodes.Count > 0)
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
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
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

	private PullRequestInfo ToPullRequestInfo(dynamic gqlData)
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
					createdAt,
					pushedAt,
				}
			} ",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToRepoInfo(gqlResponse.Data.repository);
		}
		return null;
	}

	public IEnumerable<RepoInfo>ListRepos()
	{
		// TODO: count first, then query while iterating to get full count
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_owner: String!, $_count: Int!)
			{
				user(login: $_owner)
				{
					repositoriesContributedTo(first: $_count)
					{
						nodes
						{
							id,
							url,
							name,
							description,
							nameWithOwner,
							createdAt,
							pushedAt,
						}
					}
				}
			}",
			Variables = new {
				_owner = RepoUrl.Owner,
				_count = 100,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			var list = new List<RepoInfo>();

			if (gqlResponse.Data.user.repositoriesContributedTo.nodes != null)
			{
				foreach(var n in gqlResponse.Data.user.repositoriesContributedTo.nodes)
				{
					list.Add(ToRepoInfo(n));
				}
			}

			return list as IEnumerable<RepoInfo>;
		}
		return null;
	}

	private RepoInfo ToRepoInfo(dynamic gqlData)
	{
		var repo = new RepoInfo();
		repo.Id = gqlData.id;
		repo.Url = gqlData.url;
		repo.Name = gqlData.name;
		repo.Path = gqlData.nameWithOwner;
		repo.Description = gqlData.description;
		return repo;
	}

	///////////////////////////////////////////////////////////////////////////

	public GistInfo? GetGist(string name)
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_owner: String!, $_name:String!)
			{
				user(login: $_owner)
				{
					gist(name:$_name)
					{
						id,
						name,
						description,
						createdAt,
						pushedAt,
						files
						{
							name,
							text,
						}
					}
				}
			}",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = name,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToGistInfo(gqlResponse.Data.user.gist);
		}
		return null;
	}

	public IEnumerable<GistInfo>ListGists()
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_owner: String!, $_count: Int!)
			{
				user(login: $_owner)
				{
					gists(first: $_count)
					{
						nodes
						{
							id,
							name,
							description,
							createdAt,
							pushedAt,
							files
							{
								name,
								text,
							}
						}
					}
				}
			}",
			Variables = new {
				_owner = RepoUrl.Owner,
				_count = 100,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			var list = new List<GistInfo>();

			if (gqlResponse.Data.user.gists.nodes != null)
			{
				foreach(var n in gqlResponse.Data.user.gists.nodes)
				{
					list.Add(ToGistInfo(n));
				}
			}

			return list as IEnumerable<GistInfo>;
		}
		return null;
	}

	private GistInfo ToGistInfo(dynamic gqlData)
	{
		var gist = new GistInfo();
		gist.Id = gqlData.id;
		gist.Name = gqlData.name;
		gist.Description = gqlData.description;
		gist.CreatedAt = gqlData.createdAt;
		gist.PushedAt = gqlData.pushedAt;
		return gist;
	}

	///////////////////////////////////////////////////////////////////////////
}
}
