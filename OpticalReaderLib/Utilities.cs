using Nokia.Graphics.Imaging;
using System;
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    static class Utilities
    {
        public static double CalculateZoom(Windows.Foundation.Size sensorSize, double sensorRotation, double focalLength35, Windows.Foundation.Size resolution, Windows.Foundation.Size objectSize, double distance)
        {
            var sensorDiagonal = Math.Sqrt(Math.Pow(sensorSize.Width, 2) + Math.Pow(sensorSize.Height, 2));

            // http://en.wikipedia.org/wiki/35_mm_equivalent_focal_length
            // FocalLengthMm = DiagonalMm * FocalLength35Mm / 43.27
            var focalLength = sensorDiagonal * focalLength35 / 43.27;

            // http://photo.stackexchange.com/questions/12434/how-do-i-calculate-the-distance-of-an-object-in-a-photo
            // ObjectHeightOnSensorPx = (FocalLengthMm * ObjectRealHeightMm * ImageHeightPx) / (SensorHeightMm * ObjectDistanceMm)
            var objectHeightPixels = (focalLength * objectSize.Height * resolution.Width) / (sensorSize.Height * distance);

            //return Math.Max(resolution.Height / objectHeightPixels, 1);

            return resolution.Height / objectHeightPixels;
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

    class TimedExecution
    {
        private TimeSpan _executionTime = new TimeSpan(0);

        public TimeSpan ExecutionTime
        {
            get
            {
                var t = _executionTime;

                _executionTime = new TimeSpan(0);

                return t;
            }

            private set
            {
                _executionTime = value;
            }
        }

        public async Task<T> ExecuteAsync<T>(Windows.Foundation.IAsyncOperation<T> op)
        {
            var startTime = DateTime.Now;
            var result = await op;
            var endTime = DateTime.Now;

            ExecutionTime = endTime - startTime;

            return result;
        }
    }
}
