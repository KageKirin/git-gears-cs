using System.Collections.Generic;

namespace git_gears
{
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

public interface IGear
{
	void Test();

	GistInfo? GetGist(string name);					//! TODO: params
	IssueInfo? GetIssue(int number);				//! TODO: params
	PullRequestInfo? GetPullRequest(string branch); //! TODO: params
	RepoInfo? GetRepo();							//! TODO: params

	IEnumerable<GistInfo>ListGists();				//! TODO: params + filter params
	IEnumerable<IssueInfo>ListIssues();				//! TODO: params + filter params
	IEnumerable<PullRequestInfo>ListPullRequests(); //! TODO: params + filter params
	IEnumerable<RepoInfo>ListRepos();				//! TODO: params + filter params

	IssueInfo? CreateIssue(CreateIssueParams p);
	PullRequestInfo? CreatePullRequest(CreatePullRequestParams p);
}
}
