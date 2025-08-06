using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.DocExamples.Arguments.Description;

public class DescriptionMethodTests
{
    private readonly ITestOutputHelper _output;

    public DescriptionMethodTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // begin-snippet: description_method_test_basic
    [Fact]
    public void Deploy_Help_ShowsDynamicDescriptions()
    {
        var result = new AppRunner<DescriptionMethodExamples>()
            .RunInMem("deploy --help", _output);

        result.ExitCode.Should().Be(0);
        result.Console.Out.Should().Contain("Available targets: app, database, cache, notifications");
        result.Console.Out.Should().Contain("Available environments: dev, staging, prod");
    }
    // end-snippet

    // begin-snippet: description_method_test_dynamic
    [Fact]
    public void ProcessFiles_Help_ShowsCurrentDirectoryInfo()
    {
        var result = new AppRunner<DescriptionMethodExamples>()
            .RunInMem("process-files --help", _output);

        result.ExitCode.Should().Be(0);
        result.Console.Out.Should().Contain("Directory to process. Current:");
        result.Console.Out.Should().Contain("Supported: json, xml, yaml, csv");
        result.Console.Out.Should().Contain("Last updated:");
    }
    // end-snippet

    // begin-snippet: description_method_test_execution
    [Fact]
    public void Deploy_ExecutesNormally()
    {
        var result = new AppRunner<DescriptionMethodExamples>()
            .RunInMem("deploy --targets app,database --environment staging", _output);

        result.ExitCode.Should().Be(0);
        result.Console.Out.Should().Contain("Deploying app, database to staging");
    }
    // end-snippet

    // begin-snippet: description_method_test_real_world
    [Fact]
    public void SyncData_Help_ShowsDynamicOptions()
    {
        var result = new AppRunner<DescriptionMethodExamples>()
            .RunInMem("sync-data --help", _output);

        result.ExitCode.Should().Be(0);
        result.Console.Out.Should().Contain("Available: production, staging, backup, archive");
        result.Console.Out.Should().Contain("Available: local, s3-bucket, azure-blob, gcp-storage");
        result.Console.Out.Should().Contain("Available: full, incremental, differential");
        result.Console.Out.Should().Contain("Last sync:");
    }
    // end-snippet

    // This test demonstrates the validation error when both Description and DescriptionMethod are used
    [Fact]
    public void ProcessItems_WithBothDescriptionAndMethod_ThrowsValidationError()
    {
        var exception = Assert.Throws<InvalidConfigurationException>(() =>
        {
            new AppRunner<DescriptionMethodExamples>()
                .RunInMem("process-items --help", _output);
        });

        exception.Message.Should().Contain("Multiple description properties were set");
        exception.Message.Should().Contain("Description, DescriptionMethod");
    }
}
