﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Axis.cs" company="OxyPlot">
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
//   Provides an abstract base class for axes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Axes
{
    using System;
    using System.Collections.Generic;

    using OxyPlot.Series;

    /// <summary>
    /// Provides an abstract base class for axes.
    /// </summary>
    public abstract class Axis : PlotElement
    {
        /// <summary>
        /// Exponent function.
        /// </summary>
        protected static readonly Func<double, double> Exponent = x => Math.Floor(Math.Log(Math.Abs(x), 10));

        /// <summary>
        /// Mantissa function.
        /// </summary>
        protected static readonly Func<double, double> Mantissa = x => x / Math.Pow(10, Exponent(x));

        /// <summary>
        /// The offset.
        /// </summary>
        private double offset;

        /// <summary>
        /// The scale.
        /// </summary>
        private double scale;

        /// <summary>
        /// The position of the axis.
        /// </summary>
        private AxisPosition position;

        /// <summary>
        /// Initializes a new instance of the <see cref="Axis" /> class.
        /// </summary>
        protected Axis()
        {
            this.Position = AxisPosition.Left;
            this.PositionTier = 0;
            this.IsAxisVisible = true;
            this.Layer = AxisLayer.BelowSeries;

            this.ViewMaximum = double.NaN;
            this.ViewMinimum = double.NaN;

            this.AbsoluteMaximum = double.MaxValue;
            this.AbsoluteMinimum = double.MinValue;

            this.Minimum = double.NaN;
            this.Maximum = double.NaN;
            this.MinorStep = double.NaN;
            this.MajorStep = double.NaN;

            this.MinimumPadding = 0.01;
            this.MaximumPadding = 0.01;
            this.MinimumRange = 0;

            this.TickStyle = TickStyle.Outside;
            this.TicklineColor = OxyColors.Black;

            this.AxislineStyle = LineStyle.None;
            this.AxislineColor = OxyColors.Black;
            this.AxislineThickness = 1.0;

            this.MajorGridlineStyle = LineStyle.None;
            this.MajorGridlineColor = OxyColor.FromArgb(0x40, 0, 0, 0);
            this.MajorGridlineThickness = 1;

            this.MinorGridlineStyle = LineStyle.None;
            this.MinorGridlineColor = OxyColor.FromArgb(0x20, 0, 0, 0x00);
            this.MinorGridlineThickness = 1;

            this.ExtraGridlineStyle = LineStyle.Solid;
            this.ExtraGridlineColor = OxyColors.Black;
            this.ExtraGridlineThickness = 1;

            this.ShowMinorTicks = true;

            this.MinorTickSize = 4;
            this.MajorTickSize = 7;

            this.StartPosition = 0;
            this.EndPosition = 1;

            this.TitlePosition = 0.5;
            this.TitleFormatString = "{0} [{1}]";
            this.TitleClippingLength = 0.9;
            this.TitleColor = OxyColors.Automatic;
            this.TitleFontSize = double.NaN;
            this.TitleFontWeight = FontWeights.Normal;
            this.ClipTitle = true;

            this.Angle = 0;

            this.IsZoomEnabled = true;
            this.IsPanEnabled = true;

            this.FilterMinValue = double.MinValue;
            this.FilterMaxValue = double.MaxValue;
            this.FilterFunction = null;

            this.IntervalLength = 60;

            this.AxisDistance = 0;
            this.AxisTitleDistance = 4;
            this.AxisTickToLabelDistance = 4;
        }

        /// <summary>
        /// Occurs when the axis has been changed (by zooming, panning or resetting).
        /// </summary>
        public event EventHandler<AxisChangedEventArgs> AxisChanged;

        /// <summary>
        /// Occurs when the transform changed (size or axis range was changed).
        /// </summary>
        public event EventHandler TransformChanged;

        /// <summary>
        /// Gets or sets the absolute maximum. This is only used for the UI control. It will not be possible to zoom/pan beyond this limit.
        /// </summary>
        /// <value>The absolute maximum.</value>
        public double AbsoluteMaximum { get; set; }

        /// <summary>
        /// Gets or sets the absolute minimum. This is only used for the UI control. It will not be possible to zoom/pan beyond this limit.
        /// </summary>
        /// <value>The absolute minimum.</value>
        public double AbsoluteMinimum { get; set; }

        /// <summary>
        /// Gets or sets the actual major step.
        /// </summary>
        public double ActualMajorStep { get; protected set; }

        /// <summary>
        /// Gets or sets the actual maximum value of the axis.
        /// </summary>
        /// <remarks>If ViewMaximum is not NaN, this value will be defined by ViewMaximum.
        /// Otherwise, if Maximum is not NaN, this value will be defined by Maximum.
        /// Otherwise, this value will be defined by the maximum (+padding) of the data.</remarks>
        public double ActualMaximum { get; protected set; }

        /// <summary>
        /// Gets or sets the actual minimum value of the axis.
        /// </summary>
        /// <remarks>If ViewMinimum is not NaN, this value will be defined by ViewMinimum.
        /// Otherwise, if Minimum is not NaN, this value will be defined by Minimum.
        /// Otherwise this value will be defined by the minimum (+padding) of the data.</remarks>
        public double ActualMinimum { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum value of the data displayed on this axis.
        /// </summary>
        /// <value>The data maximum.</value>
        public double DataMaximum { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum value of the data displayed on this axis.
        /// </summary>
        /// <value>The data minimum.</value>
        public double DataMinimum { get; protected set; }

        /// <summary>
        /// Gets or sets the actual minor step.
        /// </summary>
        public double ActualMinorStep { get; protected set; }

        /// <summary>
        /// Gets or sets the actual string format being used.
        /// </summary>
        public string ActualStringFormat { get; protected set; }

        /// <summary>
        /// Gets the actual title (including Unit if Unit is set).
        /// </summary>
        /// <value>The actual title.</value>
        public string ActualTitle
        {
            get
            {
                if (this.Unit != null)
                {
                    return string.Format(this.TitleFormatString, this.Title, this.Unit);
                }

                return this.Title;
            }
        }

        /// <summary>
        /// Gets or sets the angle for the axis values.
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Gets or sets the distance from axis tick to number label.
        /// </summary>
        /// <value>The axis tick to label distance.</value>
        public double AxisTickToLabelDistance { get; set; }

        /// <summary>
        /// Gets or sets the distance from axis number to axis title.
        /// </summary>
        /// <value>The axis title distance.</value>
        public double AxisTitleDistance { get; set; }

        /// <summary>
        /// Gets or sets the distance between the plot area and the axis
        /// </summary>
        public double AxisDistance { get; set; }

        /// <summary>
        /// Gets or sets the color of the axis line.
        /// </summary>
        public OxyColor AxislineColor { get; set; }

        /// <summary>
        /// Gets or sets the axis line.
        /// </summary>
        public LineStyle AxislineStyle { get; set; }

        /// <summary>
        /// Gets or sets the axis line.
        /// </summary>
        public double AxislineThickness { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to clip the axis title.
        /// </summary>
        /// <remarks>The default value is <c>true</c>.</remarks>
        public bool ClipTitle { get; set; }

        /// <summary>
        /// Gets or sets the end position of the axis on the plot area. This is a fraction from 0(bottom/left) to 1(top/right).
        /// </summary>
        public double EndPosition { get; set; }

        /// <summary>
        /// Gets or sets the color of the extra gridlines.
        /// </summary>
        public OxyColor ExtraGridlineColor { get; set; }

        /// <summary>
        /// Gets or sets the extra gridlines line style.
        /// </summary>
        public LineStyle ExtraGridlineStyle { get; set; }

        /// <summary>
        /// Gets or sets the extra gridline thickness.
        /// </summary>
        public double ExtraGridlineThickness { get; set; }

        /// <summary>
        /// Gets or sets the values for extra gridlines.
        /// </summary>
        public double[] ExtraGridlines { get; set; }

        /// <summary>
        /// Gets or sets the filter function.
        /// </summary>
        /// <value>The filter function.</value>
        public Func<double, bool> FilterFunction { get; set; }

        /// <summary>
        /// Gets or sets the maximum value that can be shown using this axis. Values greater or equal to this value will not be shown.
        /// </summary>
        /// <value>The filter max value.</value>
        public double FilterMaxValue { get; set; }

        /// <summary>
        /// Gets or sets the minimum value that can be shown using this axis. Values smaller or equal to this value will not be shown.
        /// </summary>
        /// <value>The filter min value.</value>
        public double FilterMinValue { get; set; }

        /// <summary>
        /// Gets or sets the length of the interval (screen length). The available length of the axis will be divided by this length to get the approximate number of major intervals on the axis. The default value is 60.
        /// </summary>
        public double IntervalLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this axis is visible.
        /// </summary>
        public bool IsAxisVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pan is enabled.
        /// </summary>
        public bool IsPanEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether this axis is reversed. It is reversed if StartPosition>EndPosition.
        /// </summary>
        public bool IsReversed
        {
            get
            {
                return this.StartPosition > this.EndPosition;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether zoom is enabled.
        /// </summary>
        public bool IsZoomEnabled { get; set; }

        /// <summary>
        /// Gets or sets the key of the axis. This can be used to find an axis if you have defined multiple axes in a plot.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        /// <value>The layer.</value>
        public AxisLayer Layer { get; set; }

        /// <summary>
        /// Gets or sets the color of the major gridline.
        /// </summary>
        public OxyColor MajorGridlineColor { get; set; }

        /// <summary>
        /// Gets or sets the major gridline style.
        /// </summary>
        public LineStyle MajorGridlineStyle { get; set; }

        /// <summary>
        /// Gets or sets the major gridline thickness.
        /// </summary>
        public double MajorGridlineThickness { get; set; }

        /// <summary>
        /// Gets or sets the major step. (the interval between large ticks with numbers).
        /// </summary>
        public double MajorStep { get; set; }

        /// <summary>
        /// Gets or sets the size of the major tick.
        /// </summary>
        public double MajorTickSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of the axis.
        /// </summary>
        public double Maximum { get; set; }

        /// <summary>
        /// Gets or sets the 'padding' fraction of the maximum value. A value of 0.01 gives 1% more space on the maximum end of the axis. This property is not used if the Maximum property is set.
        /// </summary>
        public double MaximumPadding { get; set; }

        /// <summary>
        /// Gets or sets the minimum value of the axis.
        /// </summary>
        public double Minimum { get; set; }

        /// <summary>
        /// Gets or sets the 'padding' fraction of the minimum value. A value of 0.01 gives 1% more space on the minimum end of the axis. This property is not used if the Minimum property is set.
        /// </summary>
        public double MinimumPadding { get; set; }

        /// <summary>
        /// Gets or sets the minimum range of the axis. Setting this property ensures that ActualMaximum-ActualMinimum > MinimumRange.
        /// </summary>
        public double MinimumRange { get; set; }

        /// <summary>
        /// Gets or sets the color of the minor gridline.
        /// </summary>
        public OxyColor MinorGridlineColor { get; set; }

        /// <summary>
        /// Gets or sets the minor gridline style.
        /// </summary>
        public LineStyle MinorGridlineStyle { get; set; }

        /// <summary>
        /// Gets or sets the minor gridline thickness.
        /// </summary>
        public double MinorGridlineThickness { get; set; }

        /// <summary>
        /// Gets or sets the minor step (the interval between small ticks without number).
        /// </summary>
        public double MinorStep { get; set; }

        /// <summary>
        /// Gets or sets the size of the minor tick.
        /// </summary>
        public double MinorTickSize { get; set; }

        /// <summary>
        /// Gets or sets the offset. This is used to transform between data and screen coordinates.
        /// </summary>
        public double Offset
        {
            get
            {
                return this.offset;
            }

            protected set
            {
                this.offset = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the axis.
        /// </summary>
        public AxisPosition Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the axis should be positioned on the zero-crossing of the related axis.
        /// </summary>
        public bool PositionAtZeroCrossing { get; set; }

        /// <summary>
        /// Gets or sets the position tier which defines in which tier the axis is displayed.
        /// </summary>
        /// <remarks>The bigger the value the the further afar is the axis from the graph.</remarks>
        public int PositionTier { get; set; }

        /// <summary>
        /// Gets or sets the related axis. This is used for polar coordinate systems where the angle and magnitude axes are related.
        /// </summary>
        public Axis RelatedAxis { get; set; }

        /// <summary>
        /// Gets or sets the scaling factor of the axis. This is used to transform between data and screen coordinates.
        /// </summary>
        public double Scale
        {
            get
            {
                return this.scale;
            }

            protected set
            {
                this.scale = value;
            }
        }

        /// <summary>
        /// Gets or sets the screen coordinate of the Maximum point on the axis.
        /// </summary>
        public ScreenPoint ScreenMax { get; protected set; }

        /// <summary>
        /// Gets or sets the screen coordinate of the Minimum point on the axis.
        /// </summary>
        public ScreenPoint ScreenMin { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether minor ticks should be shown.
        /// </summary>
        public bool ShowMinorTicks { get; set; }

        /// <summary>
        /// Gets or sets the start position of the axis on the plot area. This is a fraction from 0(bottom/left) to 1(top/right).
        /// </summary>
        public double StartPosition { get; set; }

        /// <summary>
        /// Gets or sets the string format used for formatting the axis values.
        /// </summary>
        public string StringFormat { get; set; }

        /// <summary>
        /// Gets or sets the tick style (both for major and minor ticks).
        /// </summary>
        public TickStyle TickStyle { get; set; }

        /// <summary>
        /// Gets or sets the color of the ticks (both major and minor ticks).
        /// </summary>
        public OxyColor TicklineColor { get; set; }

        /// <summary>
        /// Gets or sets the title of the axis.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the length of the title clipping rectangle (fraction of the available length of the axis).
        /// </summary>
        /// <remarks>The default value is 0.9</remarks>
        public double TitleClippingLength { get; set; }

        /// <summary>
        /// Gets or sets the color of the title.
        /// </summary>
        /// <value>The color of the title.</value>
        /// <remarks>If TitleColor is <c>null</c>, the parent PlotModel's TextColor will be used.</remarks>
        public OxyColor TitleColor { get; set; }

        /// <summary>
        /// Gets or sets the title font.
        /// </summary>
        /// <value>The title font.</value>
        public string TitleFont { get; set; }

        /// <summary>
        /// Gets or sets the size of the title font.
        /// </summary>
        /// <value>The size of the title font.</value>
        public double TitleFontSize { get; set; }

        /// <summary>
        /// Gets or sets the title font weight.
        /// </summary>
        /// <value>The title font weight.</value>
        public double TitleFontWeight { get; set; }

        /// <summary>
        /// Gets or sets the format string used for formatting the title and unit when unit is defined. If unit is <c>null</c>, only Title is used. The default value is "{0} [{1}]", where {0} uses the Title and {1} uses the Unit.
        /// </summary>
        public string TitleFormatString { get; set; }

        /// <summary>
        /// Gets or sets the position of the title (0.5 is in the middle).
        /// </summary>
        public double TitlePosition { get; set; }

        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
        public string ToolTip { get; set; }

        /// <summary>
        /// Gets or sets the unit of the axis.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use superscript exponential format. This format will convert 1.5E+03 to 1.5·10^{3} and render the superscript properly If StringFormat is <c>null</c>, 1.0E+03 will be converted to 10^{3}
        /// </summary>
        public bool UseSuperExponentialFormat { get; set; }

        /// <summary>
        /// Gets or sets the position tier max shift.
        /// </summary>
        /// <value>The position tier max shift.</value>
        internal double PositionTierMaxShift { get; set; }

        /// <summary>
        /// Gets or sets the position tier min shift.
        /// </summary>
        /// <value>The position tier min shift.</value>
        internal double PositionTierMinShift { get; set; }

        /// <summary>
        /// Gets or sets the size of the position tier.
        /// </summary>
        /// <value>The size of the position tier.</value>
        internal double PositionTierSize { get; set; }

        /// <summary>
        /// Gets the actual color of the title.
        /// </summary>
        /// <value>The actual color of the title.</value>
        protected internal OxyColor ActualTitleColor
        {
            get
            {
                return this.TitleColor.GetActualColor(this.PlotModel.TextColor);
            }
        }

        /// <summary>
        /// Gets the actual title font.
        /// </summary>
        protected internal string ActualTitleFont
        {
            get
            {
                return this.TitleFont ?? this.PlotModel.DefaultFont;
            }
        }

        /// <summary>
        /// Gets the actual size of the title font.
        /// </summary>
        /// <value>The actual size of the title font.</value>
        protected internal double ActualTitleFontSize
        {
            get
            {
                return !double.IsNaN(this.TitleFontSize) ? this.TitleFontSize : this.ActualFontSize;
            }
        }

        /// <summary>
        /// Gets the actual title font weight.
        /// </summary>
        protected internal double ActualTitleFontWeight
        {
            get
            {
                return !double.IsNaN(this.TitleFontWeight) ? this.TitleFontWeight : this.ActualFontWeight;
            }
        }

        /// <summary>
        /// Gets or sets the current view's maximum. This value is used when the user zooms or pans.
        /// </summary>
        /// <value>The view maximum.</value>
        protected double ViewMaximum { get; set; }

        /// <summary>
        /// Gets or sets the current view's minimum. This value is used when the user zooms or pans.
        /// </summary>
        /// <value>The view minimum.</value>
        protected double ViewMinimum { get; set; }

        /// <summary>
        /// Transform the specified screen point to data coordinates.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <param name="xaxis">The x axis.</param>
        /// <param name="yaxis">The y axis.</param>
        /// <returns>The data point.</returns>
        public static DataPoint InverseTransform(ScreenPoint p, Axis xaxis, Axis yaxis)
        {
            return xaxis.InverseTransform(p.x, p.y, yaxis);
        }

        /// <summary>
        /// Coerces the actual maximum and minimum values.
        /// </summary>
        public virtual void CoerceActualMaxMin()
        {
            // Coerce actual minimum
            if (double.IsNaN(this.ActualMinimum) || double.IsInfinity(this.ActualMinimum))
            {
                this.ActualMinimum = 0;
            }

            // Coerce actual maximum
            if (double.IsNaN(this.ActualMaximum) || double.IsInfinity(this.ActualMaximum))
            {
                this.ActualMaximum = 100;
            }

            if (this.ActualMaximum <= this.ActualMinimum)
            {
                this.ActualMaximum = this.ActualMinimum + 100;
            }

            // Coerce the minimum range
            var range = this.ActualMaximum - this.ActualMinimum;
            if (range < this.MinimumRange)
            {
                var average = (this.ActualMaximum + this.ActualMinimum) * 0.5;
                var delta = this.MinimumRange / 2;
                this.ActualMinimum = average - delta;
                this.ActualMaximum = average + delta;
            }

            if (this.AbsoluteMaximum <= this.AbsoluteMinimum)
            {
                throw new InvalidOperationException("AbsoluteMaximum should be larger than AbsoluteMinimum.");
            }
        }

        /// <summary>
        /// Formats the value to be used on the axis.
        /// </summary>
        /// <param name="x">The value.</param>
        /// <returns>The formatted value.</returns>
        public virtual string FormatValue(double x)
        {
            // The "SuperExponentialFormat" renders the number with superscript exponents. E.g. 10^2
            if (this.UseSuperExponentialFormat && !x.Equals(0))
            {
                double exp = Exponent(x);
                double mantissa = Mantissa(x);
                string fmt;
                if (this.StringFormat == null)
                {
                    fmt = Math.Abs(mantissa - 1.0) < 1e-6 ? "10^{{{1:0}}}" : "{0}·10^{{{1:0}}}";
                }
                else
                {
                    fmt = "{0:" + this.StringFormat + "}·10^{{{1:0}}}";
                }

                return string.Format(this.ActualCulture, fmt, mantissa, exp);
            }

            string format = string.Concat("{0:", this.ActualStringFormat ?? this.StringFormat ?? string.Empty, "}");
            return string.Format(this.ActualCulture, format, x);
        }

        /// <summary>
        /// Gets the coordinates used to draw ticks and tick labels (numbers or category names).
        /// </summary>
        /// <param name="majorLabelValues">The major label values.</param>
        /// <param name="majorTickValues">The major tick values.</param>
        /// <param name="minorTickValues">The minor tick values.</param>
        public virtual void GetTickValues(
            out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
        {
            minorTickValues = AxisUtilities.CreateTickValues(this.ActualMinimum, this.ActualMaximum, this.ActualMinorStep);
            majorTickValues = AxisUtilities.CreateTickValues(this.ActualMinimum, this.ActualMaximum, this.ActualMajorStep);
            majorLabelValues = majorTickValues;
        }

        /// <summary>
        /// Gets the value from an axis coordinate, converts from a coordinate <see cref="double" /> value to the actual data type.
        /// </summary>
        /// <param name="x">The coordinate.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>Examples: The <see cref="DateTimeAxis" /> returns the <see cref="DateTime" /> and <see cref="CategoryAxis" /> returns category strings.</remarks>
        public virtual object GetValue(double x)
        {
            return x;
        }

        /// <summary>
        /// Inverse transform the specified screen point.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="yaxis">The y-axis.</param>
        /// <returns>The data point.</returns>
        public virtual DataPoint InverseTransform(double x, double y, Axis yaxis)
        {
            return new DataPoint(this.InverseTransform(x), yaxis != null ? yaxis.InverseTransform(y) : 0);
        }

        /// <summary>
        /// Inverse transforms the specified screen coordinate. This method can only be used with non-polar coordinate systems.
        /// </summary>
        /// <param name="sx">The screen coordinate.</param>
        /// <returns>The value.</returns>
        public virtual double InverseTransform(double sx)
        {
            return (sx / this.scale) + this.offset;
        }

        /// <summary>
        /// Determines whether the axis is horizontal.
        /// </summary>
        /// <returns><c>true</c> if the axis is horizontal; otherwise, <c>false</c> .</returns>
        public bool IsHorizontal()
        {
            return this.position == AxisPosition.Top || this.position == AxisPosition.Bottom;
        }

        /// <summary>
        /// Determines whether the specified value is valid.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is valid; otherwise, <c>false</c> .</returns>
        public virtual bool IsValidValue(double value)
        {
#pragma warning disable 1718
            // ReSharper disable EqualExpressionComparison
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return value == value &&
                value != 1.0 / 0.0 &&
                value != -1.0 / 0.0 &&
                value < this.FilterMaxValue &&
                value > this.FilterMinValue &&
                (this.FilterFunction == null || this.FilterFunction(value));
            // ReSharper restore CompareOfFloatsByEqualityOperator
            // ReSharper restore EqualExpressionComparison
#pragma warning restore 1718
        }

        /// <summary>
        /// Determines whether the axis is vertical.
        /// </summary>
        /// <returns><c>true</c> if the axis is vertical; otherwise, <c>false</c> .</returns>
        public bool IsVertical()
        {
            return this.position == AxisPosition.Left || this.position == AxisPosition.Right;
        }

        /// <summary>
        /// Determines whether the axis is used for X/Y values.
        /// </summary>
        /// <returns><c>true</c> if it is an XY axis; otherwise, <c>false</c> .</returns>
        public abstract bool IsXyAxis();

        /// <summary>
        /// Measures the size of the axis (maximum axis label width/height).
        /// </summary>
        /// <param name="rc">The render context.</param>
        /// <returns>The size of the axis.</returns>
        public virtual OxySize Measure(IRenderContext rc)
        {
            IList<double> majorTickValues;
            IList<double> minorTickValues;
            IList<double> majorLabelValues;

            this.GetTickValues(out majorLabelValues, out majorTickValues, out minorTickValues);

            var maximumTextSize = new OxySize();
            foreach (double v in majorLabelValues)
            {
                string s = this.FormatValue(v);
                var size = rc.MeasureText(s, this.ActualFont, this.ActualFontSize, this.ActualFontWeight);
                if (size.Width > maximumTextSize.Width)
                {
                    maximumTextSize.Width = size.Width;
                }

                if (size.Height > maximumTextSize.Height)
                {
                    maximumTextSize.Height = size.Height;
                }
            }

            var labelTextSize = rc.MeasureText(
                this.ActualTitle, this.ActualFont, this.ActualFontSize, this.ActualFontWeight);

            double width = 0;
            double height = 0;

            if (this.IsVertical())
            {
                switch (this.TickStyle)
                {
                    case TickStyle.Outside:
                        width += this.MajorTickSize;
                        break;
                    case TickStyle.Crossing:
                        width += this.MajorTickSize * 0.75;
                        break;
                }

                width += this.AxisDistance;
                width += this.AxisTickToLabelDistance;
                width += maximumTextSize.Width;
                if (labelTextSize.Height > 0)
                {
                    width += this.AxisTitleDistance;
                    width += labelTextSize.Height;
                }
            }
            else
            {
                // caution: this includes AngleAxis because Position=None
                switch (this.TickStyle)
                {
                    case TickStyle.Outside:
                        height += this.MajorTickSize;
                        break;
                    case TickStyle.Crossing:
                        height += this.MajorTickSize * 0.75;
                        break;
                }

                height += this.AxisDistance;
                height += this.AxisTickToLabelDistance;
                height += maximumTextSize.Height;
                if (labelTextSize.Height > 0)
                {
                    height += this.AxisTitleDistance;
                    height += labelTextSize.Height;
                }
            }

            return new OxySize(width, height);
        }

        /// <summary>
        /// Pans the specified axis.
        /// </summary>
        /// <param name="ppt">The previous point (screen coordinates).</param>
        /// <param name="cpt">The current point (screen coordinates).</param>
        public virtual void Pan(ScreenPoint ppt, ScreenPoint cpt)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }

            bool isHorizontal = this.IsHorizontal();

            double dsx = isHorizontal ? cpt.X - ppt.X : cpt.Y - ppt.Y;
            this.Pan(dsx);
        }

        /// <summary>
        /// Pans the specified axis.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public virtual void Pan(double delta)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }

            double dx = delta / this.Scale;

            double newMinimum = this.ActualMinimum - dx;
            double newMaximum = this.ActualMaximum - dx;
            if (newMinimum < this.AbsoluteMinimum)
            {
                newMinimum = this.AbsoluteMinimum;
                newMaximum = Math.Min(newMinimum + this.ActualMaximum - this.ActualMinimum, this.AbsoluteMaximum);
            }

            if (newMaximum > this.AbsoluteMaximum)
            {
                newMaximum = this.AbsoluteMaximum;
                newMinimum = Math.Max(newMaximum - (this.ActualMaximum - this.ActualMinimum), this.AbsoluteMinimum);
            }

            this.ViewMinimum = newMinimum;
            this.ViewMaximum = newMaximum;
            this.UpdateActualMaxMin();

            this.OnAxisChanged(new AxisChangedEventArgs(AxisChangeTypes.Pan));
        }

        /// <summary>
        /// Renders the axis on the specified render context.
        /// </summary>
        /// <param name="rc">The render context.</param>
        /// <param name="model">The model.</param>
        /// <param name="axisLayer">The rendering order.</param>
        /// <param name="pass">The pass.</param>
        public virtual void Render(IRenderContext rc, PlotModel model, AxisLayer axisLayer, int pass)
        {
            var r = new HorizontalAndVerticalAxisRenderer(rc, model);
            r.Render(this, pass);
        }

        /// <summary>
        /// Resets the user's modification (zooming/panning) to minimum and maximum of this axis.
        /// </summary>
        public virtual void Reset()
        {
            this.ViewMinimum = double.NaN;
            this.ViewMaximum = double.NaN;
            this.UpdateActualMaxMin();
            this.OnAxisChanged(new AxisChangedEventArgs(AxisChangeTypes.Reset));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(
                this.ActualCulture,
                "{0}({1}, {2}, {3}, {4})",
                this.GetType().Name,
                this.Position,
                this.ActualMinimum,
                this.ActualMaximum,
                this.ActualMajorStep);
        }

        /// <summary>
        /// Transforms the specified point to screen coordinates.
        /// </summary>
        /// <param name="x">The x value (for the current axis).</param>
        /// <param name="y">The y value.</param>
        /// <param name="yaxis">The y axis.</param>
        /// <returns>The transformed point.</returns>
        public virtual ScreenPoint Transform(double x, double y, Axis yaxis)
        {
            if (yaxis == null)
            {
                throw new NullReferenceException("Y axis should not be null when transforming.");
            }

            return new ScreenPoint(this.Transform(x), yaxis.Transform(y));
        }

        /// <summary>
        /// Transforms the specified coordinate to screen coordinates. This method can only be used with non-polar coordinate systems.
        /// </summary>
        /// <param name="x">The value.</param>
        /// <returns>The transformed value (screen coordinate).</returns>
        public virtual double Transform(double x)
        {
            return (x - this.offset) * this.scale;
        }

        /// <summary>
        /// Zoom to the specified scale.
        /// </summary>
        /// <param name="newScale">The new scale.</param>
        public virtual void Zoom(double newScale)
        {
            double sx1 = this.Transform(this.ActualMaximum);
            double sx0 = this.Transform(this.ActualMinimum);

            double sgn = Math.Sign(this.scale);
            double mid = (this.ActualMaximum + this.ActualMinimum) / 2;

            double dx = (this.offset - mid) * this.scale;
            var newOffset = (dx / (sgn * newScale)) + mid;
            this.SetTransform(sgn * newScale, newOffset);

            double newMaximum = this.InverseTransform(sx1);
            double newMinimum = this.InverseTransform(sx0);

            if (newMinimum < this.AbsoluteMinimum && newMaximum > this.AbsoluteMaximum)
            {
                newMinimum = this.AbsoluteMinimum;
                newMaximum = this.AbsoluteMaximum;
            }
            else
            {
                if (newMinimum < this.AbsoluteMinimum)
                {
                    double d = newMaximum - newMinimum;
                    newMinimum = this.AbsoluteMinimum;
                    newMaximum = this.AbsoluteMinimum + d;
                    if (newMaximum > this.AbsoluteMaximum)
                    {
                        newMaximum = this.AbsoluteMaximum;
                    }
                }
                else if (newMaximum > this.AbsoluteMaximum)
                {
                    double d = newMaximum - newMinimum;
                    newMaximum = this.AbsoluteMaximum;
                    newMinimum = this.AbsoluteMaximum - d;
                    if (newMinimum < this.AbsoluteMinimum)
                    {
                        newMinimum = this.AbsoluteMinimum;
                    }
                }
            }

            this.ViewMaximum = newMaximum;
            this.ViewMinimum = newMinimum;
            this.UpdateActualMaxMin();
        }

        /// <summary>
        /// Zooms the axis to the range [x0,x1].
        /// </summary>
        /// <param name="x0">The new minimum.</param>
        /// <param name="x1">The new maximum.</param>
        public virtual void Zoom(double x0, double x1)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            double newMinimum = Math.Max(Math.Min(x0, x1), this.AbsoluteMinimum);
            double newMaximum = Math.Min(Math.Max(x0, x1), this.AbsoluteMaximum);

            this.ViewMinimum = newMinimum;
            this.ViewMaximum = newMaximum;
            this.UpdateActualMaxMin();

            this.OnAxisChanged(new AxisChangedEventArgs(AxisChangeTypes.Zoom));
        }

        /// <summary>
        /// Zooms the axis at the specified coordinate.
        /// </summary>
        /// <param name="factor">The zoom factor.</param>
        /// <param name="x">The coordinate to zoom at.</param>
        public virtual void ZoomAt(double factor, double x)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            double dx0 = (this.ActualMinimum - x) * this.scale;
            double dx1 = (this.ActualMaximum - x) * this.scale;
            this.scale *= factor;

            double newMinimum = Math.Max((dx0 / this.scale) + x, this.AbsoluteMinimum);
            double newMaximum = Math.Min((dx1 / this.scale) + x, this.AbsoluteMaximum);

            this.ViewMinimum = newMinimum;
            this.ViewMaximum = newMaximum;
            this.UpdateActualMaxMin();

            this.OnAxisChanged(new AxisChangedEventArgs(AxisChangeTypes.Zoom));
        }

        /// <summary>
        /// Zooms the axis with the specified zoom factor at the center of the axis.
        /// </summary>
        /// <param name="factor">The zoom factor.</param>
        public virtual void ZoomAtCenter(double factor)
        {
            double sx = (this.Transform(this.ActualMaximum) + this.Transform(this.ActualMinimum)) * 0.5;
            var x = this.InverseTransform(sx);
            this.ZoomAt(factor, x);
        }

        /// <summary>
        /// Modifies the data range of the axis [DataMinimum,DataMaximum] to includes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void Include(double value)
        {
            if (!this.IsValidValue(value))
            {
                return;
            }

            this.DataMinimum = double.IsNaN(this.DataMinimum) ? value : Math.Min(this.DataMinimum, value);
            this.DataMaximum = double.IsNaN(this.DataMaximum) ? value : Math.Max(this.DataMaximum, value);
        }

        /// <summary>
        /// Resets the <see cref="DataMaximum" /> and <see cref="DataMinimum" /> values.
        /// </summary>
        internal virtual void ResetDataMaxMin()
        {
            this.DataMaximum = this.DataMinimum = this.ActualMaximum = this.ActualMinimum = double.NaN;
        }

        /// <summary>
        /// Updates the <see cref="ActualMaximum" /> and <see cref="ActualMinimum" /> values.
        /// </summary>
        /// <remarks>If the user has zoomed/panned the axis, the internal ViewMaximum/ViewMinimum
        /// values will be used. If Maximum or Minimum have been set, these values will be used. Otherwise the maximum and minimum values
        /// of the series will be used, including the 'padding'.</remarks>
        internal virtual void UpdateActualMaxMin()
        {
            if (!double.IsNaN(this.ViewMaximum))
            {
                // The user has zoomed/panned the axis, use the ViewMaximum value.
                this.ActualMaximum = this.ViewMaximum;
            }
            else if (!double.IsNaN(this.Maximum))
            {
                // The Maximum value has been set
                this.ActualMaximum = this.Maximum;
            }
            else
            {
                // Calculate the actual maximum, including padding
                this.ActualMaximum = this.CalculateActualMaximum();
            }

            if (!double.IsNaN(this.ViewMinimum))
            {
                this.ActualMinimum = this.ViewMinimum;
            }
            else if (!double.IsNaN(this.Minimum))
            {
                this.ActualMinimum = this.Minimum;
            }
            else
            {
                this.ActualMinimum = this.CalculateActualMinimum();
            }

            this.CoerceActualMaxMin();
        }

        /// <summary>
        /// Updates the axis with information from the plot series.
        /// </summary>
        /// <param name="series">The series collection.</param>
        /// <remarks>This is used by the category axis that need to know the number of series using the axis.</remarks>
        internal virtual void UpdateFromSeries(Series[] series)
        {
        }

        /// <summary>
        /// Updates the actual minor and major step intervals.
        /// </summary>
        /// <param name="plotArea">The plot area rectangle.</param>
        internal virtual void UpdateIntervals(OxyRect plotArea)
        {
            double labelSize = this.IntervalLength;
            double length = this.IsHorizontal() ? plotArea.Width : plotArea.Height;
            length *= Math.Abs(this.EndPosition - this.StartPosition);

            this.ActualMajorStep = !double.IsNaN(this.MajorStep)
                                       ? this.MajorStep
                                       : this.CalculateActualInterval(length, labelSize);

            this.ActualMinorStep = !double.IsNaN(this.MinorStep)
                                       ? this.MinorStep
                                       : this.CalculateMinorInterval(this.ActualMajorStep);

            if (double.IsNaN(this.ActualMinorStep))
            {
                this.ActualMinorStep = 2;
            }

            if (double.IsNaN(this.ActualMajorStep))
            {
                this.ActualMajorStep = 10;
            }

            this.ActualStringFormat = this.StringFormat;

            // if (ActualStringFormat==null)
            // {
            // if (ActualMaximum > 1e6 || ActualMinimum < 1e-6)
            // ActualStringFormat = "#.#e-0";
            // }
        }

        /// <summary>
        /// Updates the scale and offset properties of the transform from the specified boundary rectangle.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        internal virtual void UpdateTransform(OxyRect bounds)
        {
            double x0 = bounds.Left;
            double x1 = bounds.Right;
            double y0 = bounds.Bottom;
            double y1 = bounds.Top;

            this.ScreenMin = new ScreenPoint(x0, y1);
            this.ScreenMax = new ScreenPoint(x1, y0);

            double a0 = this.IsHorizontal() ? x0 : y0;
            double a1 = this.IsHorizontal() ? x1 : y1;

            double dx = a1 - a0;
            a1 = a0 + (this.EndPosition * dx);
            a0 = a0 + (this.StartPosition * dx);
            this.ScreenMin = new ScreenPoint(a0, a1);
            this.ScreenMax = new ScreenPoint(a1, a0);

            if (this.ActualMaximum - this.ActualMinimum < double.Epsilon)
            {
                this.ActualMaximum = this.ActualMinimum + 1;
            }

            double max = this.PreTransform(this.ActualMaximum);
            double min = this.PreTransform(this.ActualMinimum);

            double da = a0 - a1;
            double newOffset, newScale;
            if (Math.Abs(da) > double.Epsilon)
            {
                newOffset = (a0 / da * max) - (a1 / da * min);
            }
            else
            {
                newOffset = 0;
            }

            double range = max - min;
            if (Math.Abs(range) > double.Epsilon)
            {
                newScale = (a1 - a0) / range;
            }
            else
            {
                newScale = 1;
            }

            this.SetTransform(newScale, newOffset);
        }

        /// <summary>
        /// Resets the current values.
        /// </summary>
        /// <remarks>The current values may be modified during update of max/min and rendering.</remarks>
        protected internal virtual void ResetCurrentValues()
        {
        }

        /// <summary>
        /// Applies a transformation after the inverse transform of the value.
        /// </summary>
        /// <param name="x">The value to transform.</param>
        /// <returns>The transformed value.</returns>
        /// <remarks>If this method is overridden, the <see cref="InverseTransform(double)" /> method must also be overridden.
        /// See <see cref="LogarithmicAxis" /> for examples on how to implement this.</remarks>
        protected virtual double PostInverseTransform(double x)
        {
            return x;
        }

        /// <summary>
        /// Applies a transformation before the transform the value.
        /// </summary>
        /// <param name="x">The value to transform.</param>
        /// <returns>The transformed value.</returns>
        /// <remarks>If this method is overridden, the <see cref="Transform(double)" /> method must also be overridden.
        /// See <see cref="LogarithmicAxis" /> for examples on how to implement this.</remarks>
        protected virtual double PreTransform(double x)
        {
            return x;
        }

        /// <summary>
        /// Calculates the actual maximum value of the axis, including the <see cref="MaximumPadding" />.
        /// </summary>
        /// <returns>The new actual maximum value of the axis.</returns>
        protected virtual double CalculateActualMaximum()
        {
            var actualMaximum = this.DataMaximum;
            double range = this.DataMaximum - this.DataMinimum;

            if (range < double.Epsilon)
            {
                double zeroRange = this.DataMaximum > 0 ? this.DataMaximum : 1;
                actualMaximum += zeroRange * 0.5;
            }

            if (!double.IsNaN(this.DataMinimum) && !double.IsNaN(actualMaximum))
            {
                double x1 = this.PreTransform(actualMaximum);
                double x0 = this.PreTransform(this.DataMinimum);
                double dx = this.MaximumPadding * (x1 - x0);
                return this.PostInverseTransform(x1 + dx);
            }

            return actualMaximum;
        }

        /// <summary>
        /// Calculates the actual minimum value of the axis, including the <see cref="MinimumPadding" />.
        /// </summary>
        /// <returns>The new actual minimum value of the axis.</returns>
        protected virtual double CalculateActualMinimum()
        {
            var actualMinimum = this.DataMinimum;
            double range = this.DataMaximum - this.DataMinimum;

            if (range < double.Epsilon)
            {
                double zeroRange = this.DataMaximum > 0 ? this.DataMaximum : 1;
                actualMinimum -= zeroRange * 0.5;
            }

            if (!double.IsNaN(this.ActualMaximum))
            {
                double x1 = this.PreTransform(this.ActualMaximum);
                double x0 = this.PreTransform(actualMinimum);
                double dx = this.MinimumPadding * (x1 - x0);
                return this.PostInverseTransform(x0 - dx);
            }

            return actualMinimum;
        }

        /// <summary>
        /// Sets the transform.
        /// </summary>
        /// <param name="newScale">The new scale.</param>
        /// <param name="newOffset">The new offset.</param>
        protected void SetTransform(double newScale, double newOffset)
        {
            this.scale = newScale;
            this.offset = newOffset;
            this.OnTransformChanged(new EventArgs());
        }

        /// <summary>
        /// Calculates the actual interval.
        /// </summary>
        /// <param name="availableSize">Size of the available area.</param>
        /// <param name="maxIntervalSize">Maximum length of the intervals.</param>
        /// <returns>The calculate actual interval.</returns>
        protected virtual double CalculateActualInterval(double availableSize, double maxIntervalSize)
        {
            return this.CalculateActualInterval(availableSize, maxIntervalSize, this.ActualMaximum - this.ActualMinimum);
        }

        /// <summary>
        /// Returns the actual interval to use to determine which values are displayed in the axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <param name="maxIntervalSize">The maximum interval size.</param>
        /// <param name="range">The range.</param>
        /// <returns>Actual interval to use to determine which values are displayed in the axis.</returns>
        protected double CalculateActualInterval(double availableSize, double maxIntervalSize, double range)
        {
            if (availableSize <= 0)
            {
                return maxIntervalSize;
            }

            Func<double, double> exponent = x => Math.Ceiling(Math.Log(x, 10));
            Func<double, double> mantissa = x => x / Math.Pow(10, exponent(x) - 1);

            // reduce intervals for horizontal axis.
            // double maxIntervals = Orientation == AxisOrientation.x ? MaximumAxisIntervalsPer200Pixels * 0.8 : MaximumAxisIntervalsPer200Pixels;
            // real maximum interval count
            double maxIntervalCount = availableSize / maxIntervalSize;

            range = Math.Abs(range);
            double interval = Math.Pow(10, exponent(range));
            double intervalCandidate = interval;

            // Function to remove 'double precision noise'
            // TODO: can this be improved
            Func<double, double> removeNoise = x => double.Parse(x.ToString("e14"));

            // decrease interval until interval count becomes less than maxIntervalCount
            while (true)
            {
                var m = (int)mantissa(intervalCandidate);
                if (m == 5)
                {
                    // reduce 5 to 2
                    intervalCandidate = removeNoise(intervalCandidate / 2.5);
                }
                else if (m == 2 || m == 1 || m == 10)
                {
                    // reduce 2 to 1, 10 to 5, 1 to 0.5
                    intervalCandidate = removeNoise(intervalCandidate / 2.0);
                }
                else
                {
                    intervalCandidate = removeNoise(intervalCandidate / 2.0);
                }

                if (range / intervalCandidate > maxIntervalCount)
                {
                    break;
                }

                if (double.IsNaN(intervalCandidate) || double.IsInfinity(intervalCandidate))
                {
                    break;
                }

                interval = intervalCandidate;
            }

            return interval;
        }

        /// <summary>
        /// Calculates the minor interval.
        /// </summary>
        /// <param name="majorInterval">The major interval.</param>
        /// <returns>The minor interval.</returns>
        protected double CalculateMinorInterval(double majorInterval)
        {
            // if major interval is 100, the minor interval will be 20.
            return majorInterval / 5;

            // The following obsolete code divided major intervals into 4 minor intervals, unless the major interval's mantissa was 5.
            // e.g. Major interval 100 => minor interval 25.

            // Func<double, double> exponent = x => Math.Ceiling(Math.Log(x, 10));
            // Func<double, double> mantissa = x => x / Math.Pow(10, exponent(x) - 1);
            // var m = (int)mantissa(majorInterval);
            // switch (m)
            // {
            // case 5:
            // return majorInterval / 5;
            // default:
            // return majorInterval / 4;
            // }
        }

        /// <summary>
        /// Raises the <see cref="AxisChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="OxyPlot.Axes.AxisChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnAxisChanged(AxisChangedEventArgs args)
        {
            this.UpdateActualMaxMin();

            var handler = this.AxisChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="TransformChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnTransformChanged(EventArgs args)
        {
            var handler = this.TransformChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}