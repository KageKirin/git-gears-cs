using System.Collections.Generic;

namespace git_gears
{
public interface IGear
{
	void Test();

	GistInfo? GetGist();			   //! TODO: params
	IssueInfo? GetIssue();			   //! TODO: params
	PullRequestInfo? GetPullRequest(); //! TODO: params
	RepoInfo? GetRepo();			   //! TODO: params

	IEnumerable<GistInfo>ListGists();				//! TODO: params + filter params
	IEnumerable<IssueInfo>ListIssues();				//! TODO: params + filter params
	IEnumerable<PullRequestInfo>ListPullRequests(); //! TODO: params + filter params
	IEnumerable<RepoInfo>ListRepos();				//! TODO: params + filter params
}
}
