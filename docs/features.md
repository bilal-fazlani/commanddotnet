# Features

|Feature| |
|---|---:|
|   **Commands** |  |
| \ \ \ Define commands as methods | [docs](commands.md) ✔️ |
| \ \ \ \ \ \ Parameter Resolvers<br/>\ \ \ \ \ \ \ \ \ _Inject console and other contexts as a parameter:_<br/>\ \ \ \ \ \ \ \ \ _IConsole, CommandContext, CancellationToken & IPrompter by default._ | [docs](parameter-resolvers.md) ✔️ |
| \ \ \ Infinite nesting of subcommands | [docs](subcommands.md) ✔️ |
| \ \ \ \ \ \ Nested classes | ✔️ |  |
| \ \ \ \ \ \ Composed classes | ✔️ |  |
| \ \ \ Command interception <br/>\ \ \ \ \ \ _[interceptors](interceptors.md) for subcommands <br/>\ \ \ \ \ \ [middleware](middleware.md) for all commands_ | ✔️ |
| \ \ \ Method interception <br/>\ \ \ \ \ \ _`CommandContext.InvocationPipeline` to access method and params_ | ✔️ |
|   **Arguments** |  |
| \ \ \ Positional (Operands) | [docs](arguments.md) ✔️  |
| \ \ \ Named (Options) | [docs](arguments.md) ✔️ |
| \ \ \ \ \ \ short and long names:  _`-h` or  `--help`, **not** /help_ | ✔️ |
| \ \ \ \ \ \ flags:  _`-a` instead of `-a true`_| [docs](arguments.md#flags) ✔️ |
| \ \ \ \ \ \ bundling/clubbing: _`-a -b -c` or `-abc`_ | [docs](arguments.md#flag-clubbing) ✔️ |
| \ \ \ \ \ \ value assignments: _`-a one`, `-a=one` & `-a:one`_ | [docs](arguments.md#option-assignments) ✔️ |
| \ \ \ Define arguments as parameters in methods | [docs](arguments.md) ✔️ |
| \ \ \ Define arguments as properties in POCOs | [docs](argument-models.md) ✔️ |
|   **Data Types** |  |
| \ \ \ Primitives, Enums & Nullable< T > | [docs](argument-types.md) ✔️ |
| \ \ \ Collections | [docs](argument-collections.md) ✔️ |
| \ \ \ Dictionaries | [#251](https://github.com/bilal-fazlani/commanddotnet/issues/251) ❌ |
| \ \ \ Password: _mask value in logs and output with `*****`_ | [docs](passwords.md) ✔️ |
| \ \ \ Any type with a string constructor | [docs](argument-types.md#adding-support-for-other-types) ✔️ |
| \ \ \ Any type with a [TypeConverter](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter?view=netframework-4.8) | [docs](argument-types.md#adding-support-for-other-types) ✔️ |
| \ \ \ Custom Type Descriptors<br/>\ \ \ \ \ \ _Customize parsing and the type name shown in help and_ | [docs](argument-types.md#type-descriptors) ✔️ |
| \ \ \ Define allowed values by type<br/>\ \ \ \ \ \ _Allowed values are shown in help and will soon be used for suggestions_ | [docs](argument-types.md#type-descriptors) ✔️ |
|   **Argument Sources** |  |
| \ \ \ Response Files | [docs](response-files.md) ✔️ |
| \ \ \ Piped Input | [docs](piped-arguments.md) ✔️ |
| \ \ \ \ \ \ Streaming | [docs](piped-arguments.md) ✔️ |
| \ \ \ Prompts | [docs](prompting.md) ✔️ |
| \ \ \ \ \ \ Hide passwords | [docs](prompting.md#passwords) ✔️ |
| \ \ \ \ \ \ Multi-entry for collections | [docs](prompting.md) ✔️ |
| \ \ \ \ \ \ Auto prompt for missing arguments (optional) | [docs](prompting.md#prompting-for-missing-arguments) ✔️ |
| \ \ \ Default from EnvVar | [docs](default-values-from-config.md) ✔️ |
| \ \ \ Default from AppSetting | [docs](default-values-from-config.md) ✔️ |
|   **Help** |  |
| \ \ \ Autocomplete | [#48](https://github.com/bilal-fazlani/commanddotnet/issues/48) ❌ |
| \ \ \ Typo suggestions | [docs](typo-suggestions.md) ✔️ |
| \ \ \ Auto generated help<br/>\ \ \ \ \ \ _Aliases: -? -h --help_ | [docs](help.md) ✔️ |
| \ \ \ Custom help generators | [docs](help.md#custom-help-provider) ✔️ |  |
|   **Diagnostics** |  |
| \ \ \ App Version<br/>\ \ \ \ \ \ _`-v` or `--version` | [docs](app-version.md) ✔️ |
| \ \ \ [debug] directive<br/>\ \ \ \ \ \ _step into debugger_ | [docs](debug-directive.md)  ✔️|
| \ \ \ [parse] directive | [docs](parse-directive.md) ✔️ |
| \ \ \ \ \ \ show final values | ✔️ |
| \ \ \ \ \ \ show inputs and source<br/>\ \ \ \ \ \ \ \ \ _original source of value, including response file paths_ | ✔️ |
| \ \ \ \ \ \ show defaults and source<br/>\ \ \ \ \ \ \ \ \ _including key if from EnvVar or AppSetting_ | ✔️ |
| \ \ \ Command logging<br/>\ \ \ \ \ \ _show [parse](parse-directive.md) output and optionally system info and app config_ | [docs](command-logger.md) ✔️ |
|   **Extensibility** |  |
| \ \ \ Custom middleware | [docs](middleware.md) ✔️ |
| \ \ \ Custom directives | [docs](directives.md) ✔️ |
| \ \ \ Custom token transformations | [docs](token-transformations.md) ✔️ |
| \ \ \ Custom parameter resolvers | [docs](parameter-resolvers.md) ✔️ |
|   **Testing** | [docs](test-tools.md) |
| \ \ \ BDD Framework<br/>\ \ \ \ \ \ _Test an app as if run from the console_ | ✔️ |
| \ \ \ Supports parallel test<br/>\ \ \ \ \ \ _the whole framework avoids static state to support parallel test runs_ | ✔️ |
| \ \ \ TestConsole | ✔️ |  |
| \ \ \ TestDependencyResolver<br/>\ \ \ \ \ \ _new TestDependencyResolver{ dbSvc, httpSvc }_ | ✔️ |
| \ \ \ TempFiles helper<br/>\ \ \ \ \ \ _create and cleanup files used for tests_ | ✔️ |
| \ \ \ Capture State<br/>\ \ \ \ \ \ _Capture state within a run to help test custom middleware components_ | ✔️ |
|   **Dependency Injection** | [docs](dependency-injection.md) |
| \ \ \ MicrosoftDependencyInjection | ✔️ |
| \ \ \ Autofac | ✔️ |
| \ \ \ SimpleInjector | ✔️ |
| \ \ \ Test injector | [docs](test-tools.md) ✔️ |
| \ \ \ Custom | ✔️ |
|   **Validation** |  |
| \ \ \ [FluentValidation](https://github.com/JeremySkinner/FluentValidation) for [argument models](argument-models.md) | [docs](fluent-validation-for-argument-models.md) ✔️ |
| \ \ \ [Data Annotations](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations) | [#253](https://github.com/bilal-fazlani/commanddotnet/issues/253) ❌ |
|   **Other** |  |
| \ \ \ Ctrl+C | [docs](cancellation.md) ✔️ |
| \ \ \ Name casing<br/>\ \ \ \ \ \ _consistent name casing via [Humanizer](https://github.com/Humanizr/Humanizer)_ | [docs](name-casing.md) ✔️ |