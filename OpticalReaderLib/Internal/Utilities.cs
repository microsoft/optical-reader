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
using System.Threading.Tasks;

namespace OpticalReaderLib.Internal
{
    static class Utilities
    {
        /// <summary>
        /// Calculates optimal digital zoom from the given parameters.
        /// </summary>
        /// <param name="sensorSize">Size of the camera sensor in millimeters</param>
        /// <param name="sensorRotation">Camera sensor orientation to the screen</param>
        /// <param name="focalLength35">35 millimeter equivalent focal length of the camera lense</param>
        /// <param name="resolution">Camera sensor resolution in pixels</param>
        /// <param name="objectSize">Real-life object size in millimeters</param>
        /// <param name="distance">Camera sensor distance to the object in millimeters</param>
        /// <param name="objectResolution">Preferred object size on sensor in pixels</param>
        /// <returns>Digital zoom that makes the object in question appear so that it fits in objectResolution pixels on the sensor</returns>
        public static double CalculateZoom(Windows.Foundation.Size sensorSize, double sensorRotation, double focalLength35,
            Windows.Foundation.Size resolution, Windows.Foundation.Size objectSize, double distance, Windows.Foundation.Size objectResolution)
        {
            var sensorDiagonal = Math.Sqrt(Math.Pow(sensorSize.Width, 2) + Math.Pow(sensorSize.Height, 2));

            // http://en.wikipedia.org/wiki/35_mm_equivalent_focal_length
            // FocalLengthMm = DiagonalMm * FocalLength35Mm / 43.27
            var focalLength = sensorDiagonal * focalLength35 / 43.27;

            // http://photo.stackexchange.com/questions/12434/how-do-i-calculate-the-distance-of-an-object-in-a-photo
            // ObjectHeightOnSensorPx = (FocalLengthMm * ObjectRealHeightMm * ImageHeightPx) / (SensorHeightMm * ObjectDistanceMm)
            var objectWidthPixels = (focalLength * objectSize.Width * resolution.Width) / (sensorSize.Width * distance);

            var side = sensorRotation % 180 == 0 ? objectResolution.Height : objectResolution.Width;

            return side / objectWidthPixels;
        }

        public static ColorMode FrameFormatToColorMode(FrameFormat format)
        {
            if (format == FrameFormat.Bgra32)
            {
                return ColorMode.Bgra8888;
            }
            else if (format == FrameFormat.Gray8)
            {
                return ColorMode.Gray8;
            }

            throw new ArgumentException();
        }
    }
}
