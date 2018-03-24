using System;
using System.Collections.Generic;
using System.Text;
using Alexa.NET.Request;
using BoredomInc;

namespace AlexaSkill
{
    public static class MoveParser
    {
        public static Move ParseMove(this Dictionary<string, Slot> slots, string value)
        {
            if (Enum.TryParse(slots[value].Value, true, out Move move))
            {
                return move;
            }

            throw new InvalidOperationException("Unknown move type " + value);
        }
    }
}
