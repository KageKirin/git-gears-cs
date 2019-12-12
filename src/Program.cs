using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace git_gears
{
class Program
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
			Console.WriteLine(currentDir.FullName);
			var gitDirs = currentDir.EnumerateDirectories(".git");
			if (gitDirs.Count() > 0)
			{
				return gitDirs.First().FullName;
			}
			currentDir = currentDir.Parent;
		}
		return null;
	}

	static void Main(string[] args)
	{
		using(var repo = new Repository(FindCurrentRepoPath()))
		{
			Console.WriteLine(repo.Config.HasConfig(ConfigurationLevel.Global));
			Console.WriteLine(repo.Config.HasConfig(ConfigurationLevel.System));
			Console.WriteLine();

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

			foreach(var r in repo.Network.Remotes)
			{
				Console.WriteLine("{0} -> {1}", r.Name, r.Url);
			}
		}
	}
}
}
