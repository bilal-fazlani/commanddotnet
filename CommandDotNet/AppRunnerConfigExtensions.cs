using System;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Parsing;
using CommandDotNet.Tokens;

namespace CommandDotNet
{
    /// <summary>Extensions to enable and configure features</summary>
    public static class AppRunnerConfigExtensions
    {
        [Obsolete("replace with UseVersionMiddleware, UseFluentValidation and UsePromptForMissingOperands")]
        public static AppRunner UseBackwardsCompatibilityMode(this AppRunner appRunner)
        {
            if (appRunner.AppSettings.EnableVersionOption)
            {
                appRunner.UseVersionMiddleware();
            }

            if (appRunner.AppSettings.PromptForMissingOperands)
            {
                appRunner.UsePromptForMissingOperands();
            }

            return appRunner.UseFluentValidation();
        }

        public static AppRunner UseVersionMiddleware(this AppRunner appRunner)
        {
            return VersionMiddleware.UseVersionMiddleware(appRunner);
        }

        public static AppRunner UseDebugDirective(this AppRunner appRunner)
        {
            AssertDirectivesAreEnabled(appRunner);
            return DebugDirective.UseDebugDirective(appRunner);
        }

        public static AppRunner UseParseDirective(this AppRunner appRunner)
        {
            AssertDirectivesAreEnabled(appRunner);
            return ParseDirective.UseParseDirective(appRunner);
        }

        /// <summary>Enables piped input to be appended to any commands operand list</summary>
        public static AppRunner AppendPipedInputToOperandList(this AppRunner appRunner)
        {
            return PipedInputMiddleware.EnablePipedInput(appRunner);
        }

        public static AppRunner UseFluentValidation(this AppRunner appRunner)
        {
            // TODO: move FluentValidation into a separate repo & nuget package?
            //       there are other ways to do validation that could also
            //       be applied to parameters

            return appRunner.Configure(c =>
                c.UseMiddleware(ModelValidator.ValidateModelsMiddleware, MiddlewareStages.PostBindValuesPreInvoke));
        }

        public static AppRunner UseDependencyResolver(
            this AppRunner appRunner, 
            IDependencyResolver dependencyResolver, 
            bool useLegacyInjectDependenciesAttribute = false)
        {
            return DependencyResolverMiddleware.UseDependencyResolver(appRunner, dependencyResolver, useLegacyInjectDependenciesAttribute);
        }

        public static AppRunner UsePromptForMissingOperands(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
                c.UseMiddleware(ValuePromptMiddleware.PromptForMissingOperands, MiddlewareStages.PostParseInputPreBindValues));
        }

        public static AppRunner UseResponseFiles(this AppRunner appRunner)
        {
            return ExpandResponseFilesTransformation.UseResponseFiles(appRunner);
        }

        private static void AssertDirectivesAreEnabled(AppRunner appRunner)
        {
            if (!appRunner.AppSettings.EnableDirectives)
            {
                throw new AppRunnerException($"Directives are not enabled.  " +
                                             $"{nameof(AppRunner)}.{nameof(AppRunner.AppSettings)}.{nameof(AppSettings.EnableDirectives)} " +
                                             "must be set to true");
            }
        }
    }
}