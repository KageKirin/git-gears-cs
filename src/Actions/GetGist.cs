using System;

namespace git_gears
{
public class GetGist
{
	public static int Execute(GetGistOptions opts)
	{
		Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
		return 0;
	}
}
}
