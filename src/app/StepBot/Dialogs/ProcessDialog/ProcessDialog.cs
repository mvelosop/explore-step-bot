using Microsoft.Bot.Builder.Dialogs;
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
using System.Threading;
using System.Threading.Tasks;

namespace StepBot.Dialogs
{
    public class ProcessDialog : AdaptiveDialog
    {
        public ProcessDialog()
        {
            var paths = new[] { ".", "Dialogs", "ProcessDialog", "ProcessDialog.lg" };
            var templatesFile = Path.Combine(paths);
            Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(templatesFile));

            //Recognizer = CreateRegexRecognizer();

            Triggers = new List<OnCondition> {
                new OnBeginDialog {
                    Actions = {
                        new SendActivity("${ProcessTitle()}"),

                        new TextInput {
                            Property = "dialog.processText",
                            Prompt = new ActivityTemplate("Enter text value:"),
                            //Value = "=turn.activity.text"
                        },

                        new EndDialog("=dialog.processText")
                    }
                }
            };
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