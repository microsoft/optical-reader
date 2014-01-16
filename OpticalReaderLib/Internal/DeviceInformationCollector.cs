/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using Microsoft.Phone.Info;

namespace OpticalReaderLib.Internal
{
    class DeviceInformationCollector
    {
        public static DeviceInformation GetInformation()
        {
            // http://www.dpreview.com/glossary/camera-system/sensor-sizes
            // https://github.com/ailon/PhoneNameResolver/blob/master/PhoneNameResolver.cs
            // http://en.wikipedia.org/wiki/Image_sensor_format

            if (DeviceStatus.DeviceManufacturer.Contains("NOKIA"))
            {
                var name = DeviceStatus.DeviceName;

                if (name.Contains("RM-913") || name.Contains("RM-914") || name.Contains("RM-915"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 520",
                        SensorSize = new Windows.Foundation.Size(3.2, 2.4),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 28
                    };
                }
                else if (name.Contains("RM-917"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 521",
                        SensorSize = new Windows.Foundation.Size(3.2, 2.4),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 28
                    };
                }
                else if (name.Contains("RM-998"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 525",
                        SensorSize = new Windows.Foundation.Size(3.2, 2.4),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 28
                    };
                }
                else if (name.Contains("RM-846"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 620",
                        SensorSize = new Windows.Foundation.Size(3.2, 2.4),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 28
                    };
                }
                else if (name.Contains("RM-941") || name.Contains("RM-942") || name.Contains("RM-943"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 625",
                        SensorSize = new Windows.Foundation.Size(3.2, 2.4),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 28
                    };
                }
                else if (name.Contains("RM-885") || name.Contains("RM-887"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 720",
                        SensorSize = new Windows.Foundation.Size(4, 3),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 26
                    };
                }
                else if (name.Contains("RM-824") || name.Contains("RM-825") || name.Contains("RM-825") || name.Contains("RM-826"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 820",
                        SensorSize = new Windows.Foundation.Size(4.536, 3.416),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 26
                    };
                }
                else if (name.Contains("RM-845"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 822",
                        SensorSize = new Windows.Foundation.Size(4.536, 3.416),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 26
                    };
                }
                else if (name.Contains("RM-820") || name.Contains("RM-821") || name.Contains("RM-822") || name.Contains("RM-867"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 920",
                        SensorSize = new Windows.Foundation.Size(4.536, 3.416),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 26
                    };
                }
                else if (name.Contains("RM-892") || name.Contains("RM-893") || name.Contains("RM-910") || name.Contains("RM-955"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 925",
                        SensorSize = new Windows.Foundation.Size(4.8, 3.6),
                        MinimumFocusDistance = 80,
                        FocalLength35Equivalent = 26
                    };
                }
                else if (name.Contains("RM-860"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 928",
                        SensorSize = new Windows.Foundation.Size(4.8, 3.6),
                        MinimumFocusDistance = 80,
                        FocalLength35Equivalent = 26
                    };
                }
                else if (name.Contains("RM-875") || name.Contains("RM-876") || name.Contains("RM-877"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 1020",
                        SensorSize = new Windows.Foundation.Size(8.8, 6.6),
                        MinimumFocusDistance = 150,
                        FocalLength35Equivalent = 26
                    };
                }
                else if (name.Contains("RM-994") || name.Contains("RM-995") || name.Contains("RM-996"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 1320",
                        SensorSize = new Windows.Foundation.Size(3.2, 2.4),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 28
                    };
                }
                else if (name.Contains("RM-937") || name.Contains("RM-938") || name.Contains("RM-939") || name.Contains("RM-940"))
                {
                    return new DeviceInformation()
                    {
                        Name = "Lumia 1520",
                        SensorSize = new Windows.Foundation.Size(5.76, 4.29),
                        MinimumFocusDistance = 100,
                        FocalLength35Equivalent = 26
                    };
                }
            }

            return null;
        }
    }
}
