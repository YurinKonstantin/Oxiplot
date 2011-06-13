﻿using System;
using OxyPlot;

namespace ExampleLibrary
{
    [Examples("Polar Plots")]
    public static class PolarPlotExamples
    {
        [Example("Spiral")]
        public static PlotModel ArchimedeanSpiral()
        {
            var model = new PlotModel("Polar plot", "Archimedean spiral with equation r(θ) = θ for 0 < θ < 6π")
                            {
                                PlotType = PlotType.Polar,
                                BoxThickness = 0
                            };
            model.Axes.Add(
                new LinearAxis(AxisPosition.Angle, 0, Math.PI * 2, Math.PI / 4, Math.PI / 16)
                    {
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Solid
                    });
            model.Axes.Add(new LinearAxis(AxisPosition.Magnitude)
                               {
                                   MajorGridlineStyle = LineStyle.Solid,
                                   MinorGridlineStyle = LineStyle.Solid
                               });
            model.Series.Add(new FunctionSeries(t => t, t => t,
                                                0, Math.PI * 6, 0.01));
            return model;
        }
    }
}