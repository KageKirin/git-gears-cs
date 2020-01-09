﻿using System;
using LibGit2Sharp;
using CommandLine;

namespace git_gears
{
class Program
{
	static int Main(string[] args)
	{
		return Parser.Default
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
				TestOptions				  //
				>(args)
			.MapResult(																//
				(CreateGistOptions opts) => CreateGist.Execute(opts),				//
				(CreateIssueOptions opts) => CreateIssue.Execute(opts),				//
				(CreatePullRequestOptions opts) => CreatePullRequest.Execute(opts), //
				(CreateRepoOptions opts) => CreateRepo.Execute(opts),				//
				(GetGistOptions opts) => GetGist.Execute(opts),						//
				(GetIssueOptions opts) => GetIssue.Execute(opts),					//
				(GetPullRequestOptions opts) => GetPullRequest.Execute(opts),		//
				(GetRepoOptions opts) => GetRepo.Execute(opts),						//
				(ListGistsOptions opts) => ListGists.Execute(opts),					//
				(ListIssuesOptions opts) => ListIssues.Execute(opts),				//
				(ListPullRequestsOptions opts) => ListPullRequests.Execute(opts),	//
				(TestOptions opts) => Test.Execute(opts),							//
				(ListReposOptions opts) => ListRepos.Execute(opts),					//
				(errs) => 1);
	}
}
}
