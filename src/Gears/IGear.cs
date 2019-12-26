using System.Collections.Generic;

namespace git_gears
{
public interface IGear
{
	void Test();

	Gist GetGist();				  //! TODO: params
	Issue GetIssue();			  //! TODO: params
	PullRequest GetPullRequest(); //! TODO: params
	Repo GetRepo();				  //! TODO: params

	IEnumerable<Gist>ListGists();				//! TODO: params + filter params
	IEnumerable<Issue>ListIssues();				//! TODO: params + filter params
	IEnumerable<PullRequest>ListPullRequests(); //! TODO: params + filter params
	IEnumerable<Repo>ListRepos();				//! TODO: params + filter params
}
}
