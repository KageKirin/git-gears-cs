using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace git_gears
{
/// <summary>
/// Class for parsing a git repo URL into its components
/// exists b/c default parser `class Uri` fails on github URLs
/// </summary>
public class GitUrl
{
	static private List<Regex>UrlSchemas = new List<Regex>(){
		// generic patterns _adapted_ from git-url-parse (python)
		new Regex(@"^(?<protocol>https?|git|ssh|rsync)\://" + //
					  @"(?:(?<user>.+)@)*" +				  //
					  @"(?<hostname>[a-z0-9_.-]*)" +		  //
					  @"[:/]*" +							  //
					  @"(?<port>[\d]+){0,1}" +				  //
					  @"(?<path>\/((?<owner>[\w\-]+)\/)?" +	  //
					  @"((?<reponame>[\w\-\.]+?)(\.git|\/)?)?)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"(git\+)?" +						   //
					  @"((?<protocol>\w+)://)" +	   //
					  @"((?<user>\w+)@)?" +			   //
					  @"((?<hostname>[\w\.\-]+))" +	   //
					  @"(:(?<port>\d+))?" +			   //
					  @"(?<path>(\/(?<owner>\w+)/)?" + //
					  @"(\/?(?<reponame>[\w\-]+)(\.git|\/)?)?)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"^(?:(?<user>.+)@)*" +				 //
					  @"(?<hostname>[a-z0-9_.-]*)[:]*" + //
					  @"(?<port>[\d]+){0,1}" +			 //
					  @"(?<path>\/?(?<owner>.+)/(?<reponame>.+).git)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"((?<user>\w+)@)?" +				 //
					  @"((?<hostname>[\w\.\-]+))" +	 //
					  @"[\:\/]{1,2}" +				 //
					  @"(?<path>((?<owner>\w+)/)?" + //
					  @"((?<reponame>[\w\-]+)(\.git|\/)?)?)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),

		// service specific patterns adapted from giturlparse
		// base
		new Regex(@"(?<user>.+)s@(?<hostname>.+)s:(?<reponame>.+)s.git",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"(http://(?<hostname>.+)s/(?<reponame>.+)s.git)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"(http://(?<hostname>.+)s/(?<reponame>.+)s.git)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"^(git://(?<hostname>.+)s/(?<reponame>.+)s.git)$", RegexOptions.Compiled | RegexOptions.IgnoreCase),

		// assembla
		new Regex(@"^(git@(?<hostname>.+):(?<reponame>.+).git)$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"^(git://(?<hostname>.+)/(?<reponame>.+).git)$", RegexOptions.Compiled | RegexOptions.IgnoreCase),

		// bitbucket
		new Regex(@"^(https://(?<user>.+)@(?<hostname>.+)/(?<owner>.+)/(?<reponame>.+).git)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"^(git@(?<hostname>.+):(?<owner>.+)/(?<reponame>.+).git)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),

		// friendcode
		new Regex(@"^(https://(?<hostname>.+)/(?<owner>.+)@user/(?<reponame>.+).git)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),

		// github
		new Regex(@"^(https://(?<hostname>.+)/(?<owner>.+)/(?<reponame>.+).git)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"^(git@(?<hostname>.+):(?<owner>.+)/(?<reponame>.+).git)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"^(git://(?<hostname>.+)/(?<owner>.+)/(?<reponame>.+).git)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),

		// gitlab
		new Regex(@"^(https://(?<hostname>.+)/(?<owner>.+)/(?<reponame>.+).git)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"^(git@(?<hostname>.+):(?<owner>.+)/(?<reponame>.+).git)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new Regex(@"^(git://(?<hostname>.+)/(?<owner>.+)/(?<reponame>.+).git)$",
				  RegexOptions.Compiled | RegexOptions.IgnoreCase),
	};

	public GitUrl(string url)
	{
		foreach(var pattern in UrlSchemas)
		{
			Match match = pattern.Match(url);
			if (match.Success)
			{
				if (match.Groups.ContainsKey("protocol") && !string.IsNullOrEmpty(match.Groups["protocol"].Value))
				{
					Protocol = match.Groups["protocol"].Value;
				}

				if (match.Groups.ContainsKey("user") && !string.IsNullOrEmpty(match.Groups["user"].Value))
				{
					User = match.Groups["user"].Value;
				}

				if (match.Groups.ContainsKey("hostname") && !string.IsNullOrEmpty(match.Groups["hostname"].Value))
				{
					Host = match.Groups["hostname"].Value;
				}

				if (match.Groups.ContainsKey("port") && !string.IsNullOrEmpty(match.Groups["port"].Value))
				{
					Console.WriteLine($"port: {match.Groups[" port "].Value}");
					Port = int.Parse(match.Groups["port"].Value);
				}

				if (match.Groups.ContainsKey("path") && !string.IsNullOrEmpty(match.Groups["path"].Value))
				{
					Path = match.Groups["path"].Value;
				}

				if (match.Groups.ContainsKey("owner") && !string.IsNullOrEmpty(match.Groups["owner"].Value))
				{
					Owner = match.Groups["owner"].Value;
				}

				if (match.Groups.ContainsKey("reponame") && !string.IsNullOrEmpty(match.Groups["reponame"].Value))
				{
					RepoName = match.Groups["reponame"].Value;
				}

				// iterate until the important parts are set
				// i.e. break if they are set, else continue
				if (!(string.IsNullOrEmpty(Host) || string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(Owner) ||
					  string.IsNullOrEmpty(RepoName)))
					break;
			}
		}
	}

	public string Protocol
	{
		get;
	}
	public string User
	{
		get;
	}
	public string Host
	{
		get;
	}
	public int Port
	{
		get;
	}
	public string Path
	{
		get;
	}
	public string Owner
	{
		get;
	}
	public string RepoName
	{
		get;
	}
}
}