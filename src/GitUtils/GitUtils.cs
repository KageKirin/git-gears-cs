using System;
using System.IO;
using System.Linq;

using LibGit2Sharp;

namespace git_gears
{
class GitUtils
{
	/// <summary>
	/// find the .git repo path for current repo
	/// </summary>
	/// <returns>path to .git</returns>
	static public string FindCurrentRepoPath(string currentPath = null)
	{
		var currentDir = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());
		while (currentDir != null)
		{
			string gitDirPath = null;

			/// support for bare/mirror repos
			if (currentDir.FullName.EndsWith(".git"))
			{
				gitDirPath = currentDir.FullName;
			}

			/// usual repo naming convention
			var gitDirs = currentDir.EnumerateDirectories(".git");
			if (gitDirs.Any())
			{
				gitDirPath = gitDirs.First().FullName;
			}

			if (gitDirPath != null)
			{
				return gitDirPath;
			}

			currentDir = currentDir.Parent;
		}
		return null;
	}

	public static string GetCurrentRemote()
	{
		string repoPath = FindCurrentRepoPath();
		using(var repo = (repoPath != null) ? new Repository(repoPath) : new Repository())
		{
			if (repo.Head.RemoteName != null)
				return repo.Head.RemoteName;

			if (repo.Network.Remotes.Any(r => r.Name == "origin"))
				return "origin";

			return repo.Network.Remotes.First().Name;
		}
	}

	public static string GetCurrentBranch()
	{
		string repoPath = FindCurrentRepoPath();
		if (repoPath == null)
		{
			return null;
		}

		using(var repo = new Repository(repoPath))
		{
			return repo.Head.FriendlyName;
		}
	}

	/// <summary>
	/// Retrieves the git-config entry for the given key
	/// by iterating the configs from local -> global -> system
	/// </summary>
	/// <returns>value of key or null</returns>

	public static string GetConfigEntry(string key, bool checkLocalConfig = true)
	{
		string repoPath = FindCurrentRepoPath();
		using(var repo = (repoPath != null) ? new Repository(repoPath) : new Repository())
		{
			Console.WriteLine($"repo: {repo != null}");
			foreach(var configLevel in Enum.GetValues(typeof (ConfigurationLevel)).Cast<ConfigurationLevel>().Reverse())
			{
				Console.WriteLine($"checking {configLevel}");
				// skip local config when told so
				if (configLevel == ConfigurationLevel.Local && !checkLocalConfig)
					continue;

				if (repo.Config.HasConfig(configLevel))
				{
					Console.WriteLine("has config");
					foreach(var entry in repo.Config.Find(key, configLevel))
					{
						Console.WriteLine($"{entry}");
						return entry.Value;
					}
				}
				else
				{
					Console.WriteLine("NO config");
				}
			}
		}

		using(var repo = new Repository())
		{
			if (repo.Config.HasConfig(ConfigurationLevel.Global))
			{
				foreach(var entry in repo.Config.Find(key, ConfigurationLevel.Global))
				{
					Console.WriteLine($"{entry}");
					return entry.Value;
				}
			}
		}


		return null;
	}

	/// <summary>
	/// Retrieves the git-config 'gears' entry for the given key and remote
	/// gears entries must be set as follows in git-config
	/// ```
	/// [gears "host.name"]
	/// 	key = value
	/// ```
	/// </summary>
	/// <returns>value of key or null</returns>
	public static string GetGearsConfigEntry(string key, string remoteName)
	{
		Console.WriteLine($"GetGearsConfigEntry({key}, {remoteName})");
		remoteName = remoteName ?? GetCurrentRemote();
		Console.WriteLine($"{GitUrl.IsUrl(remoteName)}");

		if (GitUrl.IsUrl(remoteName))
		{
			var url = new GitUrl(remoteName);
			var gearsKey = $"gears.{url.Host}.{key}";

			var value = GetConfigEntry(gearsKey, false);
			Console.WriteLine($"retrieved {value} for {url} {gearsKey}");
			return value;
		}
		

		using(var repo = new Repository(FindCurrentRepoPath()))
		{
			var url = new GitUrl(repo.Network.Remotes[remoteName].Url);
			var gearsKey = $"gears.{url.Host}.{key}";

			return GetConfigEntry(gearsKey);
		}
	}

	/// <summary>
	/// Retrieves the git-config 'gears' GraphQL API token for the given remote
	/// Token must be set as follows:
	/// ```
	/// [gears "host.name"]
	/// 	token = tokenasgivenbyhostapi
	/// ```
	/// </summary>
	/// <returns>value of key or null</returns>
	public static string GetGearsAuthBearerToken(string remoteName)
	{
		return GetGearsConfigEntry("token", remoteName);
	}

