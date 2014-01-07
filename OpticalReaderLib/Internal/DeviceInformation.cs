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
