using System;
using System.Collections.Generic;

namespace CommandDotNet.Diagnostics;

internal record CommandLoggerConfig(
    Func<CommandContext, Action<string?>?>? WriterFactory,
    bool IncludeSystemInfo,
    bool IncludeAppConfig,
    bool IncludeMachineAndUser,
    Func<CommandContext, IEnumerable<(string key, string value)>?>? AdditionalHeadersCallback);