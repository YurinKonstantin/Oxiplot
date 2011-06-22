﻿using System;
using System.Collections.Generic;

namespace OxyPlot
{
    public class LogarithmicAxis : AxisBase
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "LogarithmicAxis" /> class.
        /// </summary>
        public LogarithmicAxis()
        {
            FilterMinValue = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogarithmicAxis"/> class.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="title">The title.</param>
        public LogarithmicAxis(AxisPosition pos, string title)
            : this()
        {
            Position = pos;
            Title = title;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogarithmicAxis"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="title">The title.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        public LogarithmicAxis(AxisPosition position, double minimum = double.NaN, double maximum = double.NaN, string title = null)
            : this()
        {
            Position = position;
            Title = title;
            Minimum = minimum;
            Maximum = maximum;
        }

        public override void GetTickValues(out ICollection<double> majorValues, out ICollection<double> minorValues)
        {
            if (ActualMinimum <= 0)
            {
                ActualMinimum = 0.1;
            }

            var e0 = (int)Math.Floor(Math.Log10(ActualMinimum));
            var e1 = (int)Math.Ceiling(Math.Log10(ActualMaximum));
            double d0 = Math.Pow(10, e0);
            double d1 = Math.Pow(10, e1);
            double d = d0;
            majorValues = new List<double>();
            minorValues = new List<double>();

            //if (ActualMaximum / ActualMinimum < 10)
            //{
            //    while (d<=d1+double.Epsilon)
            //    {
            //        if (d >= ActualMinimum && d <= ActualMaximum)
            //        {
            //            majorValues.Add(d);
            //        }
            //        double e2 = (int)Math.Floor(Math.Log10(d));
            //        double dd = Math.Pow(10, e2);
            //        d += dd * 0.1;
            //    }
            //    return;
            //}

            while (d <= d1 + double.Epsilon)
            {
                if (d >= ActualMinimum && d <= ActualMaximum)
                {
                    majorValues.Add(d);
                }

                for (int i = 1; i <= 9; i++)
                {
                    double d2 = d * (i + 1);
                    if (d2 > d1 + double.Epsilon)
                    {
                        break;
                    }

                    if (d2 > ActualMaximum)
                    {
                        break;
                    }

                    if (d2 > ActualMinimum && d2 < ActualMaximum)
                    {
                        minorValues.Add(d2);
                    }
                }

                d *= 10;
            }
        }

        protected override double PreTransform(double x)
        {
            if (x < 0)
            {
                return -1;
            }

            return Math.Log(x);
        }

        protected override double PostInverseTransform(double x)
        {
            return Math.Exp(x);
        }

        public override void Pan(double x0, double x1)
        {
            if (!IsPanEnabled)
                return;
            if (x1 == 0)
                return;
            double dx = x0 / x1;
            Minimum = ActualMinimum * dx;
            Maximum = ActualMaximum * dx;
        }

        public override void ZoomAt(double factor, double x)
        {
            if (!IsZoomEnabled)
                return;
            double px = PreTransform(x);
            double dx0 = PreTransform(ActualMinimum) - px;
            double dx1 = PreTransform(ActualMaximum) - px;
            Minimum = PostInverseTransform(dx0 / factor + px);
            Maximum = PostInverseTransform(dx1 / factor + px);
        }
    }
}