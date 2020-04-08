using System;
using System.Threading.Tasks;
using CommandDotNet.Diagnostics;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ExceptionHandlingTests
    {
        [Theory]
        [InlineData(null, nameof(ExceptionApp.Default))]
        [InlineData(nameof(ExceptionApp.ThrowException))]
        [InlineData(nameof(ExceptionApp.ThrowOneMoreException))]
        [InlineData(nameof(ExceptionApp.ThrowExceptionAsync))]
        public void CanThrowExceptions(string commandName, string exceptionMessage = null)
        {
            var args = commandName == null ? new string[0] : new[] { commandName };
            AppRunner<ExceptionApp> appRunner = new AppRunner<ExceptionApp>();
            Exception exception = Assert.Throws<Exception>(() => appRunner.Run(args));
            exception.Message.Should().Be(exceptionMessage ?? commandName);
            AssertHasCommandContext(exception);
        }

        [Fact]
        public void CanThrowExceptionsFromConstructor()
        {
            AppRunner<ExceptionConstructorApp> appRunner = new AppRunner<ExceptionConstructorApp>();
            Exception exception = Assert.Throws<Exception>(() => appRunner.Run("Process"));
            exception.Message.Should().Be("Constructor is broken");
            AssertHasCommandContext(exception);
        }

        private static void AssertHasCommandContext(Exception exception)
        {
            var ctx = exception.GetCommandContext();
            ctx.Should().NotBeNull();
        }

        public class ExceptionApp
        {
            [DefaultMethod]
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
    }
}