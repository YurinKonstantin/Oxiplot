﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoxPlotSeries.cs" company="OxyPlot">
//   http://oxyplot.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a series for box plots.
    /// </summary>
    public class BoxPlotSeries : XYAxisSeries
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxPlotSeries"/> class. 
        /// </summary>
        public BoxPlotSeries()
        {
            this.Items = new List<BoxPlotItem>();
            this.TrackerFormatString =
                "X: {1:0.00}\nUpper Whisker: {2:0.00}\nThird Quartil: {3:0.00}\nMedian: {4:0.00}\nFirst Quartil: {5:0.00}\nLower Whisker: {6:0.00}";
            this.OutlierTrackerFormatString =
                "X: {1:0.00}\nY: {2:0.00}";
            this.Title = null;
            this.Fill = null;
            this.Stroke = OxyColors.Black;
            this.BoxWidth = 20;
            this.StrokeThickness = 1;
            this.OutlierSize = 2;
            this.WhiskerWidth = 0.5;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the width of the boxes (specified in x-axis units).
        /// </summary>
        /// <value> The width of the boxes. </value>
        public double BoxWidth { get; set; }

        /// <summary>
        ///   Gets or sets the fill color. If null, this color will be automatically set.
        /// </summary>
        /// <value> The fill color. </value>
        public OxyColor Fill { get; set; }

        /// <summary>
        ///   Gets or sets the box plot items.
        /// </summary>
        /// <value> The items. </value>
        public IList<BoxPlotItem> Items { get; set; }

        /// <summary>
        ///   Gets or sets the diameter of the outlier circles (specified in points).
        /// </summary>
        /// <value> The size of the outlier. </value>
        public double OutlierSize { get; set; }

        /// <summary>
        /// Gets or sets the tracker format string for the outliers.
        /// </summary>
        /// <value>
        /// The tracker format string for the outliers.
        /// </value>
        /// <remarks>
        /// Use {0} for series title, {1} for x- and {2} for y-value.
        /// </remarks>
        public string OutlierTrackerFormatString { get; set; }

        /// <summary>
        ///   Gets or sets the stroke.
        /// </summary>
        /// <value> The stroke. </value>
        public OxyColor Stroke { get; set; }

        /// <summary>
        ///   Gets or sets the stroke thickness.
        /// </summary>
        /// <value> The stroke thickness. </value>
        public double StrokeThickness { get; set; }

        /// <summary>
        ///   Gets or sets the width of the whiskers (relative to the BoxWidth).
        /// </summary>
        /// <value> The width of the whiskers. </value>
        public double WhiskerWidth { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the nearest point.
        /// </summary>
        /// <param name="point">
        /// The point. 
        /// </param>
        /// <param name="interpolate">
        /// interpolate if set to <c>true</c> . 
        /// </param>
        /// <returns>
        /// A TrackerHitResult for the current hit. 
        /// </returns>
        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            double minimumDistance = double.MaxValue;
            var result = new TrackerHitResult(this, DataPoint.Undefined, ScreenPoint.Undefined);
            foreach (var item in this.Items)
            {
                foreach (var outlier in item.Outliers)
                {
                    var sp = this.Transform(item.X, outlier);
                    double d = (sp - point).LengthSquared;
                    if (d < minimumDistance)
                    {
                        result.DataPoint = new DataPoint(item.X, outlier);
                        result.Position = sp;
                        result.Item = item;
                        result.Text = StringHelper.Format(
                            this.ActualCulture,
                            this.OutlierTrackerFormatString,
                            item,
                            this.Title,
                            result.DataPoint.X,
                            outlier);
                        minimumDistance = d;
                    }
                }

                // check if we are inside the box rectangle
                var rect = this.GetBoxRect(item);
                if (rect.Contains(point))
                {
                    result.DataPoint = new DataPoint(item.X, this.YAxis.InverseTransform(point.Y));
                    result.Position = this.Transform(result.DataPoint);
                    result.Item = item;

                    result.Text = StringHelper.Format(
                        this.ActualCulture,
                        this.TrackerFormatString,
                        item,
                        this.Title,
                        result.DataPoint.X,
                        item.UpperWhisker,
                        item.BoxTop,
                        item.Median,
                        item.BoxBottom,
                        item.LowerWhisker);

                    minimumDistance = 0;
                }

                var topWhisker = this.Transform(item.X, item.UpperWhisker);
                var bottomWhisker = this.Transform(item.X, item.LowerWhisker);

                // check if we are near the line
                var p = ScreenPointHelper.FindPointOnLine(point, topWhisker, bottomWhisker);
                double d2 = (p - point).LengthSquared;
                if (d2 < minimumDistance)
                {
                    result.DataPoint = this.InverseTransform(p);
                    result.Position = this.Transform(result.DataPoint);
                    result.Item = item;
                    result.Text = StringHelper.Format(
                        this.ActualCulture,
                        this.TrackerFormatString,
                        item,
                        this.Title,
                        result.DataPoint.X,
                        item.UpperWhisker,
                        item.BoxTop,
                        item.Median,
                        item.BoxBottom,
                        item.LowerWhisker);
                    minimumDistance = d2;
                }
            }

            if (minimumDistance < double.MaxValue)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified item contains a valid point.
        /// </summary>
        /// <param name="item">
        /// The item. 
        /// </param>
        /// <param name="xaxis">
        /// The x axis. 
        /// </param>
        /// <param name="yaxis">
        /// The y axis. 
        /// </param>
        /// <returns>
        /// <c>true</c> if the point is valid; otherwise, <c>false</c> . 
        /// </returns>
        public virtual bool IsValidPoint(BoxPlotItem item, Axis xaxis, Axis yaxis)
        {
            return !double.IsNaN(item.X) && !double.IsInfinity(item.X) && !item.Values.Any(double.IsNaN)
                   && !item.Values.Any(double.IsInfinity) && (xaxis != null && xaxis.IsValidValue(item.X))
                   && (yaxis != null && item.Values.All(yaxis.IsValidValue));
        }

        /// <summary>
        /// Renders the series on the specified render context.
        /// </summary>
        /// <param name="rc">
        /// The rendering context. 
        /// </param>
        /// <param name="model">
        /// The model. 
        /// </param>
        public override void Render(IRenderContext rc, PlotModel model)
        {
            base.Render(rc, model);

            if (this.Items.Count == 0)
            {
                return;
            }

            var clippingRect = this.GetClippingRect();

            var outlierScreenPoints = new List<ScreenPoint>();
            var halfBoxWidth = this.BoxWidth * 0.5;
            var halfWhiskerWidth = halfBoxWidth * this.WhiskerWidth;
            var strokeColor = this.GetSelectableColor(this.Stroke);
            var fillColor = this.GetSelectableFillColor(this.Fill);

            foreach (var item in this.Items)
            {
                // Add the outlier points
                foreach (var outlier in item.Outliers)
                {
                    var screenPoint = this.Transform(item.X, outlier);
                    outlierScreenPoints.Add(screenPoint);
                }

                // Draw the whiskers
                var topWhiskerTop = this.Transform(item.X, item.UpperWhisker);
                var topWhiskerBottom = this.Transform(item.X, item.BoxTop);
                var topWhiskerLine1 = this.Transform(item.X - halfWhiskerWidth, item.UpperWhisker);
                var topWhiskerLine2 = this.Transform(item.X + halfWhiskerWidth, item.UpperWhisker);
                var bottomWhiskerTop = this.Transform(item.X, item.BoxBottom);
                var bottomWhiskerBottom = this.Transform(item.X, item.LowerWhisker);
                var bottomWhiskerLine1 = this.Transform(item.X - halfWhiskerWidth, item.LowerWhisker);
                var bottomWhiskerLine2 = this.Transform(item.X + halfWhiskerWidth, item.LowerWhisker);
                rc.DrawClippedLine(
                    new[] { topWhiskerTop, topWhiskerBottom },
                    clippingRect,
                    0,
                    strokeColor,
                    this.StrokeThickness,
                    LineStyle.Dash,
                    OxyPenLineJoin.Miter,
                    true);
                rc.DrawClippedLine(
                    new[] { bottomWhiskerTop, bottomWhiskerBottom },
                    clippingRect,
                    0,
                    strokeColor,
                    this.StrokeThickness,
                    LineStyle.Dash,
                    OxyPenLineJoin.Miter,
                    true);
                rc.DrawClippedLine(
                    new[] { topWhiskerLine1, topWhiskerLine2 },
                    clippingRect,
                    0,
                    strokeColor,
                    this.StrokeThickness,
                    LineStyle.Solid,
                    OxyPenLineJoin.Miter,
                    true);
                rc.DrawClippedLine(
                    new[] { bottomWhiskerLine1, bottomWhiskerLine2 },
                    clippingRect,
                    0,
                    strokeColor,
                    this.StrokeThickness,
                    LineStyle.Solid,
                    OxyPenLineJoin.Miter,
                    true);

                // Draw the box
                var rect = this.GetBoxRect(item);
                rc.DrawClippedRectangleAsPolygon(rect, clippingRect, fillColor, strokeColor, this.StrokeThickness);

                // Draw the median
                var medianLeft = this.Transform(item.X - halfBoxWidth, item.Median);
                var medianRight = this.Transform(item.X + halfBoxWidth, item.Median);
                rc.DrawClippedLine(
                    new[] { medianLeft, medianRight },
                    clippingRect,
                    0,
                    strokeColor,
                    this.StrokeThickness * 2,
                    LineStyle.Solid,
                    OxyPenLineJoin.Miter,
                    true);
            }

            // Draw the outlier(s)
            var markerSizes = outlierScreenPoints.Select(o => this.OutlierSize).ToList();
            rc.DrawMarkers(
                outlierScreenPoints,
                clippingRect,
                MarkerType.Circle,
                null,
                markerSizes,
                fillColor,
                strokeColor,
                this.StrokeThickness);
        }

        /// <summary>
        /// Renders the legend symbol on the specified rendering context.
        /// </summary>
        /// <param name="rc">
        /// The rendering context. 
        /// </param>
        /// <param name="legendBox">
        /// The legend rectangle. 
        /// </param>
        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            double xmid = (legendBox.Left + legendBox.Right) / 2;
            double yopen = legendBox.Top + ((legendBox.Bottom - legendBox.Top) * 0.7);
            double yclose = legendBox.Top + ((legendBox.Bottom - legendBox.Top) * 0.3);

            var halfBoxWidth = legendBox.Width * 0.24;
            var halfWhiskerWidth = halfBoxWidth * this.WhiskerWidth;
            double strokeThickness = 1;
            var strokeColor = this.GetSelectableColor(this.Stroke);
            var fillColor = this.GetSelectableFillColor(this.Fill);

            rc.DrawLine(
                new[] { new ScreenPoint(xmid, legendBox.Top), new ScreenPoint(xmid, legendBox.Bottom) },
                strokeColor,
                strokeThickness,
                LineStyleHelper.GetDashArray(LineStyle.Solid),
                OxyPenLineJoin.Miter,
                true);

            // top whisker
            rc.DrawLine(
                new[]
                    {
                        new ScreenPoint(xmid - halfWhiskerWidth - 1, legendBox.Bottom), 
                        new ScreenPoint(xmid + halfWhiskerWidth, legendBox.Bottom)
                    },
                strokeColor,
                strokeThickness,
                LineStyleHelper.GetDashArray(LineStyle.Solid),
                OxyPenLineJoin.Miter,
                true);

            // bottom whisker
            rc.DrawLine(
                new[]
                    {
                        new ScreenPoint(xmid - halfWhiskerWidth - 1, legendBox.Top), 
                        new ScreenPoint(xmid + halfWhiskerWidth, legendBox.Top)
                    },
                strokeColor,
                strokeThickness,
                LineStyleHelper.GetDashArray(LineStyle.Solid),
                OxyPenLineJoin.Miter,
                true);

            // box
            rc.DrawRectangleAsPolygon(
                new OxyRect(xmid - halfBoxWidth, yclose, 2 * halfBoxWidth, yopen - yclose),
                fillColor,
                strokeColor,
                strokeThickness);

            // median
            rc.DrawLine(
                new[]
                    {
                        new ScreenPoint(xmid - halfBoxWidth, (yopen + yclose) * 0.5), 
                        new ScreenPoint(xmid + halfBoxWidth, (yopen + yclose) * 0.5)
                    },
                strokeColor,
                strokeThickness,
                LineStyleHelper.GetDashArray(LineStyle.Solid),
                OxyPenLineJoin.Miter,
                true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the max/minimum values.
        /// </summary>
        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();
            this.InternalUpdateMaxMin(this.Items);
        }

        /// <summary>
        /// Updates the max and min of the series.
        /// </summary>
        /// <param name="items">
        /// The items. 
        /// </param>
        protected void InternalUpdateMaxMin(IList<BoxPlotItem> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            double minx = this.MinX;
            double miny = this.MinY;
            double maxx = this.MaxX;
            double maxy = this.MaxY;

            foreach (var pt in items)
            {
                if (!this.IsValidPoint(pt, this.XAxis, this.YAxis))
                {
                    continue;
                }

                var x = pt.X;
                if (x < minx || double.IsNaN(minx))
                {
                    minx = x;
                }

                if (x > maxx || double.IsNaN(maxx))
                {
                    maxx = x;
                }

                foreach (var y in pt.Values)
                {
                    if (y < miny || double.IsNaN(miny))
                    {
                        miny = y;
                    }

                    if (y > maxy || double.IsNaN(maxy))
                    {
                        maxy = y;
                    }
                }
            }

            this.MinX = minx;
            this.MinY = miny;
            this.MaxX = maxx;
            this.MaxY = maxy;
        }

        /// <summary>
        /// Gets the screen rectangle for the box.
        /// </summary>
        /// <param name="item">
        /// The box item. 
        /// </param>
        /// <returns>
        /// A rectangle. 
        /// </returns>
        private OxyRect GetBoxRect(BoxPlotItem item)
        {
            var halfBoxWidth = this.BoxWidth * 0.5;

            var boxTop = this.Transform(item.X - halfBoxWidth, item.BoxTop);
            var boxBottom = this.Transform(item.X + halfBoxWidth, item.BoxBottom);

            var rect = new OxyRect(boxTop.X, boxTop.Y, boxBottom.X - boxTop.X, boxBottom.Y - boxTop.Y);
            return rect;
        }

        #endregion
    }
}