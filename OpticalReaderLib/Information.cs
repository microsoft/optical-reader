using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    public class Information
    {
        public static ParameterSuggestion GetSuggestedParameters(Windows.Foundation.Size resolution, double sensorRotation, Windows.Foundation.Size objectSize)
        {
            var deviceInformation = DeviceInformationCollector.GetInformation();

            if (deviceInformation != null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Detected {0}, returning device specific parameters", deviceInformation.Name));

                return new ParameterSuggestion()
                {
                    IsAccurate = true,
                    Zoom = Utilities.CalculateZoom(deviceInformation.SensorSize, sensorRotation, deviceInformation.FocalLength35Equivalent, resolution, objectSize, deviceInformation.MinimumFocusDistance),
                    Distance = deviceInformation.MinimumFocusDistance
                };
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Unknown device, returning default parameters");

                return new ParameterSuggestion()
                {
                    IsAccurate = false,
                    Zoom = Utilities.CalculateZoom(new Windows.Foundation.Size(3.2, 2.4), sensorRotation, 26, resolution, objectSize, 100),
                    Distance = 100
                };
            }
        }
    }
}
