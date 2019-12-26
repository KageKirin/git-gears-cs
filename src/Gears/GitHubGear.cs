using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;

namespace git_gears
{
public class GitHubGear : CommonGear, IGear
{
	public GitHubGear(string remote) : base(remote)
	{
	}

	public void Test()
	{
		var gqlRequest = new GraphQLRequest{Query = @" query($_owner
															 : String !, $_name
															 : String !){repository(owner
																					: $_owner, name
																					: $_name){id, name, url}} ",
											Variables = new {
												_owner = RepoUrl.Owner,
												_name = RepoUrl.RepoName,
											}};
		Console.WriteLine($"{gqlRequest.Query}");
		Console.WriteLine($"{gqlRequest.Variables}");
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		Console.WriteLine($"{gqlResponse.Data != null}");

		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
		}
	}

	///////////////////////////////////////////////////////////////////////////

	public Issue GetIssue()
	{
		return new Issue();
	}

	public IEnumerable<Issue>ListIssues()
	{
		var gqlRequest =
			new GraphQLRequest{Query = @" query($_owner
												: String !, $_name
												: String !, $_count
												: Int !){repository(owner
																	: $_owner, name
																	: $_name){id, name, url,
																			  issues(first
																					 : $_count){nodes{
																				  number,
																				  bodyText,
																				  state,
																				  title,
																				  url,
																			  }}}} ",
							   Variables = new {_owner = RepoUrl.Owner, _name = RepoUrl.RepoName, _count = 100}};
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");

			if (gqlResponse.Data.repository.issues.nodes != null)
			{
				var list = new List<Issue>();
				foreach(var i in gqlResponse.Data.repository.issues.nodes)
				{
					list.Add(ToIssue(i));
				}
				return list as IEnumerable<Issue>;
			}
		}
		return null;
	}

	Issue ToIssue(dynamic gqlData)
	{
		var issue = new Issue();
		issue.Number = gqlData.number;
		// TODO: state
		issue.Url = gqlData.url;
		issue.Title = gqlData.title;
		issue.Body = gqlData.bodyText;
		return issue;
	}

	///////////////////////////////////////////////////////////////////////////

	public PullRequest GetPullRequest()
	{
		var gqlRequest = new GraphQLRequest{Query = @" query($_owner
															 : String !, $_name
															 : String !, $_branch
															 : String !){repository(owner
																					: $_owner, name
																					: $_name){
												id, name, url,
												pullRequests(headRefName
															 : $_branch, orderBy
															 : {direction : DESC, field : UPDATED_AT}, first : 1){nodes{
													baseRefName,
													headRefName,
													number,
													bodyText,
													permalink,
													resourcePath,
													state,
													title,
													url,
												}}}} ",
											Variables = new {
												_owner = RepoUrl.Owner, _name = RepoUrl.RepoName,
												_branch = "getlist-impl", // TODO
											}};
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			if (gqlResponse.Data.repository.pullRequests.nodes != null)
			{
				return ToPullRequest(gqlResponse.Data.repository.pullRequests.nodes[0]);
			}
		}
		return null;
	}

	public IEnumerable<PullRequest>ListPullRequests()
	{
		var gqlRequest = new GraphQLRequest{
			Query = @" query($_owner
							 : String !, $_name
							 : String !, $_count
							 : Int !){repository(owner
												 : $_owner, name
												 : $_name){id, name, url,
														   pullRequests(orderBy
																		: {direction : DESC, field : UPDATED_AT}, first
																		: $_count){nodes{
															   baseRefName,
															   headRefName,
															   number,
															   bodyText,
															   permalink,
															   resourcePath,
															   state,
															   title,
															   url,
														   }}}} ",
			Variables = new {_owner = RepoUrl.Owner, _name = RepoUrl.RepoName, _count = 100}};
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			var list = new List<PullRequest>();

			if (gqlResponse.Data.repository.pullRequests.nodes != null)
			{
				foreach(var n in gqlResponse.Data.repository.pullRequests.nodes)
				{
					list.Add(ToPullRequest(n));
				}
			}

			return list as IEnumerable<PullRequest>;
		}
		return null;
	}

	private PullRequest ToPullRequest(dynamic gqlData)
	{
		var pr = new PullRequest();
		pr.Url = gqlData.url;
		pr.Number = gqlData.number;
		pr.BaseRef = gqlData.baseRefName;
		pr.HeadRef = gqlData.headRefName;
		pr.Permalink = gqlData.permalink;
		pr.ResourcePath = gqlData.resourcePath;
		// pr.State = gqlData.state; //TODO: Enum.Parse
		pr.Title = gqlData.title;
		pr.Body = gqlData.bodyText;

		return pr;
	}

	///////////////////////////////////////////////////////////////////////////

	public Repo GetRepo()
	{
		var gqlRequest = new GraphQLRequest{Query = @" query($_owner
															 : String !, $_name
															 : String !){repository(owner
																					: $_owner, name
																					: $_name){
												id,
												url,
												name,
												description,
												nameWithOwner,
											}} ",
											Variables = new {
												_owner = RepoUrl.Owner,
												_name = RepoUrl.RepoName,
											}};
		GraphQLResponse gqlResponse = Client.PostAsync(gqlRequest).Result;
		if (gqlResponse.Data != null)
		{
			Console.WriteLine($"{gqlResponse.Data.ToString()}");
			return ToRepo(gqlResponse.Data);
		}
		return null;
	}

	public IEnumerable<Repo>ListRepos()
	{
		return new List<Repo>() as IEnumerable<Repo>;
	}

	private Repo ToRepo(dynamic gqlData)
	{
		var repo = new Repo();
		repo.Id = gqlData.repository.id;
		repo.Url = gqlData.repository.url;
		repo.Name = gqlData.repository.name;
		repo.Path = gqlData.repository.nameWithOwner;
		repo.Description = gqlData.repository.description;
		return repo;
	}

	///////////////////////////////////////////////////////////////////////////

	public Gist GetGist()
	{
		return new Gist();
	}

	public IEnumerable<Gist>ListGists()
	{
		return new List<Gist>() as IEnumerable<Gist>;
	}

	///////////////////////////////////////////////////////////////////////////
}
}
