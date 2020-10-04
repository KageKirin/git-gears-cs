using System.Collections.Generic;
using System.Threading.Tasks;

namespace git_gears
{
public struct CreateGistParams
{
	public string title;
	public string description;
	public string filename;
	public string body;
	public bool isRepoGist;
	public bool isPublic;
	//! TODO: more params

	public CreateGistParams(string title, string description, string filename, string body, bool isRepoGist,
							bool isPublic)
	{
		this.title = title;
		this.description = description;
		this.filename = filename;
		this.body = body;
		this.isRepoGist = isRepoGist;
		this.isPublic = isPublic;
	}
}

public struct CreateIssueParams
{
	public string title;
	public string body;
	//! TODO: more params

	public CreateIssueParams(string title, string body)
	{
		this.title = title;
		this.body = body;
	}
}

public struct CreatePullRequestParams
{
	public string branch;
	// public string targetRepo; //! TODO: figure out how to support cross-repo PRs
	public string targetBranch;
	public string title;
	public string body;
	public bool draft;

	public CreatePullRequestParams(string branch, string targetBranch, string title, string body, bool draft = false)
	{
		this.branch = branch;
		this.targetBranch = targetBranch;
		this.title = title;
		this.body = body;
		this.draft = draft;
	}
}

public struct CreateRepoParams
{
	public string description;
	public string homepage;
	public bool isPublic;
	//! TODO: more params

	public CreateRepoParams(string description, string homepage, bool isPublic)
	{
		this.description = description;
		this.homepage = homepage;
		this.isPublic = isPublic;
	}
}

public interface IGear
{
	Task TestAsync();

	Task<UserInfo?> GetUserAsync(string login);
	Task<OrganizationInfo?> GetOrganizationAsync(string login);
	Task<OwnerInfo?> GetOwnerAsync(string login);

	Task<GistInfo?> GetGistAsync(string name);				   //! TODO: params
	Task<IssueInfo?> GetIssueAsync(int number);				   //! TODO: params
	Task<PullRequestInfo?> GetPullRequestAsync(string branch); //! TODO: params
	Task<RepoInfo?> GetRepoAsync();							   //! TODO: params

	Task<IEnumerable<GistInfo>> ListGistsAsync();				//! TODO: params + filter params
	Task<IEnumerable<IssueInfo>> ListIssuesAsync();				//! TODO: params + filter params
	Task<IEnumerable<PullRequestInfo>> ListPullRequestsAsync(); //! TODO: params + filter params
	Task<IEnumerable<RepoInfo>> ListReposAsync();				//! TODO: params + filter params

	Task<GistInfo?> CreateGistAsync(CreateGistParams p);
	Task<IssueInfo?> CreateIssueAsync(CreateIssueParams p);
	Task<PullRequestInfo?> CreatePullRequestAsync(CreatePullRequestParams p);
	Task<RepoInfo?> CreateRepoAsync(CreateRepoParams p);
}
}
