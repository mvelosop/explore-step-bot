﻿using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;
using System.Collections.Generic;
using System.IO;

namespace StepBot.Dialogs
{
    public class ProcessDialog : AdaptiveDialog
    {
        public ProcessDialog()
        {
            var paths = new[] { ".", "Dialogs", "ProcessDialog", "ProcessDialog.lg" };
            var templatesFile = Path.Combine(paths);
            var templates = Templates.ParseFile(templatesFile);
            Generator = new TemplateEngineLanguageGenerator(templates);

            Recognizer = CreateRegexRecognizer();

            Triggers = new List<OnCondition> {
                new OnBeginDialog {
                    Actions = {
                        new SendActivity("${ProcessTitle()}"),

                        new BeginDialog(nameof(StepDialog), "{stepTitle: 'Step #1'}"){ 
                            ResultProperty = "dialog.step1Text"
                        },

                        new BeginDialog(nameof(StepDialog), "{stepTitle: 'Step #2'}"){ 
                            ResultProperty = "dialog.step2Text"
                        },

                        new CodeAction(async (context, options) =>
                        {
                            context.State.SetValue(
                                "dialog.processText",
                                templates.Evaluate("Result", context.State, new EvaluationOptions{ LineBreakStyle = LGLineBreakStyle.Default }));

                            return await context.EndDialogAsync(options);
                        }),

                        new EndDialog("=dialog.processText")
                    }
                },

                new OnIntent {
                    Intent = "HelpIntent",
                    Actions = {
                        new SendActivity("${Help()}")
                    }
                },

                new OnIntent {
                    Intent = "WhereIntent",
                    Actions = {
                        new SendActivity("${Where()}")
                    }
                },

                //new OnIntent {
                //    Intent = "CancelIntent",
                //    Actions = {
                //        new SendActivity("${DialogCancelled()}"),

                //        new CancelAllDialogs()
                //    }
                //},

                new OnCancelDialog {
                    Actions = {
                        new SendActivity("${DialogCancelled()}")
                    }
                },

            };

            Dialogs.Add(new StepDialog());
        }

        private RegexRecognizer CreateRegexRecognizer()
        {
            return new RegexRecognizer {
                Intents = new List<IntentPattern> {
                    new IntentPattern {
                        Intent = "HelpIntent",
                        Pattern = @"(help|help me)"
                    },
                    new IntentPattern {
                        Intent = "CancelIntent",
                        Pattern = "@(cancel|exit|bye)"
                    },
                    new IntentPattern {
                        Intent = "WhereIntent",
                        Pattern = "@(where)"
                    },
                }
            };
        }
    }
}