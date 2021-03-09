using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.LanguageGeneration;
using System.Collections.Generic;
using System.IO;

namespace StepBot.Dialogs
{
    public class RootDialog : AdaptiveDialog
    {
        public RootDialog()
            : base(nameof(RootDialog))
        {
            var paths = new[] { ".", "Dialogs", "RootDialog", "RootDialog.lg" };
            var templatesFile = Path.Combine(paths);
            Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(templatesFile));

            Recognizer = CreateRegexRecognizer();

            Triggers = new List<OnCondition> {
                new OnConversationUpdateActivity {
                    Actions = {
                        new SendActivity("${Hello()}"),
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

                new OnIntent {
                    Intent = "CancelIntent",
                    Actions = {
                        new SendActivity("${DialogCancelled()}"),

                        new CancelAllDialogs()
                    }
                },

                new OnCancelDialog {
                    Actions = {
                        new SendActivity("${DialogCancelled()}")
                    }
                },

                new OnIntent {
                    Intent = "HelloIntent",
                    Actions = {
                        new SendActivity("${Hello()}")
                    }
                },

                new OnIntent {
                    Intent = "ProcessIntent",
                    Actions = {
                        new SendActivity("${StartProcess()}"),

                        new DeleteProperty {
                            Property = "conversation.process"
                        },

                        new BeginDialog(nameof(ProcessDialog)),

                        new SendActivity("${ProcessResult()}")
                    }
                },

                new OnIntent {
                    Intent = "UpdateStep1",
                    Actions = {
                        new SendActivity("${UpdateProcess()}"),

                        new DeleteProperty {
                            Property = "conversation.process.step1"
                        },

                        new BeginDialog(nameof(ProcessDialog)),

                        new SendActivity("${ProcessResult()}")
                    }
                },

                new OnIntent {
                    Intent = "UpdateStep2",
                    Actions = {
                        new SendActivity("${UpdateProcess()}"),

                        new DeleteProperty {
                            Property = "conversation.process.step2"
                        },

                        new BeginDialog(nameof(ProcessDialog)),

                        new SendActivity("${ProcessResult()}")
                    }
                },

                new OnUnknownIntent {
                    Actions = {
                        new SendActivity("${Unknown()}")
                    }
                },

            };

            Dialogs.Add(new ProcessDialog());
        }

        private RegexRecognizer CreateRegexRecognizer()
        {
            return new RegexRecognizer {
                Intents = new List<IntentPattern> {
                    new IntentPattern {
                        Intent = "HelloIntent",
                        Pattern = @"(hi|hello)"
                    },
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
                    new IntentPattern {
                        Intent = "ProcessIntent",
                        Pattern = @"(process|begin|start)"
                    },
                    new IntentPattern {
                        Intent = "UpdateStep1",
                        Pattern = @"update 1"
                    },
                    new IntentPattern {
                        Intent = "UpdateStep2",
                        Pattern = @"update 2"
                    },
                }
            };
        }
    }
}