	/// <summary>
	/// Retrieves the git-config 'gears' GraphQL API Endpoint URL for the given remote
	/// Token must be set as follows:
	/// ```
	/// [gears "host.name"]
	/// 	url = https://service.com/graphql
	/// ```
	///
	/// Common endpoints are:
	/// https://api.github.com/graphql for public GitHub
	/// https://host.name/api/graphql for corporate GitHub Enterprise
	/// https://gitlab.com/api/graphql for public GitLab
	/// </summary>
	/// <returns>value of key or null</returns>
	public static string GetGearsEndpointUrl(string remoteName)
	{
		return GetGearsConfigEntry("url", remoteName);
	}

	/// <summary>
	/// Retrieves the git-config 'gears' REST API Endpoint root URL for the given remote
	/// Token must be set as follows:
	/// ```
	/// [gears "host.name"]
	/// 	rest = https://service.com/api/v3
	/// ```
	///
	/// Common endpoints are:
	/// https://api.github.com/v3 for public GitHub
	/// https://host.name/api/v3 for corporate GitHub Enterprise
	/// https://gitlab.com/api/v4 for public GitLab
	/// </summary>
	/// <returns>value of key or null</returns>
	public static string GetGearsRestEndpointUrl(string remoteName)
	{
		return GetGearsConfigEntry("rest", remoteName);
	}

	/// <summary>
	/// Defines a type for Gears API
	/// Currently, only GitHub and GitLab are defined and implemented
	/// </summary>
	public enum GearsApiType {
		Invalid,
		GitHub,
		GitLab,
	}

	/// <summary>
	/// Retrieves the typed Gears API Type for the given remote
	/// Token must be set as follows:
	/// ```
	/// [gears "host.name"]
	/// 	api = type
	/// ```
	///
	/// Valid values are:
	/// - github
	/// - gitlab
	/// </summary>
	/// <returns>value of key or Invalid</returns>
	public static GearsApiType GetGearsApiType(string remoteName)
	{
		Console.WriteLine($"GetGearsApiType: {remoteName}");
		const bool ignoreCase = true;
		try
		{
			return (GearsApiType) Enum.Parse(typeof (GearsApiType), GetGearsConfigEntry("api", remoteName), ignoreCase);
		}
		catch (ArgumentException)
		{
		}
		return GearsApiType.Invalid;
	}

	static public GitUrl GetRemoteUrl(string remote)
	{
		if (GitUrl.IsUrl(remote))
			return new GitUrl(remote);

		string repoPath = FindCurrentRepoPath();
		using(var repo = (repoPath != null) ? new Repository(repoPath) : new Repository())
		{
			var repoRemote = repo.Network.Remotes[remote];
			if (repoRemote != null)
				return new GitUrl(repoRemote.Url);
			return null;
		}
	}

	public static GitUrl GetCurrentRemoteUrl()
	{
		return GetRemoteUrl(GetCurrentRemote());
	}

	/// testing function
	public static void TestPrintInfo()
	{
		string repoPath = FindCurrentRepoPath();
		using(var repo = (repoPath != null) ? new Repository(repoPath) : new Repository())
		{
			foreach(var r in repo.Network.Remotes)
			{
				Console.WriteLine($"name: {r.Name}");
				Console.WriteLine($"url: {r.Url}");
				var gitUrl = new GitUrl(r.Url);
				Console.WriteLine($"host: {gitUrl.Host}");
				Console.WriteLine($"owner: {gitUrl.Owner}");
				Console.WriteLine($"name: {gitUrl.RepoName}");

				Console.WriteLine($"bearer token: {GetGearsAuthBearerToken(r.Name)}");
			}

			Console.WriteLine($"{repo.Head.CanonicalName}");
			Console.WriteLine($"{repo.Head.FriendlyName}");
			Console.WriteLine($"{repo.Head.RemoteName}");
			Console.WriteLine($"{repo.Head.UpstreamBranchCanonicalName}");
		}
	}

	public static void TestPrintConfig()
	{
		string repoPath = FindCurrentRepoPath();
		using(var repo = (repoPath != null) ? new Repository(repoPath) : new Repository())
		{
			Console.WriteLine(repo.Config.HasConfig(ConfigurationLevel.System));
			Console.WriteLine(repo.Config.HasConfig(ConfigurationLevel.Global));
			Console.WriteLine(repo.Config.HasConfig(ConfigurationLevel.Local));

			foreach(var c in repo.Config)
			{
				// Console.WriteLine("{0} -> {1}", c.Key, c.Value);
			}

			if (repo.Config.HasConfig(ConfigurationLevel.Local))
			{

				foreach(var c in repo.Config.Find(@"(gears).*", ConfigurationLevel.Local))
				{
					Console.WriteLine("{0} -> {1}", c.Key, c.Value);
				}
			}

			if (repo.Config.HasConfig(ConfigurationLevel.Global))
			{

				foreach(var c in repo.Config.Find(@"(gears).*", ConfigurationLevel.Global))
				{
					Console.WriteLine("{0} -> {1}", c.Key, c.Value);
				}
			}
		}
	}
}
}
