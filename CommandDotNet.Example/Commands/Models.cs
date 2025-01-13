﻿using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;
using FluentValidation;

namespace CommandDotNet.Example.Commands;

[Command(
    Description = "example of using IArgumentModel's to define arguments, including fluent validation.",
    Usage = "'models notify [options] <email> <messages>'")]
public class Models
{
    [Command(
        Description = "Arguments can be defined and validate in a compositional manner using argument models",
        ExtendedHelpText =
            "defining common options with argument models improves consistency across an application " +
            "and can be used by middleware to implement a feature")]
    public void Notify(
        Notification notification,
        DryRunArgs dryRunArgs,
        VerbosityArgs verbosityArgs,
        IConsole console)
    {
        if (verbosityArgs.Verbose)
        {
            console.Out.WriteLine($"preparing notification for {notification.Email}");
        }

        if (dryRunArgs.IsDryRun || verbosityArgs.Quite)
        {
            return;
        }

        console.Out.WriteLine($"sending notification to {notification.Email}");
        if (verbosityArgs.Verbose)
        {
            console.Out.WriteLine(notification.Email!.ToStringFromPublicProperties());
        }
    }

    public class DryRunArgs : IArgumentModel
    {
        [Option(
            "dryrun",
            Description = "middleware could abort a UnitOfWork or DbTransaction")]
        public bool IsDryRun { get; set; } = false;
    }

    public class VerbosityArgs : IArgumentModel
    {
        [Option('v', "verbose",
            Description = "middleware could enable console logging at trace level")]
        public bool Verbose { get; set; }

        [Option('q', "quiet", Description = "middleware could disable console logging")]
        public bool Quite { get; set; }
    }

    public class Notification : IArgumentModel
    {
        [Operand]
        public string? Email { get; set; }

        [Operand]
        public List<string>? Messages { get; set; }

        [Operand]
        public NotificationOptions? NotificationOptions { get; set; }
    }

    public class NotificationOptions : IArgumentModel
    {
        [Option]
        public DateTime? SendAfter { get; set; }

        [Option]
        public int? RetryCount { get; set; }
    }

    private class NotificationValidator : AbstractValidator<Notification>
    {
        public NotificationValidator()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
            RuleFor(x => x.Messages).NotNull().NotEmpty();
        }
    }
}