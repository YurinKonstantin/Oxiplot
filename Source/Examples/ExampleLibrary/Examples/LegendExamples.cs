﻿using System;
using OxyPlot;

namespace ExampleLibrary
{
    [Examples("Legends")]
    public static class LegendExamples
    {
        private static PlotModel CreateModel(int n = 20)
        {
            var model = new PlotModel("LineSeries");
            for (int i = 1; i <= n; i++)
            {
                var s = new LineSeries("Series " + i);
                model.Series.Add(s);
                for (double x = 0; x < 2 * Math.PI; x += 0.1)
                    s.Points.Add(new DataPoint(x, Math.Sin(x * i) / i + i));
            }
            return model;

        }

        [Example("Legend at right top inside")]
        public static PlotModel LegendRightTopInside()
        {
            var model = CreateModel();
            model.LegendPlacement = LegendPlacement.Inside;
            model.LegendPosition = LegendPosition.RightTop;
            return model;
        }

        [Example("Legend at right top outside")]
        public static PlotModel LegendRightTopOutside()
        {
            var model = CreateModel();
            model.LegendPlacement = LegendPlacement.Outside;
            model.LegendPosition = LegendPosition.RightTop;
            return model;
        }

        [Example("Legend at BottomLeft outside horizontal")]
        public static PlotModel LegendBottomLeftHorizontal()
        {
            var model = CreateModel(4);
            model.LegendPlacement = LegendPlacement.Outside;
            model.LegendPosition = LegendPosition.BottomLeft;
            model.LegendOrientation = LegendOrientation.Horizontal;
            return model;
        }

        [Example("Legend at TopLeft outside vertical")]
        public static PlotModel LegendTopLeftVertical()
        {
            var model = CreateModel(4);
            model.LegendPlacement = LegendPlacement.Outside;
            model.LegendPosition = LegendPosition.TopLeft;
            model.LegendOrientation = LegendOrientation.Vertical;
            return model;
        }


    }
}
