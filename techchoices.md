# git-gears -- Technological Choices

A little blog post to cover the technological
choices and considerations I took when starting with git-gears.

## Minimal binary size

I'm targeting to have a small binary size.  
With C# and .NET native, this might not be as minimal I'd love to have,
but it's a good compromise wrt the other considerations.

## Multiplatform deploy

Ideally, I want a "build-once, debug everywhere" kind of multiplatform deployability.
With "everywhere" meaning Windows, macOS and Linux.  
C# can be built for nearly any platform, hence the obvious choice.

## Minimal dependencies

In order to achieve the goal of "minimal binary size",
I can't afford to mindlessly require tons of dependencies.  
The (currently) 5 dependencies (libgit2, commandlineparser, graphql-client, flurl, json.net)
were carefully selected in this regard.

## Starting with C#

Another important consideration is that I'm basically just starting with C#:
I'm more used to writing C++ for compilation, with Python and Lua being
my predilections for scripting, so C# is rather new to me.
But this was a deliberate choice to learn the language more in-depth.


## So, why not XXX?

### Why not use libs that directly wrap the web APIs?

Sure, I could just have used [OctoKit.net](https://github.com/octokit/octokit.net)
and [GitLabApiClient](https://github.com/nmklotas/GitLabApiClient)
and wrapped the Gears class implmentations around them.
This surely would have sped up the development process, but added complex layers
of code that I don't _own_ (in the sense of, having a faint idea of what it does).

Which is why I chose to use lower level code to access the web APIs _by hand_,
rather than obliging yet another code dependency to do it.

### Why not write this in Rust?

Seriously, I considered this point for quite a while before starting.
While Rust would certainly be better at fulfilling my target considerations,
I had the impression that Rust is still in its infancy compared to C#.
I.e. the C# tooling (with VS) is pretty impressive, and for what I care for: stable.

### Why not write this in C or C++?

Yes, this would have been possible, especially with libgit2 being a C lib,
and curl being the go-to lib for everything HTTP.
I wanted to try something new.

