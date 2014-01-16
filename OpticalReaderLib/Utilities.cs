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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OpticalReaderLib
{
    public class Utilities
    {
        /// <summary>
        /// Gets suggested camera parameters from the given information.
        /// </summary>
        /// <param name="sensorResolution">Camera sensor resolution in pixels.</param>
        /// <param name="sensorRotation">Camera sensor orientation to the screen.</param>
        /// <param name="objectSize">Real-life object size in millimeters.</param>
        /// <param name="length">Preferred object width or height in pixels.</param>
        /// <returns>Suggested camera parameters.</returns>
        public static ParameterSuggestion GetSuggestedParameters(Windows.Foundation.Size sensorResolution, double sensorRotation, Windows.Foundation.Size objectSize, Windows.Foundation.Size objectResolution)
        {
            var deviceInformation = Internal.DeviceInformationCollector.GetInformation();

            if (deviceInformation != null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Detected {0}, returning device specific parameters", deviceInformation.Name));

                return new ParameterSuggestion()
                {
                    IsAccurate = true,
                    Zoom = Internal.Utilities.CalculateZoom(deviceInformation.SensorSize, sensorRotation, deviceInformation.FocalLength35Equivalent, sensorResolution, objectSize, deviceInformation.MinimumFocusDistance, objectResolution),
                    Distance = deviceInformation.MinimumFocusDistance
                };
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Unknown device, returning default parameters");

                return new ParameterSuggestion()
                {
                    IsAccurate = false,
                    Zoom = Internal.Utilities.CalculateZoom(new Windows.Foundation.Size(3.2, 2.4), sensorRotation, 26, sensorResolution, objectSize, 100, objectResolution),
                    Distance = 100
                };
            }
        }

        /// <summary>
        /// Renders a writeable bitmap preview of the given frame.
        /// </summary>
        /// <param name="frame">Frame to render.</param>
        /// <param name="size">Preview size in pixels.</param>
        /// <returns>Rendered frame preview.</returns>
        public static async Task<WriteableBitmap> RenderPreviewAsync(Frame frame, Windows.Foundation.Size size)
        {
            using (var bitmap = new Bitmap(frame.Dimensions, Internal.Utilities.FrameFormatToColorMode(frame.Format), frame.Pitch, frame.Buffer.AsBuffer()))
            using (var source = new BitmapImageSource(bitmap))
            using (var renderer = new WriteableBitmapRenderer(source, new WriteableBitmap((int)size.Width, (int)size.Height), OutputOption.Stretch))
            {
                return await renderer.RenderAsync();
            }
        }
    }
}
