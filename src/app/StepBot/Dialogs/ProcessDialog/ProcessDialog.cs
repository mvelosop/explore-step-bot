using Microsoft.Bot.Builder.Dialogs.Adaptive;
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

                new OnCancelDialog {
                    Actions = {
                        new SendActivity("${DialogCancelled()}")
                    }
                },

                new OnBeginDialog {
                    Actions = {
                        new SendActivity("${ProcessTitle()}"),

                        new EmitEvent {
                            EventName = "Process.NextStep",
                            BubbleEvent = false
                        },
                    }
                },

                new OnDialogEvent {
                    Event = "Process.NextStep",

                    Actions = {
                        new IfCondition {
                            Condition = "not(conversation.process.step1.text)",

                            Actions = {
                                new EmitEvent {
                                    EventName = "Process.GetStep1",
                                    BubbleEvent = false
                                }
                            }
                        },

                        new IfCondition {
                            Condition = "not(conversation.process.step2.text)",

                            Actions = {
                                new EmitEvent {
                                    EventName = "Process.GetStep2",
                                    BubbleEvent = false
                                }
                            }
                        },

                        new IfCondition {
                            Condition = "not(conversation.process.text)",

                            Actions = {
                                new EmitEvent {
                                    EventName = "Process.CalculateResult",
                                    BubbleEvent = false
                                }
                            }
                        },

                    }
                },

                new OnDialogEvent {
                    Event = "Process.GetStep1",

                    Actions = {
                        new BeginDialog(nameof(StepDialog), "{stepTitle: 'Step #1'}"){
                            ResultProperty = "conversation.process.step1.text"
                        },

                        new DeleteProperty {
                            Property = "conversation.process.text",
                        },

                        new EmitEvent {
                            EventName = "Process.NextStep",
                            BubbleEvent = false
                        },
                    }
                },

                new OnDialogEvent {
                    Event = "Process.GetStep2",

                    Actions = {
                        new BeginDialog(nameof(StepDialog), "{stepTitle: 'Step #2'}"){
                            ResultProperty = "conversation.process.step2.text"
                        },

                        new DeleteProperty {
                            Property = "conversation.process.text",
                        },

                        new EmitEvent {
                            EventName = "Process.NextStep",
                            BubbleEvent = false
                        },
                    }
                },

                new OnDialogEvent {
                    Event = "Process.CalculateResult",

                    Actions = {
                        new CodeAction(async (context, options) =>
                        {
                            context.State.SetValue(
                                "conversation.process.text",
                                templates.Evaluate("Result", context.State));

                            return await context.EndDialogAsync(options);
                        }),

                        new EndDialog()
                    }
                },

                //new OnIntent {
                //    Intent = "CancelIntent",
                //    Actions = {
                //        new SendActivity("${DialogCancelled()}"),

                //        new CancelAllDialogs()
                //    }
                //},

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