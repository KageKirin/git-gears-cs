
namespace git_gears
{
public abstract class CommonGear
{
	public CommonGear(string remote)
	{
		Token = GitUtils.GetGearsAuthBearerToken(remote);
	}

	protected string Token { get; }
}
}
