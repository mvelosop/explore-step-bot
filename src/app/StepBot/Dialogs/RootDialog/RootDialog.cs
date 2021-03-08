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
                    Intent = "HelloIntent",
                    Actions = {
                        new SendActivity("${Hello()}")
                    }
                },

                new OnIntent {
                    Intent = "HelpIntent",
                    Actions = {
                        new SendActivity("${Help()}")
                    }
                },

                new OnIntent {
                    Intent = "ProcessIntent",
                    Actions = {
                        new SendActivity("${StartProcess()}"),

                        new BeginDialog(nameof(ProcessDialog)) {
                            ResultProperty = "dialog.processResult"
                        },

                        new SendActivity("${ProcessResult()}")
                    }
                },

                new OnUnknownIntent {
                    Actions = {
                        new SendActivity("${Unknown()}")
                    }
                }
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
                        Pattern = "@(cancel|exit|bye)"
                    },
                    new IntentPattern {
                        Intent = "ProcessIntent",
                        Pattern = @"(process|begin|start)"
                    }
                }
            };
        }
    }
}