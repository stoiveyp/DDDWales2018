using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexa.NET.Gadgets.GadgetController;
using Alexa.NET.Gadgets.GameEngine;
using Alexa.NET.Gadgets.GameEngine.Directives;
using Alexa.NET.Gadgets.GameEngine.Requests;

namespace AlexaSkill
{
    public static class InCaseOfDraw
    {
        private static readonly string[] _colors = { "FF0000", "FFFF00", "00FF00" };
        private static readonly Random _rnd = new Random();

        //Remember to add
        //RequestConverterHelper.AddGadgetRequests();

        public static SetLightDirective SetChallenge()
        {
            var findTheYellowLight = new SetLightAnimation
            {
                Sequence =
                    Enumerable.Range(0, 40).Select(r => new AnimationSegment
                    {
                        Blend = false,
                        Color = _colors[_rnd.Next(0, 2)],
                        DurationMilliseconds = 300
                    }).ToList()
            };

            return new SetLightDirective
            {
                TargetGadgets = new List<string> { "setGadget" },
                Parameters =
                {
                    TriggerEvent = ButtonAction.Down,
                    Animations = new List<SetLightAnimation>{findTheYellowLight}
                }
            };

        }

        public static StartInputHandlerDirective SetEvents()
        {
            var directive = new StartInputHandlerDirective
            {
                TimeoutMilliseconds = (int)TimeSpan.FromSeconds(10).TotalMilliseconds
            };

            directive.Recognizers.Add("hitYellow", new PatternRecognizer
            {
                Fuzzy = true,
                Patterns = new List<Pattern>
                {
                    new Pattern
                    {
                        Colors = new List<string> {"FFFF00"},
                        Action = ButtonAction.Down
                    }
                }
            });

            directive.Events.Add("win", new InputHandlerEvent
            {
                Meets = new List<string> { "hitYellow" },
                Reports = GadgetEventReportType.Matches
            });

            directive.Events.Add("lose", new InputHandlerEvent
            {
                Meets = new List<string> { "timed out" },
                Reports = GadgetEventReportType.History
            });

            return directive;
        }

        public static void HandleEventGame(InputHandlerEventRequest request)
        {
            var raisedEvent = request.Events.First();
            if (raisedEvent.Name == "lose")
            {
                var totalRaised = raisedEvent.InputEvents.Count(e => e.Color == "FFF00");
                //TODO: Store percentage as part of move for comparison
                Console.WriteLine(totalRaised);
            }
        }
    }
}
