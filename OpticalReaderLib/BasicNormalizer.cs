/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    /// <summary>
    /// Basic generic normalizer implementation.
    /// </summary>
    public class BasicNormalizer : INormalizer
    {
        public async Task<NormalizeResult> NormalizeAsync(Frame frame, Windows.Foundation.Rect area, double rotation)
        {
            using (var bitmap = new Bitmap(frame.Dimensions, Internal.Utilities.FrameFormatToColorMode(frame.Format), frame.Pitch, frame.Buffer.AsBuffer()))
            using (var source = new BitmapImageSource(bitmap))
            using (var effect = new FilterEffect(source))
            using (var renderer = new BitmapRenderer(effect))
            {
                effect.Filters = new List<IFilter>()
                {
                    new ReframingFilter(area, -rotation)
                };

                using (var newBitmap = new Bitmap(new Windows.Foundation.Size(area.Width, area.Height), Internal.Utilities.FrameFormatToColorMode(frame.Format)))
                {
                    await effect.GetBitmapAsync(newBitmap, OutputOption.PreserveAspectRatio);

                    return new NormalizeResult()
                    {
                        Frame = new Frame()
                        {
                            Buffer = newBitmap.Buffers[0].Buffer.ToArray(),
                            Pitch = newBitmap.Buffers[0].Pitch,
                            Format = frame.Format,
                            Dimensions = newBitmap.Dimensions
                        },
                        Translate = new Func<Windows.Foundation.Point, Windows.Foundation.Point>((normalizedPoint) =>
                        {
                            var rotationRadians = -rotation / 360.0 * 2.0 * Math.PI;
                            var sin = Math.Sin(rotationRadians);
                            var cos = Math.Cos(rotationRadians);
                            var origoX = area.Width / 2.0;
                            var origoY = area.Height / 2.0;

                            // Translate point to origo before rotation
                            var ox = normalizedPoint.X - origoX;
                            var oy = normalizedPoint.Y - origoY;

                            // Move area to origo, calculate new point positions, restore area location and add crop margins
                            var x = ox * cos - oy * sin;
                            var y = ox * sin + oy * cos;

                            // Translate point back to area after rotation
                            x = x + origoX;
                            y = y + origoY;

                            // Add margins from original uncropped frame
                            x = x + area.X;
                            y = y + area.Y;

                            return new Windows.Foundation.Point(x, y);
                        })
                    };
                }
            }
        }
    }
}
