# git-gears

Git and git-host helper CLI tool.

Git Gears tries to bridge the gap between
a local git repository and remote git hosting services
by bringing service functionality straight to
the command line.

## Tech

git-gears is written in C# (.NET Core 3.x)
and (ulteriorly) compiled down to a native executable.

So far, git-gears uses:

- [libgit2sharp](https://github.com/libgit2/libgit2sharp) for Git integration
- GraphQL via [GraphQL.Client](https://github.com/graphql-dotnet/graphql-client) for Service interfacing
- [CommandLineParser](https://github.com/commandlineparser/commandline) for obvious reasons.

### GraphQL

Since both [GitHub](https://developer.github.com/v4/) and
[GitLab](https://docs.gitlab.com/ee/api/graphql/index.html)
expose a GraphQL interface,
git-gears uses the latter, instead of multiplying the dependencies on
hosting service specific libraries.

## Name

git-gears is built and named after my personal proof-of-concept
Python script [git-cogs](https://github.com/KageKirin/git-cog.py),
but makes its pronounciation easier for non-native english speakers.
(I.e. not sound like male genitalia).  
The image is still that about dented wheels interacting
in a smooth mechanical system.

## Status

- git integration with libgit2 works, config can be read.
- GitHub proof-of-concept GraphQL connection works
- GitLab proof-of-concept GraphQL connection works

## Planned features

- GitHub support
- GitLab support
- atm, support for other hosting services is not planned. I take pull requests (e.g. for BitBucket).
- pull requests:
  - `git gears create-pullrequest`
  - `git gears close-pullrequest`
  - `git gears get-pullrequest`
  - `git gears list-pullrequests`
  - `git gears comment-pullrequest`
  - `git gears merge-pullrequest`
- issues:
  - `git gears create-issue`
  - `git gears close-issue`
  - `git gears get-issue`
  - `git gears list-issues`
  - `git gears comment-issue`
- gists:
  - `git gears create-gist`
  - `git gears remove-gist`
  - `git gears get-gist`
  - `git gears list-gists`
  - `git gears comment-gist`
- repos:
  - `git gears create-repo`
  - `git gears delete-repo`
  - `git gears get-repo`
  - `git gears list-repos`
  - `git gears fork-repo`
