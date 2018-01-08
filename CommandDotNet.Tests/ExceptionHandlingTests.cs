using System;
using System.Threading.Tasks;
using CommandDotNet.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class ExceptionHandlingTests : TestBase
    {
        public ExceptionHandlingTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData("ThrowException")]
        [InlineData("ThrowOneMoreException")]
        [InlineData("ThrowExceptionAsync")]
        public void CanThrowExceptions(string commandName)
        {
            AppRunner<ExceptionApp> appRunner = new AppRunner<ExceptionApp>();
            Exception exception = Assert.Throws<Exception>(() => appRunner.Run(commandName));
            exception.Message.Should().Be(commandName);
        }

        [Fact]
        public void CanThrowExceptionsFromDefaultMethod()
        {
            AppRunner<ExceptionApp> appRunner = new AppRunner<ExceptionApp>();
            Exception exception = Assert.Throws<Exception>(() => appRunner.Run());
            exception.Message.Should().Be("Default");
        }
        
        [Fact]
        public void CanThrowExceptionsFromConstructor()
        {
            AppRunner<ExceptionConstructorApp> appRunner = new AppRunner<ExceptionConstructorApp>();
            Exception exception = Assert.Throws<Exception>(() => appRunner.Run("Process"));
            exception.Message.Should().Be("Constructor is broken");
        }
    }

    public class ExceptionApp
    {
        [DefaultMethod]
        public void Default()
        {
            throw new Exception("Default");
        }
        
        public void ThrowException()
        {
            throw new Exception("ThrowException");
        }

        public int ThrowOneMoreException()
        {
            throw new Exception("ThrowOneMoreException");
        }
        
        public async Task<int> ThrowExceptionAsync()
        {
            throw new Exception("ThrowExceptionAsync");
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