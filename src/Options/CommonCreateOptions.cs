using CommandLine;

namespace git_gears
{
public abstract class CommonCreateOptions : CommonOptions
{
	[Option('o', "open", Required = false, HelpText = "Open in default browser.")]
	public bool OpenInBrowser
	{
		get;
		set;
	}

	[Option('e', "editor", Required = false, HelpText = "Open $GITEDITOR to edit message.")]
	public bool OpenInEditor
	{
		get;
		set;
	}
}
}
