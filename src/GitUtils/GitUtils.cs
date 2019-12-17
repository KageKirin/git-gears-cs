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
	static string FindCurrentRepoPath(string currentPath = null)
	{
		var currentDir = new DirectoryInfo((currentPath != null) ? currentPath : Directory.GetCurrentDirectory());
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
			if (gitDirs.Count() > 0)
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
		using(var repo = new Repository(FindCurrentRepoPath()))
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
		using(var repo = new Repository(FindCurrentRepoPath()))
		{
			return repo.Head.FriendlyName;
		}
	}

	public static string GetAuthBearerToken(string remoteName)
	{
		if (remoteName == null)
		{
			remoteName = GetCurrentRemote();
		}
		using(var repo = new Repository(FindCurrentRepoPath()))
		{
			var remoteUrl = repo.Network.Remotes[remoteName].Url;
			var remoteUri = new Uri(remoteUrl);
			//remoteUri.Host

			foreach(var configLevel in Enum.GetValues(typeof(ConfigurationLevel)).Cast<ConfigurationLevel>().Reverse())
			{
				if (repo.Config.HasConfig(configLevel))
				{
					//string gearsKey = $""
				}
			}
			return "";
		}
	}

	public static void TestPrintInfo()
	{
		using(var repo = new Repository(FindCurrentRepoPath()))
		{
			foreach(var r in repo.Network.Remotes)
			{
				Console.WriteLine($"name: {r.Name}");
				Console.WriteLine($"url: {r.Url}");
				var gitUrl = new GitUrl(r.Url);
				Console.WriteLine($"host: {gitUrl.Host}");
				Console.WriteLine($"owner: {gitUrl.Owner}");
				Console.WriteLine($"name: {gitUrl.RepoName}");

			}

			Console.WriteLine($"{repo.Head.CanonicalName}");
			Console.WriteLine($"{repo.Head.FriendlyName}");
			Console.WriteLine($"{repo.Head.RemoteName}");
			Console.WriteLine($"{repo.Head.UpstreamBranchCanonicalName}");
			
		}
	}

	public static void TestPrintConfig()
	{
		using(var repo = new Repository(FindCurrentRepoPath()))
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
