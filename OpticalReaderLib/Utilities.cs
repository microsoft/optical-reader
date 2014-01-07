using System;

namespace OpticalReaderLib
{
    public class Utilities
    {
        /// <summary>
        /// Gets suggested camera parameters from the given information.
        /// </summary>
        /// <param name="sensorResolution">Camera sensor resolution in pixels</param>
        /// <param name="sensorRotation">Camera sensor orientation to the screen</param>
        /// <param name="objectSize">Real-life object size in millimeters</param>
        /// <param name="length">Preferred object width or height in pixels</param>
        /// <returns>Suggested camera parameters</returns>
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
    }
}
