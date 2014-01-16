/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticalReaderLib.Internal
{
    class DeviceInformation
    {
        public string Name = null;
        public Windows.Foundation.Size SensorSize = new Windows.Foundation.Size(0, 0);
        public double MinimumFocusDistance = 0;
        public double FocalLength35Equivalent = 0;
    }
}
