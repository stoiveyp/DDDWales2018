﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoredomInc
{
    public class Results
    {
        public Results(IEnumerable<Result> results)
        {
            ResultInformation = results.ToList();
        }

        public List<Result> ResultInformation { get; }

        public bool? OverallWin
        {
            get
            {
                var wins = ResultInformation.Count(r => r.Win.HasValue && r.Win.Value);
                if (wins == 0)
                {
                    return null;
                }

                return wins > Math.Ceiling((double) ResultInformation.Count / 2);
            }
        }
    }
}
