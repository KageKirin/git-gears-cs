using System.Collections.Generic;

namespace git_gears
{
public interface IGear
{
	void Test();

	Gist GetGist();
	Issue GetIssue();
	PullRequest GetPullRequest();
	Repo GetRepo();

	IEnumerable<Gist> ListGists();
	IEnumerable<Issue> ListIssues();
	IEnumerable<PullRequest> ListPullRequests();
	IEnumerable<Repo> ListRepos();
}
}
