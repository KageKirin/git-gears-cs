using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using CommandLine;

namespace git_gears
{
class Program
{
	/// <summary>
	/// Program options
	/// </summary>
	public abstract class CommonOptions
	{
		[Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
		public bool Verbose
		{
			get;
			set;
		}
	}

	[Verb("foobar", HelpText = "foobar the hoge.")]
	class FoobarOptions : CommonOptions
	{
		[Option('h', "hoge", Required = false, HelpText = "hoge to foobar.")]
		public bool Hoge
		{
			get;
			set;
		}
	}

	[Verb("action", HelpText = "actionate the action.")]
	class ActionOptions : CommonOptions
	{
		[Option('a', "aaaa", Required = false, HelpText = "aaaa.")]
		public bool Aaaa
		{
			get;
			set;
		}
	}


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

	static int RunFoobar(FoobarOptions opts)
	{
		Console.WriteLine($"hoge: {opts.Hoge}");
		return 0;
	}

	static int RunAction(ActionOptions opts)
	{
		Console.WriteLine($"aaaa: {opts.Aaaa}");
		return 0;
	}

	static int Main(string[] args)
	{
		return Parser.Default
			.ParseArguments<FoobarOptions, ActionOptions>(args)
			.MapResult(
				(FoobarOptions opts) => RunFoobar(opts),
				(ActionOptions opts) => RunAction(opts),
				(errs) => 1
			);

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
