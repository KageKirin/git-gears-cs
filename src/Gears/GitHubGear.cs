using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Flurl;
using Flurl.Http;

namespace git_gears
{
public class GitHubGear : CommonGear, IGear
{
	public GitHubGear(string remote) : base(remote)
	{
		GqlClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v4+json");
		FlurlClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
	}

	public async Task TestAsync()
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
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		Console.WriteLine($"{gqlResponse.Data != null}");

		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}
	}

	///////////////////////////////////////////////////////////////////////////

	public async Task<UserInfo?> GetUserAsync(string login)
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_login: String!)
			{
				user(login: $_login)
				{
					id,
					name,
					login,
					email,
					url,

					bio,
					company,
					websiteUrl,
				}
			}",
			Variables = new {
				_login = login
			}
		};
		// clang-format on
		Console.WriteLine($"{gqlRequest.Query}");
		// Console.WriteLine($"{gqlRequest.Variables}");
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		Console.WriteLine($"{gqlResponse.Data != null}");

		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToUserInfo(gqlResponse.Data);
		}
		else if (gqlResponse.Errors != null && gqlResponse.Data.user != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}

		return null;
	}

	UserInfo ToUserInfo(dynamic gqlData)
	{
		var userInfo = new UserInfo();
		userInfo.Id = gqlData.user.id;
		userInfo.Name = gqlData.user.name;
		userInfo.Login = gqlData.user.login;
		userInfo.Email = gqlData.user.email;
		userInfo.Url = gqlData.user.url;
		userInfo.Bio = gqlData.user.bio;
		userInfo.Company = gqlData.user.company;
		userInfo.Website = gqlData.user.websiteUrl;
		return userInfo;
	}

	public async Task<OrganizationInfo?> GetOrganizationAsync(string login)
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_login: String!)
			{
				organization(login: $_login)
				{
					id,
					name,
					login,
					email,
					url,
					description

					projectsUrl,
					websiteUrl,
				}
			}",
			Variables = new {
				_login = login
			}
		};
		// clang-format on
		Console.WriteLine($"{gqlRequest.Query}");
		// Console.WriteLine($"{gqlRequest.Variables}");
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		Console.WriteLine($"{gqlResponse.Data != null}");

		if (gqlResponse.Data != null && gqlResponse.Data.organization != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToOrganizationInfo(gqlResponse.Data);
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}

		return null;
	}

	OrganizationInfo ToOrganizationInfo(dynamic gqlData)
	{
		var orgInfo = new OrganizationInfo();
		orgInfo.Id = gqlData.organization.id;
		orgInfo.Name = gqlData.organization.name;
		orgInfo.Login = gqlData.organization.login;
		orgInfo.Email = gqlData.organization.email;
		orgInfo.Url = gqlData.organization.url;
		orgInfo.Description = gqlData.organization.description;
		orgInfo.Website = gqlData.organization.websiteUrl;
		return orgInfo;
	}

	public async Task<OwnerInfo?> GetOwnerAsync(string login)
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			query($_login: String!)
			{
				organization(login: $_login)
				{
					id,
					name,
					login,
					email,
					url,
				}
				user(login: $_login)
				{
					id,
					name,
					login,
					email,
					url,
				}
			}",
			Variables = new {
				_login = login
			}
		};
		// clang-format on
		Console.WriteLine($"{gqlRequest.Query}");
		// Console.WriteLine($"{gqlRequest.Variables}");
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		Console.WriteLine($"{gqlResponse.Data != null}");

		if (gqlResponse.Data != null && (gqlResponse.Data.organization != null || gqlResponse.Data.user != null))
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToOwnerInfo(gqlResponse.Data);
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}

		return null;
	}

	OwnerInfo ToOwnerInfo(dynamic gqlData)
	{
		var ownerInfo = new OwnerInfo();
		if (gqlData.organization != null)
		{
			ownerInfo.Id = gqlData.organization.id;
			ownerInfo.Name = gqlData.organization.name;
			ownerInfo.Login = gqlData.organization.login;
			ownerInfo.Url = gqlData.organization.url;
		}
		else if (gqlData.user != null)
		{
			ownerInfo.Id = gqlData.user.id;
			ownerInfo.Name = gqlData.user.name;
			ownerInfo.Login = gqlData.user.login;
			ownerInfo.Url = gqlData.user.url;
		}
		return ownerInfo;
	}

	///////////////////////////////////////////////////////////////////////////

	public async Task<IssueInfo?> CreateIssueAsync(CreateIssueParams p)
	{
		var repo = await GetRepoAsync();
		if (repo.HasValue)
		{
			// clang-format off
			var gqlRequest = new GraphQLRequest{
				Query = @"
				mutation($_mutationId: String!, $_repositoryId: ID!, $_title: String!, $_body: String!)
				{
					createIssue(input: {
						clientMutationId: $_mutationId,
						title: $_title,
						body: $_body,
						repositoryId: $_repositoryId,
					})
					{
						clientMutationId,
						issue
						{
							number,
							bodyText,
							state,
							title,
							url,
						}
					}
				}",
				Variables = new {
					_mutationId = Guid.NewGuid().ToString(),
					_repositoryId = repo.Value.Id,
					_title = p.title,
					_body = p.body,
				}
			};
			// clang-format on
			GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
			if (gqlResponse.Data != null)
			{
				Console.WriteLine($"{gqlResponse.Data.ToString()}");
				if (gqlResponse.Data.createIssue.issue != null)
				{
					return ToIssueInfo(gqlResponse.Data.createIssue.issue);
				}
			}
			else
			{
				foreach (var e in gqlResponse.Errors)
				{
					Console.WriteLine($"GraphQL error: {e.Message}");
				}
			}
		}
		return null;
	}

	public async Task<IssueInfo?> GetIssueAsync(int number)
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
			}",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
				_number = number,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			if (gqlResponse.Data.repository.issue != null)
			{
				return ToIssueInfo(gqlResponse.Data.repository.issue);
			}
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}

		return null;
	}

	public async Task<IEnumerable<IssueInfo>> ListIssuesAsync()
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
			}",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
				_count = 100
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");

			if (gqlResponse.Data.repository.issues.nodes != null)
			{
				var list = new List<IssueInfo>();
				foreach (var i in gqlResponse.Data.repository.issues.nodes)
				{
					list.Add(ToIssueInfo(i));
				}
				return list as IEnumerable<IssueInfo>;
			}
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
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

	public async Task<PullRequestInfo?> CreatePullRequestAsync(CreatePullRequestParams p)
	{
		var repo = await GetRepoAsync();
		if (repo.HasValue)
		{
			// clang-format off
			var gqlRequest = new GraphQLRequest{
				Query = @"
				mutation($_mutationId: String!,
						 $_repositoryId: ID!,
						 $_branch: String!
						 $_targetBranch: String!,
						 $_title: String!,
						 $_body: String!)
				{
					createPullRequest(input: {
						clientMutationId: $_mutationId,
						repositoryId: $_repositoryId,
						headRefName: $_branch,
						baseRefName: $_targetBranch,
						title: $_title,
						body: $_body,
					})
					{
						clientMutationId,
						pullRequest
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
				}",
				Variables = new {
					_mutationId = Guid.NewGuid().ToString(),
					_repositoryId = repo.Value.Id,
					_branch = p.branch,
					_targetBranch = p.targetBranch,
					_title = p.title,
					_body = p.body,
				}
			};
			// clang-format on
			GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
			if (gqlResponse.Data != null)
			{
				Console.WriteLine($"{gqlResponse.Data.ToString()}");
				if (gqlResponse.Data.createPullRequest.pullRequest != null)
				{
					return ToPullRequestInfo(gqlResponse.Data.createPullRequest.pullRequest);
				}
			}
			else
			{
				foreach (var e in gqlResponse.Errors)
				{
					Console.WriteLine($"GraphQL error: {e.Message}");
				}
			}
		}
		return null;
	}

	public async Task<PullRequestInfo?> GetPullRequestAsync(string branch)
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
			}",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
				_branch = branch,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			if (gqlResponse.Data.repository.pullRequests.nodes != null &&
				gqlResponse.Data.repository.pullRequests.nodes.Count > 0)
			{
				return ToPullRequestInfo(gqlResponse.Data.repository.pullRequests.nodes[0]);
			}
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}

		return null;
	}

	public async Task<IEnumerable<PullRequestInfo>> ListPullRequestsAsync()
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
			}",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
				_count = 100
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			var list = new List<PullRequestInfo>();

			if (gqlResponse.Data.repository.pullRequests.nodes != null)
			{
				foreach (var n in gqlResponse.Data.repository.pullRequests.nodes)
				{
					list.Add(ToPullRequestInfo(n));
				}
			}

			return list as IEnumerable<PullRequestInfo>;
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
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

	public async Task<RepoInfo?> CreateRepoAsync(CreateRepoParams p)
	{
		var owner = await GetOwnerAsync(RepoUrl.Owner);
		if (owner.HasValue)
		{
			Console.WriteLine($"owner: {owner.Value.Name} ({owner.Value.Id})");
			// clang-format off
			var gqlRequest = new GraphQLRequest{
				Query = @"
				mutation($_mutationId: String!,
						 $_ownerId: ID!,
						 $_name: String!,
						 $_visibility: RepositoryVisibility!,
						 $_description: String,
						 $_homepageUrl: URI)
				{
					createRepository(input: {
						clientMutationId: $_mutationId,
						ownerId: $_ownerId,
						name: $_name,
						visibility: $_visibility,
						description: $_description,
						homepageUrl: $_homepageUrl,
					})
					{
						clientMutationId,
						repository
						{
							id,
							url,
							name,
							description,
							homepageUrl,
							nameWithOwner,
							createdAt,
							pushedAt,
						}
					}
				}",
				Variables = new {
					_mutationId = Guid.NewGuid().ToString(),
					_ownerId = owner.Value.Id,
					_name = this.RepoUrl.RepoName,
					_visibility = p.isPublic ? "PUBLIC" : "PRIVATE",
					_description = p.description,
					_homepageUrl = p.homepage,
				}
			};
			// clang-format on
			Console.WriteLine($"{gqlRequest.Query}");
			GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
			if (gqlResponse.Data != null && gqlResponse.Data.createRepository != null)
			{
				Console.WriteLine($"{gqlResponse.Data.ToString()}");
				if (gqlResponse.Data.createRepository.repository != null)
				{
					return ToRepoInfo(gqlResponse.Data.createRepository.repository);
				}
			}
			else if (gqlResponse.Errors != null)
			{
				foreach (var e in gqlResponse.Errors)
				{
					Console.WriteLine($"GraphQL error: {e.Message}");
				}
			}
		}
		return null;
	}

	public async Task<RepoInfo?> GetRepoAsync()
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
					homepageUrl,
					nameWithOwner,
					createdAt,
					pushedAt,
				}
			}",
			Variables = new {
				_owner = RepoUrl.Owner,
				_name = RepoUrl.RepoName,
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToRepoInfo(gqlResponse.Data.repository);
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}
		return null;
	}

	public async Task<IEnumerable<RepoInfo>> ListReposAsync()
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
							homepageUrl,
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
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			var list = new List<RepoInfo>();

			if (gqlResponse.Data.user.repositoriesContributedTo.nodes != null)
			{
				foreach (var n in gqlResponse.Data.user.repositoriesContributedTo.nodes)
				{
					list.Add(ToRepoInfo(n));
				}
			}

			return list as IEnumerable<RepoInfo>;
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
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
		repo.Homepage = gqlData.homepageUrl;
		return repo;
	}

	///////////////////////////////////////////////////////////////////////////

	public async Task<GistInfo?> CreateGistAsync(CreateGistParams p)
	{
		try
		{
			// dynamic queryData = new {
			//	description = p.title,
			//	files = new {
			//		gist = new {content = p.body}
			//	}
			//};
			// clang-format off
			string query = @"{"															   //
						   + @"""description"" : " + $"{JsonConvert.ToString($"{p.title} -- {p.description}")},"   //
						   + @"""public"" : " + $"{p.isPublic.ToString().ToLower()},"	   //
						   + @"""files"" : {"											   //
						   + $"  {JsonConvert.ToString(p.filename)} : "					   //
						   + @"{""content"" : " + $"{JsonConvert.ToString(p.body)}"		   //
						   + @"  }"														   //
						   + @" }"														   //
						   + @"}";														   //
			// clang-format on
			dynamic rstResponse = JObject.Parse(await(await RestEndpoint.WithClient(FlurlClient)
														  .AppendPathSegments("gists")
														  .PostJsonAsync((object) JObject.Parse(query)))
													.Content.ReadAsStringAsync());
			if (rstResponse != null)
			{
				rstResponse.url = rstResponse.html_url;
				return ToGistInfo(rstResponse);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine($"The request failed with following error: {e}.");
		}
		return null;
	}

	public async Task<GistInfo?> GetGistAsync(string name)
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
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		if (gqlResponse.Data != null)
		{
			gqlResponse.Data.user.gist.url =
				$"https://gist.{RepoUrl.Host}/{RepoUrl.Owner}/{gqlResponse.Data.user.gist.name}";
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToGistInfo(gqlResponse.Data.user.gist);
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}
		return null;
	}

	public async Task<IEnumerable<GistInfo>> ListGistsAsync()
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
		GraphQLResponse gqlResponse = await GqlClient.PostAsync(gqlRequest);
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			var list = new List<GistInfo>();

			if (gqlResponse.Data.user.gists.nodes != null)
			{
				foreach (var n in gqlResponse.Data.user.gists.nodes)
				{
					n.url = $"https://gist.{RepoUrl.Host}/{RepoUrl.Owner}/{n.name}";
					list.Add(ToGistInfo(n));
				}
			}

			return list as IEnumerable<GistInfo>;
		}
		else if (gqlResponse.Errors != null)
		{
			foreach (var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}

		return null;
	}

	private GistInfo ToGistInfo(dynamic gqlData)
	{
		var gist = new GistInfo();
		gist.Id = gqlData.id;
		gist.Name = gqlData.name;
		gist.Url = gqlData.url;
		gist.Description = gqlData.description;
		gist.CreatedAt = gqlData.createdAt;
		gist.PushedAt = gqlData.pushedAt;
		return gist;
	}

	///////////////////////////////////////////////////////////////////////////
}
}
