using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StepBot.Dialogs
{
    public class StepDialog : AdaptiveDialog
    {
        public StepDialog()
        {
            var paths = new[] { ".", "Dialogs", "StepDialog", "StepDialog.lg" };
            var templatesFile = Path.Combine(paths);
            var templates = Templates.ParseFile(templatesFile);
            Generator = new TemplateEngineLanguageGenerator(templates);

            Recognizer = CreateRegexRecognizer();

            Triggers = new List<OnCondition> {
                new OnBeginDialog {
                    Actions = {
                        new CodeAction(async (context, options) => await context.EndDialogAsync(options)),

                        new SendActivity("${StepTitle()}"),

                        new TextInput {
                            Property = "dialog.text1",
                            Prompt = new ActivityTemplate("Enter text value #1:"),
                            AllowInterruptions = "turn.recognized.score >= 0.9"
                        },

                        new TextInput {
                            Property = "dialog.text2",
                            Prompt = new ActivityTemplate("Enter text value #2:"),
                            AllowInterruptions = "turn.recognized.score >= 0.9"
                        },

                        new CodeAction(async(context, options) =>
                        {
                            context.State.SetValue(
                                "dialog.stepText",
                                templates.Evaluate("Result", context.State, new EvaluationOptions{ LineBreakStyle = LGLineBreakStyle.Default}));

                            return await context.EndDialogAsync(options);
                        }),

                        new EndDialog("=dialog.stepText")
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
                        new CodeAction(async (context, options) => await context.EndDialogAsync(options)),

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
                        Pattern = @"(cancel|exit|bye)"
                    },
                    new IntentPattern {
                        Intent = "WhereIntent",
                        Pattern = @"(where)"
                    },
                }
            };
        }
    }
}
