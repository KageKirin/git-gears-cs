using System;
using System.Threading.Tasks;
using LibGit2Sharp;
using CommandLine;

namespace git_gears
{
class Program
{
	static async Task<int> Main(string[] args)
	{
		return await Parser.Default
			.ParseArguments<			  //
				CreateGistOptions,		  //
				CreateIssueOptions,		  //
				CreatePullRequestOptions, //
				CreateRepoOptions,		  //
				GetGistOptions,			  //
				GetIssueOptions,		  //
				GetPullRequestOptions,	  //
				GetRepoOptions,			  //
				ListGistsOptions,		  //
				ListIssuesOptions,		  //
				ListPullRequestsOptions,  //
				ListReposOptions,		  //
				GetUserOptions,			  //
				GetOwnerOptions,		  //
				GetOrganizationOptions,	  //
				TestOptions				  //
				>(args)
			.MapResult(																	 //
				(CreateGistOptions opts) => CreateGist.ExecuteAsync(opts),				 //
				(CreateIssueOptions opts) => CreateIssue.ExecuteAsync(opts),			 //
				(CreatePullRequestOptions opts) => CreatePullRequest.ExecuteAsync(opts), //
				(CreateRepoOptions opts) => CreateRepo.ExecuteAsync(opts),				 //
				(GetGistOptions opts) => GetGist.ExecuteAsync(opts),					 //
				(GetIssueOptions opts) => GetIssue.ExecuteAsync(opts),					 //
				(GetPullRequestOptions opts) => GetPullRequest.ExecuteAsync(opts),		 //
				(GetRepoOptions opts) => GetRepo.ExecuteAsync(opts),					 //
				(ListGistsOptions opts) => ListGists.ExecuteAsync(opts),				 //
				(ListIssuesOptions opts) => ListIssues.ExecuteAsync(opts),				 //
				(ListPullRequestsOptions opts) => ListPullRequests.ExecuteAsync(opts),	 //
				(ListReposOptions opts) => ListRepos.ExecuteAsync(opts),				 //
				(GetUserOptions opts) => GetUser.ExecuteAsync(opts),					 //
				(GetOwnerOptions opts) => GetOwner.ExecuteAsync(opts),					 //
				(GetOrganizationOptions opts) => GetOrganization.ExecuteAsync(opts),	 //
				(TestOptions opts) => Test.ExecuteAsync(opts),							 //
				(errs) => Task.Run(() => 1));
	}
}
}
