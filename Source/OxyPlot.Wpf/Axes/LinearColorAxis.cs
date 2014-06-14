﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearColorAxis.cs" company="OxyPlot">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 OxyPlot contributors
//   
//   Permission is hereby granted, free of charge, to any person obtaining a
//   copy of this software and associated documentation files (the
//   "Software"), to deal in the Software without restriction, including
//   without limitation the rights to use, copy, modify, merge, publish,
//   distribute, sublicense, and/or sell copies of the Software, and to
//   permit persons to whom the Software is furnished to do so, subject to
//   the following conditions:
//   
//   The above copyright notice and this permission notice shall be included
//   in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
//   OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//   IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//   CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//   TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//   SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   Provides a WPF wrapper for the OxyPlot.Axes.LinearColorAxis.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Wpf
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;

    /// <summary>
    /// Provides a WPF wrapper for the <see cref="OxyPlot.Axes.LinearColorAxis" />.
    /// </summary>
    [ContentProperty("GradientStops")]
    public class LinearColorAxis : Axis
    {
        /// <summary>
        /// Identifies the <see cref="GradientStopsProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GradientStopsProperty = DependencyProperty.Register(
            "GradientStops",
            typeof(GradientStopCollection),
            typeof(LinearColorAxis),
            new PropertyMetadata(default(GradientStopCollection), AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="HighColorProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HighColorProperty = DependencyProperty.Register(
            "HighColor",
            typeof(Color),
            typeof(LinearColorAxis),
            new PropertyMetadata(Colors.White, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LowColorProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LowColorProperty = DependencyProperty.Register(
            "LowColor",
            typeof(Color),
            typeof(LinearColorAxis),
            new PropertyMetadata(Colors.Black, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="PaletteSizeProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteSizeProperty = DependencyProperty.Register(
            "PaletteSize",
            typeof(int),
            typeof(LinearColorAxis),
            new PropertyMetadata(20, AppearanceChanged),
            ValidatePaletteSize);

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearColorAxis"/> class.
        /// </summary>
        public LinearColorAxis()
        {
            this.InternalAxis = new Axes.LinearColorAxis();
            this.GradientStops = new GradientStopCollection();
        }

        /// <summary>
        /// Gets or sets the palette size.
        /// </summary>
        public int PaletteSize
        {
            get
            {
                return (int)GetValue(PaletteSizeProperty);
            }

            set
            {
                this.SetValue(PaletteSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the high color.
        /// </summary>
        public Color HighColor
        {
            get
            {
                return (Color)GetValue(HighColorProperty);
            }

            set
            {
                this.SetValue(HighColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the low color.
        /// </summary>
        public Color LowColor
        {
            get
            {
                return (Color)GetValue(LowColorProperty);
            }

            set
            {
                this.SetValue(LowColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the gradient stops.
        /// </summary>
        public GradientStopCollection GradientStops
        {
            get
            {
                return (GradientStopCollection)GetValue(GradientStopsProperty);
            }

            set
            {
                this.SetValue(GradientStopsProperty, value);
            }
        }

        /// <summary>
        /// Creates the model.
        /// </summary>
        /// <returns>
        /// An axis object.
        /// </returns>
        public override Axes.Axis CreateModel()
        {
            this.SynchronizeProperties();
            return this.InternalAxis;
        }

        /// <summary>
        /// Synchronizes the properties.
        /// </summary>
        protected override void SynchronizeProperties()
        {
            base.SynchronizeProperties();
            var axis = InternalAxis as Axes.LinearColorAxis;
            Trace.Assert(axis != null);
            if (this.GradientStops != null)
            {
                axis.Palette = this.GradientStops.Count > 2
                                   ? Interpolate(this.GradientStops.ToList(), this.PaletteSize)
                                   : new OxyPalette();
            }

            axis.HighColor = this.HighColor.ToOxyColor();
            axis.LowColor = this.LowColor.ToOxyColor();
            axis.Minimum = this.Minimum;
            axis.Maximum = this.Maximum;
        }

        /// <summary>
        /// Translates a collection of <see cref="GradientStop" /> to an <see cref="OxyPalette" />.
        /// </summary>
        /// <param name="stops">
        /// The gradient stops collection to convert.
        /// </param>
        /// <param name="paletteSize">
        /// The palette size.
        /// </param>
        /// <returns>
        /// The interpolated <see cref="OxyPalette"/>.
        /// </returns>
        private static OxyPalette Interpolate(List<GradientStop> stops, int paletteSize)
        {
            Debug.Assert(stops.Count >= 2, "Can't interpolate less than 2 gradient stops.");
            Debug.Assert(paletteSize > 0, "Palette size must be non-zero positive number.");

            var palette = new List<OxyColor>();
            stops.Sort((x1, x2) => x1.Offset.CompareTo(x2.Offset));

            var palettePositions = stops[0].Offset;
            var step = (double)stops.Count / paletteSize;

            for (int i = 0; i < stops.Count - 1; i++)
            {
                var start = stops[i];
                var end = stops[i + 1];

                while (palettePositions <= end.Offset)
                {
                    palette.Add(
                        OxyColor.Interpolate(
                            start.Color.ToOxyColor(),
                            end.Color.ToOxyColor(),
                            (palettePositions - start.Offset) / (end.Offset - start.Offset)));
                    palettePositions += step;
                }
            }

            return new OxyPalette(palette);
        }

        /// <summary>
        /// Validates the palette size.
        /// </summary>
        /// <param name="value">
        /// The property value.
        /// </param>
        /// <returns>
        /// The validation result.
        /// </returns>
        private static bool ValidatePaletteSize(object value)
        {
            return (int)value >= 1;
        }
    }
}
