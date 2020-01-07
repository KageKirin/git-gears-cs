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
			query ($_fullPath: ID!, $_iid: String!)
			{
				project(fullPath: $_fullPath)
				{
					id,
					name,
					webUrl,
					issue(iid: $_iid)
					{
						iid,
						title,
						description,
						webUrl,
					}
				}
			}",
			Variables = new {
				_fullPath = $"{RepoUrl.Owner}/{RepoUrl.RepoName}",
				_iid = $"{number}",
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			if (gqlResponse.Data.project.issue != null)
			{
				return ToIssueInfo(gqlResponse.Data.project.issue);
			}
		}
		return null;
	}

	public IEnumerable<IssueInfo>ListIssues()
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query ($_fullPath: ID!, $_count: Int!)
			{
				project(fullPath: $_fullPath)
				{
					id,
					name,
					webUrl,
					issues(first: $_count)
					{
						nodes
						{
							iid,
							title,
							description,
							webUrl,
						}
					}
				}
			}",
			Variables = new {
				_fullPath = $"{RepoUrl.Owner}/{RepoUrl.RepoName}",
				_count = 100
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");

			if (gqlResponse.Data.project.issues.nodes != null)
			{
				var list = new List<IssueInfo>();
				foreach(var i in gqlResponse.Data.project.issues.nodes)
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
		issue.Number = gqlData.iid;
		// TODO: state
		issue.Url = gqlData.webUrl;
		issue.Title = gqlData.title;
		issue.Body = gqlData.description;
		return issue;
	}

	///////////////////////////////////////////////////////////////////////////

	public PullRequestInfo? GetPullRequest(string branch)
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query ($_fullPath: ID!, $_branch: String!)
			{
				project(fullPath: $_fullPath)
				{
					id,
					name,
					webUrl,
					mergeRequests(filter:{targetBranch: $_branch})
					{
						nodes
						{
							id,
							targetBranch,
							sourceBranch,
							iid,
							description,
							mergeStatus,
							title,
							webUrl,
						}
					}
				}
			}",
			Variables = new {
				_fullPath = $"{RepoUrl.Owner}/{RepoUrl.RepoName}",
				_branch = branch,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			if (gqlResponse.Data.project.mergeRequests.nodes != null
			&&  gqlResponse.Data.project.mergeRequests.nodes.Count > 0)
			{
				return ToPullRequestInfo(gqlResponse.Data.project.mergeRequests.nodes[0]);
			}
		}
		return null;
	}

	public IEnumerable<PullRequestInfo>ListPullRequests()
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query ($_fullPath: ID!, $_count: Int!)
			{
				project(fullPath: $_fullPath)
				{
					id,
					name,
					webUrl,
					mergeRequests(first: $_count)
					{
						nodes
						{
							id,
							targetBranch,
							sourceBranch,
							iid,
							description,
							mergeStatus,
							title,
							webUrl,
						}
					}
				}
			}",
			Variables = new {
				_fullPath = $"{RepoUrl.Owner}/{RepoUrl.RepoName}",
				_count = 100
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			var list = new List<PullRequestInfo>();

			if (gqlResponse.Data.project.mergeRequests.nodes != null)
			{
				foreach(var n in gqlResponse.Data.project.mergeRequests.nodes)
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
		pr.Url = gqlData.webUrl;
		pr.Number = gqlData.iid;
		pr.BaseRef = gqlData.targetBranch;
		pr.HeadRef = gqlData.sourceBranch;
		pr.Permalink = gqlData.webUrl;
		pr.ResourcePath = gqlData.id;
		// pr.State = gqlData.mergeStatus; //TODO: Enum.Parse
		pr.Title = gqlData.title;
		pr.Body = gqlData.description;

		return pr;
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
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
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
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
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
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_gid: ID!)
			{
				currentUser
				{
					snippets(ids: [$_gid])
					{
						nodes
						{
							id,
							title,
							description,
							createdAt,
							updatedAt,
							fileName,
							content,
							webUrl,
						}
					}
				}
			}",
			Variables = new {
				//_fullPath = $"{RepoUrl.Owner}/{RepoUrl.RepoName}",
				_gid = name
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		Console.WriteLine($"{gqlResponse.Data != null}");
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToGistInfo(gqlResponse.Data["currentUser"].snippets.nodes[0]);
		}
		return null;
	}

	public IEnumerable<GistInfo>ListGists()
	{
		// TODO: count first, then query while iterating to get full count
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_count: Int!)
			{
				currentUser
				{
					snippets(first: $_count)
					{
						nodes
						{
							id,
							title,
							description,
							createdAt,
							updatedAt,
							fileName,
							content,
							webUrl,
						}
					}
				}
			}",
			Variables = new {
				//_owner = RepoUrl.Owner,
				_count = 100,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			var list = new List<GistInfo>();

			if (gqlResponse.Data["currentUser"].snippets.nodes != null)
			{
				foreach(var n in gqlResponse.Data["currentUser"].snippets.nodes)
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
		gist.Name = gqlData.fileName;
		gist.Description = gqlData.description;
		gist.CreatedAt = gqlData.createdAt;
		gist.PushedAt = gqlData.updatedAt;
		return gist;
	}

	///////////////////////////////////////////////////////////////////////////
}
}
