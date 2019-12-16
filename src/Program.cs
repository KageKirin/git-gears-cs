using System;
using LibGit2Sharp;
using CommandLine;

namespace git_gears
{
class Program
{
	static int Main(string[] args)
	{
		return Parser.Default
			.ParseArguments<GetGistOptions,			 //
							GetIssueOptions,		 //
							GetPullRequestOptions,	 //
							GetRepoOptions,			 //
							ListGistsOptions,		 //
							ListIssuesOptions,		 //
							ListPullRequestsOptions, //
							ListReposOptions>(args)
			.MapResult((GetGistOptions opts) => GetGist.Execute(opts), //
					   (GetIssueOptions opts) => GetIssue.Execute(opts),
					   (GetPullRequestOptions opts) => GetPullRequest.Execute(opts),
					   (GetRepoOptions opts) => GetRepo.Execute(opts),
					   (ListGistsOptions opts) => ListGists.Execute(opts),
					   (ListIssuesOptions opts) => ListIssues.Execute(opts),
					   (ListPullRequestsOptions opts) => ListPullRequests.Execute(opts),
					   (ListReposOptions opts) => ListRepos.Execute(opts), (errs) => 1);
	}
}
}
