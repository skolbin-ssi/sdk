﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Microsoft.TemplateEngine.Abstractions;

namespace Microsoft.TemplateEngine.Cli.Commands
{
    internal class LegacySearchCommand : BaseSearchCommand
    {
        public LegacySearchCommand(NewCommand parentCommand, Func<ParseResult, ITemplateEngineHost> hostBuilder, Func<ParseResult, ITelemetryLogger> telemetryLoggerBuilder)
            : base(parentCommand, hostBuilder, telemetryLoggerBuilder, "--search")
        {
            this.IsHidden = true;
            AddValidator(ValidateParentCommandArguments);

            parentCommand.AddNoLegacyUsageValidators(this, except: Filters.Values.Concat(new Symbol[] { ColumnsAllOption, ColumnsOption, NewCommand.ShortNameArgument }).ToArray());
        }

        public override Option<bool> ColumnsAllOption => ParentCommand.ColumnsAllOption;

        public override Option<string[]> ColumnsOption => ParentCommand.ColumnsOption;

        protected override Option GetFilterOption(FilterOptionDefinition def)
        {
            return ParentCommand.LegacyFilters[def];
        }

        protected override Task<NewCommandStatus> ExecuteAsync(SearchCommandArgs args, IEngineEnvironmentSettings environmentSettings, ITelemetryLogger telemetryLogger, InvocationContext context)
        {
            PrintDeprecationMessage<LegacySearchCommand, SearchCommand>(args.ParseResult);
            return base.ExecuteAsync(args, environmentSettings, telemetryLogger, context);
        }

        private void ValidateParentCommandArguments(CommandResult commandResult)
        {
            var nameArgumentResult = commandResult.Children.FirstOrDefault(symbol => symbol.Symbol == NameArgument);
            if (nameArgumentResult == null)
            {
                return;
            }
            ParentCommand.ValidateShortNameArgumentIsNotUsed(commandResult);
        }
    }
}
