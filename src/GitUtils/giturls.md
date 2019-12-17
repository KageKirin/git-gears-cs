# About Git URLs

The following syntaxes may be used with them:

- `ssh://[user@]host.xz[:port]/path/to/repo.git/`
- `git://host.xz[:port]/path/to/repo.git/`
- `http[s]://host.xz[:port]/path/to/repo.git/`
- `ftp[s]://host.xz[:port]/path/to/repo.git/`

An alternative scp-like syntax may also be used with the ssh protocol:

- `[user@]host.xz:path/to/repo.git/`

This syntax is only recognized if there are no slashes before the first
colon. This helps differentiate a local path that contains a colon. For
example the local path `foo::bar` could be specified as an absolute path
or `..//foo::bar` to avoid being misinterpreted as an ssh url.

The ssh and git protocols additionally support ~username expansion:

- `ssh://[user@]host.xz[:port]/~[user]/path/to/repo.git/`
- `git://host.xz[:port]/~[user]/path/to/repo.git/`
- `[user@]host.xz:/~[user]/path/to/repo.git/`

For local repositories, also supported by Git natively, the following
syntaxes may be used:

- `/path/to/repo.git/`
- `file:///path/to/repo.git/`


## Valid URLs

from [SO](https://stackoverflow.com/questions/2514859/regular-expression-for-git-repository)

Remote host paths:

- `user@host.xz:path/to/repo.git`
- `user@host.xz:~user/path/to/repo.git/`
- `user@host.xz:/path/to/repo.git/`
- `ssh://user@host.xz/path/to/repo.git/`
- `ssh://user@host.xz/~user/path/to/repo.git/`
- `ssh://user@host.xz/~/path/to/repo.git`
- `ssh://user@host.xz:port/path/to/repo.git/`
- `ssh://host.xz/path/to/repo.git/`
- `ssh://host.xz/~user/path/to/repo.git/`
- `ssh://host.xz/~/path/to/repo.git`
- `ssh://host.xz:port/path/to/repo.git/`
- `ssh://[user@]host.xz[:port]/path/to/repo.git/`
- `ssh://[user@]host.xz[:port]/~[user]/path/to/repo.git/`
- `rsync://host.xz/path/to/repo.git/`
- `https://host.xz/path/to/repo.git/`
- `http[s]://host.xz[:port]/path/to/repo.git/`
- `http://host.xz/path/to/repo.git/`
- `host.xz:path/to/repo.git`
- `host.xz:~user/path/to/repo.git/`
- `host.xz:/path/to/repo.git/`
- `git+ssh://host/path/to/repo.git/`
- `git://host.xz/path/to/repo.git/`
- `git://host.xz/~user/path/to/repo.git/`
- `git://host.xz[:port]/path/to/repo.git/`
- `git://host.xz[:port]/~[user]/path/to/repo.git/`
- `ftp[s]://host.xz[:port]/path/to/repo.git/`
- `[user@]host.xz:path/to/repo.git/`
- `[user@]host.xz:/~[user]/path/to/repo.git/`

Local filesystem paths:

- `file://~/path/to/repo.git/`
- `file:///path/to/repo.git/`
- `~/path/to/repo.git`
- `/path/to/repo.git/`
- `path/to/repo.git/`


## Parsing regex

1. `'(\w+://)(.+@)*([\w\d\.]+)(:[\d]+){0,1}/*(.*)'`
2. `'file://(.*)'`
3. `'(.+@)*([\w\d\.]+):(.*)'`

- `((git|ssh|http(s)?)|(git@[\w\.]+))(:(//)?)([\w\.@\:/\-~]+)(\.git)(/)?`

- `(?P<host>(git@|https://)([\w\.@]+)(/|:))(?P<owner>[\w,\-,\_]+)/(?P<repo>[\w,\-,\_]+)(.git){0,1}((/){0,1})`

- `((git@|http(s)?:\/\/)([\w\.@]+)(\/|:))([\w,\-,\_]+)\/([\w,\-,\_]+)(.git){0,1}((\/){0,1})`

## C# Parsing (memo)

```csharp
string input = "abc:1|bbbb:2|xyz:45|p:120";
string pattern = @"(?<Key>[^:]+)(?:\:)(?<Value>[^|]+)(?:\|?)";
 
Dictionary<string, string> KVPs
    = ( from Match m in Regex.Matches( input, pattern )
        select new
        {
            key = m.Groups["Key"].Value,
            value = m.Groups["Value"].Value
        }
    ).ToDictionary( p => p.key, p => p.value );

foreach ( KeyValuePair<string, string> kvp in KVPs )
Console.WriteLine( "{0,6} : {1,3}", kvp.Key, kvp.Value );
```

## Python

- [git-url-parse](https://pypi.org/project/git-url-parse/)  
  [git-url-parse docs](https://git-url-parse.readthedocs.io/en/latest/)  
  [git-url-parse repo](https://github.com/coala/git-url-parse)  
- [giturlparse.py](https://pypi.org/project/giturlparse/)  
  [giturlparse.py repo](https://github.com/FriendCode/giturlparse.py)  
  note: this one has regexes per-host
- forked [giturlparse](https://github.com/nephila/giturlparse)


git-url-parse uses the following regexes:

```python
POSSIBLE_REGEXES = (
    re.compile(r'^(?P<protocol>https?|git|ssh|rsync)\://'
               r'(?:(?P<user>.+)@)*'
               r'(?P<resource>[a-z0-9_.-]*)'
               r'[:/]*'
               r'(?P<port>[\d]+){0,1}'
               r'(?P<pathname>\/((?P<owner>[\w\-]+)\/)?'
               r'((?P<name>[\w\-\.]+?)(\.git|\/)?)?)$'),
    re.compile(r'(git\+)?'
               r'((?P<protocol>\w+)://)'
               r'((?P<user>\w+)@)?'
               r'((?P<resource>[\w\.\-]+))'
               r'(:(?P<port>\d+))?'
               r'(?P<pathname>(\/(?P<owner>\w+)/)?'
               r'(\/?(?P<name>[\w\-]+)(\.git|\/)?)?)$'),
    re.compile(r'^(?:(?P<user>.+)@)*'
               r'(?P<resource>[a-z0-9_.-]*)[:]*'
               r'(?P<port>[\d]+){0,1}'
               r'(?P<pathname>\/?(?P<owner>.+)/(?P<name>.+).git)$'),
    re.compile(r'((?P<user>\w+)@)?'
               r'((?P<resource>[\w\.\-]+))'
               r'[\:\/]{1,2}'
               r'(?P<pathname>((?P<owner>\w+)/)?'
               r'((?P<name>[\w\-]+)(\.git|\/)?)?)$'),
)
```

## JS

- JS [git-url-parse](https://github.com/IonicaBizau/git-url-parse)
