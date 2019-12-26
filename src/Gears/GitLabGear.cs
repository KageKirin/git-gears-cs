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
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		Console.WriteLine($"{gqlResponse.Data != null}");

		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
		}
	}

	///////////////////////////////////////////////////////////////////////////

	public IssueInfo? GetIssue(int number)
	{
		return null;
	}

	public IEnumerable<IssueInfo>ListIssues()
	{
		return new List<IssueInfo>() as IEnumerable<IssueInfo>;
	}

	///////////////////////////////////////////////////////////////////////////

	public PullRequestInfo? GetPullRequest(string branch)
	{
		return null;
	}

	public IEnumerable<PullRequestInfo>ListPullRequests()
	{
		return new List<PullRequestInfo>() as IEnumerable<PullRequestInfo>;
	}

	///////////////////////////////////////////////////////////////////////////

	public RepoInfo? GetRepo()
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_fullPath: ID!)
			{
				project(fullPath: $_fullPath)
				{
					id,
					webUrl,
					name,
					description,
					fullPath,
					createdAt,
					lastActivityAt,
				}
			}",
			Variables = new {
				_fullPath = $"{RepoUrl.Owner}/{RepoUrl.RepoName}",
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		Console.WriteLine($"{gqlResponse.Data != null}");
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToRepoInfo(gqlResponse.Data.project);
		}
		return null;
	}

	public IEnumerable<RepoInfo>ListRepos()
	{
		// TODO: count first, then query while iterating to get full count
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_owner: ID!, $_count: Int!)
			{
				namespace(fullPath: $_owner)
				{
					projects(first: $_count)
					{
						nodes
						{
							id,
							webUrl,
							name,
							description,
							fullPath,
							createdAt,
							lastActivityAt,
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
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			var list = new List<RepoInfo>();

			if (gqlResponse.Data["namespace"].projects.nodes != null)
			{
				foreach(var n in gqlResponse.Data["namespace"].projects.nodes)
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
		repo.Url = gqlData.webUrl;
		repo.Name = gqlData.name;
		repo.Path = gqlData.fullPath;
		repo.Description = gqlData.description;
		return repo;
	}

	///////////////////////////////////////////////////////////////////////////

	public GistInfo? GetGist(string name)
	{
		return null;
	}

	public IEnumerable<GistInfo>ListGists()
	{
		return new List<GistInfo>() as IEnumerable<GistInfo>;
	}

	///////////////////////////////////////////////////////////////////////////
}
}
