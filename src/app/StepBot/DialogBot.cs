// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StepBot
{
    public class DialogBot<T> : ActivityHandler
        where T : Dialog
    {
        private readonly DialogManager DialogManager;
        protected readonly ILogger Logger;

        public DialogBot(T rootDialog, ILogger<DialogBot<T>> logger)
        {
            Logger = logger;

            DialogManager = new DialogManager(rootDialog);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Running dialog with Activity.");
            await DialogManager.OnTurnAsync(turnContext, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

    }
}
