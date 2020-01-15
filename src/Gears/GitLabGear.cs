using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Flurl;
using Flurl.Http;

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

	public UserInfo? GetUser(string login)
	{
		try
		{
			// search user -> id
			var rstResponse = JArray.Parse(RestEndpoint.WithClient(FlurlClient)
											   .AppendPathSegments("users")
											   .SetQueryParams(new {
												   username = login,
											   })
											   .GetStringAsync()
											   .Result);
			Console.WriteLine($"{rstResponse}");

			if (rstResponse.Count > 0)
			{
				dynamic userData = rstResponse[0];
				string userId = $"{userData.id}";
				// then get user data
				var rstResponseUser = JObject.Parse(
					RestEndpoint.WithClient(FlurlClient).AppendPathSegments("users", userId).GetStringAsync().Result);
				Console.WriteLine($"{rstResponseUser}");
				if (rstResponseUser != null)
				{
					return ToUserInfo(rstResponseUser);
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine($"The request failed with following error: {e}.");
		}

		return null;
	}

	UserInfo ToUserInfo(dynamic gqlData)
	{
		var userInfo = new UserInfo();
		userInfo.Id = gqlData.id;
		userInfo.Name = gqlData.name;
		userInfo.Login = gqlData.username;
		userInfo.Email = gqlData.public_email;
		userInfo.Url = gqlData.web_url;
		userInfo.Bio = gqlData.bio;
		userInfo.Company = gqlData.organization;
		userInfo.Website = gqlData.website_url;
		return userInfo;
	}

	public OrganizationInfo? GetOrganization(string login)
	{
		try
		{
			var rstResponse = JObject.Parse(RestEndpoint.WithClient(FlurlClient)
												.AppendPathSegments("groups", login) //
												.GetStringAsync()
												.Result);
			Console.WriteLine($"{rstResponse}");
			if (rstResponse != null)
			{
				return ToOrganizationInfo(rstResponse);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine($"The request failed with following error: {e}.");
		}

		return null;
	}

	OrganizationInfo ToOrganizationInfo(dynamic gqlData)
	{
		var orgInfo = new OrganizationInfo();
		orgInfo.Id = gqlData.id;
		orgInfo.Name = gqlData.full_name;
		orgInfo.Login = gqlData.name;
		orgInfo.Email = gqlData.email;
		orgInfo.Url = gqlData.web_url;
		orgInfo.Description = gqlData.description;
		orgInfo.Website = gqlData.web_url;
		return orgInfo;
	}

	public OwnerInfo? GetOwner(string login)
	{
		try
		{
			var rstResponse = JObject.Parse(RestEndpoint.WithClient(FlurlClient)
												.AppendPathSegments("namespaces", login) //
												.GetStringAsync()
												.Result);
			Console.WriteLine($"{rstResponse}");
			if (rstResponse != null)
			{
				return ToOwnerInfo(rstResponse);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine($"The request failed with following error: {e}.");
		}

		return null;
	}

	OwnerInfo ToOwnerInfo(dynamic gqlData)
	{
		var ownerInfo = new OwnerInfo();
		ownerInfo.Id = gqlData.id;
		ownerInfo.Name = gqlData.name;
		ownerInfo.Login = gqlData.full_path;
		ownerInfo.Url = gqlData.web_url;
		return ownerInfo;
	}

	///////////////////////////////////////////////////////////////////////////

	public IssueInfo? CreateIssue(CreateIssueParams p)
	{
		try
		{
			var rstResponse = JObject.Parse(RestEndpoint.WithClient(FlurlClient)
												.AppendPathSegments("projects", GetRepoProjectId(), "issues")
												.PostJsonAsync(new {
													//
													title = p.title,	 //
													description = p.body //
												})
												.Result.Content.ReadAsStringAsync()
												.Result);
			if (rstResponse != null)
			{
				return ToIssueInfo(rstResponse);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine($"The request failed with following error: {e}.");
		}

		return null;
	}

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
		else if (gqlResponse.Errors != null)
		{
			foreach(var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
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
		else if (gqlResponse.Errors != null)
		{
			foreach(var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}

		return null;
	}

	IssueInfo ToIssueInfo(dynamic gqlData)
	{
		var issue = new IssueInfo();
		issue.Number = gqlData.iid;
		// TODO: state
		issue.Url = gqlData.webUrl ?? gqlData.web_url;
		issue.Title = gqlData.title;
		issue.Body = gqlData.description;
		return issue;
	}

	///////////////////////////////////////////////////////////////////////////

	public PullRequestInfo? CreatePullRequest(CreatePullRequestParams p)
	{
		try
		{
			var rstResponse = JObject.Parse(RestEndpoint.WithClient(FlurlClient)
												.AppendPathSegments("projects", GetRepoProjectId(), "merge_requests")
												.PostJsonAsync(new {
													source_branch = p.branch,		//
													target_branch = p.targetBranch, //
													title = p.title,				//
													description = p.body			//
												})
												.Result.Content.ReadAsStringAsync()
												.Result);
			if (rstResponse != null)
			{
				return ToPullRequestInfo(rstResponse);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine($"The request failed with following error: {e}.");
		}

		return null;
	}

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
		string project_id = null;
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			string project_gid = gqlResponse.Data.project.id;
			project_id = GetProjectIdFromGid(project_gid);
			if (gqlResponse.Data.project.mergeRequests.nodes != null &&
				gqlResponse.Data.project.mergeRequests.nodes.Count > 0)
			{
				return ToPullRequestInfo(gqlResponse.Data.project.mergeRequests.nodes[0]);
			}
		}
		else if (gqlResponse.Errors != null)
		{
			foreach(var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}

		// workaround
		project_id = project_id ?? GetRepoProjectId();
		var rstResponse = JArray.Parse(RestEndpoint.WithClient(FlurlClient)
										   .AppendPathSegments("projects", project_id, "merge_requests")
										   .SetQueryParams(new {
											   source_branch = branch,
											   max_results = 1,
											   order_by = "updated_at",
											   sort = "desc",
										   })
										   .GetStringAsync()
										   .Result);

		Console.WriteLine($"{rstResponse}");
		if (rstResponse != null && rstResponse.Count > 0)
		{
			return ToPullRequestInfo(rstResponse[0]);
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
		string project_id = null;
		var list = new List<PullRequestInfo>();
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			string project_gid = gqlResponse.Data.project.id;
			project_id = GetProjectIdFromGid(project_gid);
			if (gqlResponse.Data.project.mergeRequests.nodes != null &&
				gqlResponse.Data.project.mergeRequests.nodes.Count > 0)
			{
				foreach(var n in gqlResponse.Data.project.mergeRequests.nodes)
				{
					list.Add(ToPullRequestInfo(n));
				}
				return list as IEnumerable<PullRequestInfo>;
			}
		}
		else if (gqlResponse.Errors != null)
		{
			foreach(var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}

		// workaround
		project_id = project_id ?? GetRepoProjectId();
		var rstResponse = JArray.Parse(RestEndpoint.WithClient(FlurlClient)
										   .AppendPathSegments("projects", project_id, "merge_requests")
										   .SetQueryParams(new {
											   max_results = 100,
											   order_by = "updated_at",
											   sort = "desc",
										   })
										   .GetStringAsync()
										   .Result);

		Console.WriteLine($"{rstResponse}");
		if (rstResponse != null && rstResponse.Count > 0)
		{
			foreach(var n in rstResponse)
			{
				list.Add(ToPullRequestInfo(n));
			}
			return list as IEnumerable<PullRequestInfo>;
		}

		return null;
	}

	private PullRequestInfo ToPullRequestInfo(dynamic gqlData)
	{
		var pr = new PullRequestInfo();
		pr.Url = gqlData.webUrl ?? gqlData.web_url;
		pr.Number = gqlData.iid;
		pr.BaseRef = gqlData.targetBranch ?? gqlData.target_branch;
		pr.HeadRef = gqlData.sourceBranch ?? gqlData.source_branch;
		pr.Permalink = gqlData.webUrl ?? gqlData.web_url;
		pr.ResourcePath = gqlData.id;
		// pr.State = gqlData.mergeStatus; //TODO: Enum.Parse
		pr.Title = gqlData.title;
		pr.Body = gqlData.description;

		return pr;
	}

	///////////////////////////////////////////////////////////////////////////

	private string GetProjectIdFromGid(string gid)
	{
		return Regex.Split(gid, @"gid://gitlab/Project/")[1];
	}

	private string GetRepoProjectId()
	{
		var repo = GetRepo();
		if (repo.HasValue)
		{
			return GetProjectIdFromGid(repo.Value.Id);
		}

		return "";
	}

	public RepoInfo? CreateRepo(CreateRepoParams p)
	{
		var owner = GetOwner(RepoUrl.Owner);
		if (owner.HasValue)
		{
			try
			{
				var rstResponse =
					JObject.Parse(RestEndpoint.WithClient(FlurlClient)
									  .AppendPathSegments("projects")
									  .PostJsonAsync(new {
										  name = this.RepoUrl.RepoName,
										  namespace_id = owner.Value.Id,
										  description = $"{p.description}\n{p.homepage}",
										  visibility = p.isPublic ? "public" : "private",
										  // good defaults
										  lfs_enabled = true, // or check whether LFS enabled in local repo
										  remove_source_branch_after_merge = true,
										  autoclose_referenced_issues = true,
									  })
									  .Result.Content.ReadAsStringAsync()
									  .Result);
				if (rstResponse != null)
				{
					return ToRepoInfo(rstResponse);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"The request failed with following error: {e}.");
			}
		}

		return null;
	}

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
		if (gqlResponse.Data != null && gqlResponse.Data.project != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToRepoInfo(gqlResponse.Data.project);
		}
		else if (gqlResponse.Errors != null)
		{
			foreach(var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
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
		else if (gqlResponse.Errors != null)
		{
			foreach(var e in gqlResponse.Errors)
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
		repo.Url = gqlData.webUrl;
		repo.Name = gqlData.name;
		repo.Path = gqlData.fullPath;
		repo.Description = gqlData.description;
		return repo;
	}

	///////////////////////////////////////////////////////////////////////////

	public GistInfo? CreateGist(CreateGistParams p)
	{
		// clang-format off
		var gqlRequest = new GraphQLRequest{
			Query = @"
			mutation($_mutationId: String!, $_title: String!, $_body: String!, $_visibility: VisibilityLevelsEnum!, $_description: String, $_filename: String, $_fullPath: ID)
			{
				createSnippet(input: {
					clientMutationId: $_mutationId,
					title: $_title,
					fileName: $_filename,
					description: $_description,
					visibilityLevel: $_visibility,
					content: $_body,
					projectPath: $_fullPath,
				})
				{
					clientMutationId,
					snippet
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
			}",
			Variables = new {
				_mutationId = Guid.NewGuid().ToString(),
				_title = p.title,
				_body = p.body,
				_description = p.description,
				_filename = p.filename,
				_visibility = p.isPublic ? "public" : "private",
				_fullPath = p.isRepoGist ? $"{RepoUrl.Owner}/{RepoUrl.RepoName}" : "",
			}
		};
		// clang-format on
		GraphQLResponse gqlResponse = GqlClient.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			if (gqlResponse.Data.createSnippet.snippet != null)
			{
				return ToGistInfo(gqlResponse.Data.createSnippet.snippet);
			}
		}
		else if (gqlResponse.Errors != null)
		{
			foreach(var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
		}

		return null;
	}

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
		else if (gqlResponse.Errors != null)
		{
			foreach(var e in gqlResponse.Errors)
			{
				Console.WriteLine($"GraphQL error: {e.Message}");
			}
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
		else if (gqlResponse.Errors != null)
		{
			foreach(var e in gqlResponse.Errors)
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
		gist.Name = gqlData.fileName ?? gqlData.file_name;
		gist.Url = gqlData.webUrl ?? gqlData.web_url;
		gist.Description = $"{gqlData.title} -- {gqlData.description}";
		gist.CreatedAt = gqlData.createdAt ?? gqlData.created_at;
		gist.PushedAt = gqlData.updatedAt ?? gqlData.updated_at;
		return gist;
	}

	///////////////////////////////////////////////////////////////////////////
}
}
