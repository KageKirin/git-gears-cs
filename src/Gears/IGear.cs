using System.Collections.Generic;

namespace git_gears
{
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
}
}
