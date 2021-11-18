using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandDotNet.Diagnostics;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ExceptionHandlingTests
    {
        public ExceptionHandlingTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Theory]
        [InlineData(null, nameof(ExceptionApp.Default))]
        [InlineData(nameof(ExceptionApp.ThrowException))]
        [InlineData(nameof(ExceptionApp.ThrowOneMoreException))]
        [InlineData(nameof(ExceptionApp.ThrowExceptionAsync))]
        public void CanThrowExceptions(string? commandName, string? exceptionMessage = null)
        {
            var args = commandName == null ? Array.Empty<string>() : new[] { commandName };
            var exception = Assert.Throws<Exception>(() => 
                new AppRunner<ExceptionApp>().Run(args));
            AssertException(exception, exceptionMessage ?? commandName!, "ExceptionApp", true);
        }

        [Fact]
        public void CanThrowExceptionsFromConstructor()
        {
            var exception = Assert.Throws<Exception>(() => 
                new AppRunner<ExceptionConstructorApp>().Run("Process"));
            AssertException(exception, "Constructor is broken", "ExceptionConstructorApp", true);
        }

        [Theory]
        [InlineData(null, nameof(ExceptionApp.Default))]
        [InlineData(nameof(ExceptionApp.ThrowException))]
        [InlineData(nameof(ExceptionApp.ThrowOneMoreException))]
        [InlineData(nameof(ExceptionApp.ThrowExceptionAsync))]
        public void CanHandleErrors(string? commandName, string? exceptionMessage = null)
        {
            var args = commandName == null ? Array.Empty<string>() : new[] { commandName };
            CommandContext? context = null;
            Exception? exception = null;
            var exitCode = new AppRunner<ExceptionApp>()
                .UseErrorHandler((ctx, ex) =>
                {
                    context = ctx;
                    exception = ex;
                    return ExitCodes.Error.Result;
                })
                .Run(args);
            exitCode.Should().Be(1);
            AssertException(exception!, exceptionMessage ?? commandName!, "ExceptionApp", false);
            context.Should().NotBeNull();
        }

        [Fact]
        public void CanHandleErrorsFromConstructor()
        {
            CommandContext? context = null;
            Exception? exception = null;
            var exitCode = new AppRunner<ExceptionConstructorApp>()
                .UseErrorHandler((ctx, ex) =>
                {
                    context = ctx;
                    exception = ex;
                    return ExitCodes.Error.Result;
                })
                .Run("Process");

            exitCode.Should().Be(1);
            AssertException(exception!, "Constructor is broken", "ExceptionConstructorApp", false);
            context.Should().NotBeNull();
        }

        [Fact]
        public void ErrorHandlerIsNotTriggeredForValueParsingException()
        {
            Exception? exception = null;
            var exitCode = new AppRunner<ExceptionApp>()
                .UseErrorHandler((ctx, ex) =>
                {
                    exception = ex;
                    return ExitCodes.Error.Result;
                })
                .Run("Process -o");

            exitCode.Should().Be(1);
            exception.Should().BeNull();
        }

        [Fact]
        public void ErrorHandlerIsNotTriggeredForInvalidConfigurationException()
        {
            Exception? exception = null;
            var exitCode = new AppRunner<InvalidConfigurationApp>()
                .UseErrorHandler((ctx, ex) =>
                {
                    exception = ex;
                    return ExitCodes.Error.Result;
                })
                .Run("Do");

            exitCode.Should().Be(1);
            exception.Should().BeNull();
        }

        private void AssertException(Exception exception, string exceptionMessage, string appName, bool errorHasContext)
        {
            exception.Should().NotBeNull();
            exception.Message.Should().Be(exceptionMessage);
            exception.StackTrace.Should()
                .StartWith($"   at CommandDotNet.Tests.FeatureTests.ExceptionHandlingTests.{appName}.");

            if (errorHasContext)
            {
                var ctx = exception.GetCommandContext();
                ctx.Should().NotBeNull();
            }
            //_output.WriteLine(exception.StackTrace);
        }

        public class ExceptionApp
        {
            [DefaultCommand]
            public void Default()
            {
                throw new Exception(nameof(Default));
            }

            public void ThrowException()
            {
                throw new Exception(nameof(ThrowException));
            }

            public int ThrowOneMoreException()
            {
                throw new Exception(nameof(ThrowOneMoreException));
            }

#pragma warning disable 1998
            public async Task<int> ThrowExceptionAsync()
#pragma warning restore 1998
            {
                throw new Exception(nameof(ThrowExceptionAsync));
            }
        }

        public class ExceptionConstructorApp
        {
            public ExceptionConstructorApp()
            {
                throw new Exception("Constructor is broken");
            }

            public void Process()
            {

            }
        }

        public class InvalidConfigurationApp
        {
            public void Do(List<string> opList1, List<string> opList2){}
        }
    }
}