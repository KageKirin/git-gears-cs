using CommandLine;

namespace git_gears
{
public abstract class CommonGetOptions : CommonOptions
{
	[Option('o', "open", Required = false, HelpText = "Open in default browser.")]
	public bool OpenInBrowser
	{
		get;
		set;
	}

	[Option("full", Required = false, HelpText = "Print full information contents.")]
	public bool FullContents
	{
		get;
		set;
	}

	[Option('f', "format", Required = false, HelpText = "Change log format.")]
	public string LogFormat
	{
		get;
		set;
	}

	[Option("url-only", Required = false, HelpText = "Print URL only.")]
	public bool UrlOnly
	{
		get;
		set;
	}

	[Option("number-only", Required = false, HelpText = "Print number only.")]
	public bool NumberOnly
	{
		get;
		set;
	}

	[Option("latest", Required = false, HelpText = "Print most recent entry only")]
	public bool LatestContents
	{
		get;
		set;
	}

	[Option("reverse", Required = false, HelpText = "Print in reverse order.")]
	public bool ReverseContents
	{
		get;
		set;
	}
}
}
