using System.Collections.Generic;

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
	void Test();

	UserInfo? GetUser(string login);
	OrganizationInfo? GetOrganization(string login);
	OwnerInfo? GetOwner(string login);

	GistInfo? GetGist(string name);					//! TODO: params
	IssueInfo? GetIssue(int number);				//! TODO: params
	PullRequestInfo? GetPullRequest(string branch); //! TODO: params
	RepoInfo? GetRepo();							//! TODO: params

	IEnumerable<GistInfo>ListGists();				//! TODO: params + filter params
	IEnumerable<IssueInfo>ListIssues();				//! TODO: params + filter params
	IEnumerable<PullRequestInfo>ListPullRequests(); //! TODO: params + filter params
	IEnumerable<RepoInfo>ListRepos();				//! TODO: params + filter params

	GistInfo? CreateGist(CreateGistParams p);
	IssueInfo? CreateIssue(CreateIssueParams p);
	PullRequestInfo? CreatePullRequest(CreatePullRequestParams p);
	RepoInfo? CreateRepo(CreateRepoParams p);
}
}
