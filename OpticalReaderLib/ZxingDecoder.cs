using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    class ZxingDecoder : IDecoder
    {
        private ZXing.BarcodeReader _reader = new ZXing.BarcodeReader();

        public async Task<DecodeResult> DecodeAsync(Frame frame)
        {
            _reader.Options.TryHarder = true;

            var result = await Task.Run<ZXing.Result>(() =>
            {
                var width = 0;

                if (frame.Format == FrameFormat.Bgra32)
                {
                    width = (int)frame.Dimensions.Width;
                }
                else if (frame.Format == FrameFormat.Gray8)
                {
                    width = (int)frame.Pitch;
                }
                else
                {
                    throw new ArgumentException(String.Format("Incompatible frame format: {0}", frame.Format.ToString()));
                }

                return _reader.Decode(frame.Buffer, width, (int)frame.Dimensions.Height, FrameFormatToBitmapFormat(frame.Format));
            });

            if (result != null)
            {
                var decodeResult = new DecodeResult()
                {
                    Text = result.Text,
                    Data = result.RawBytes,
                    Format = result.BarcodeFormat.ToString()
                };

                if (result.ResultPoints != null && result.ResultPoints.Length > 0)
                {
                    decodeResult.InterestPoints = new List<Windows.Foundation.Point>();

                    foreach (ZXing.ResultPoint point in result.ResultPoints)
                    {
                        decodeResult.InterestPoints.Add(new Windows.Foundation.Point(point.X, point.Y));
                    }
                }

                return decodeResult;
            }
            else
            {
                return null;
            }
        }

        private static ZXing.RGBLuminanceSource.BitmapFormat FrameFormatToBitmapFormat(FrameFormat format)
        {
            if (format == FrameFormat.Bgra32)
            {
                return ZXing.RGBLuminanceSource.BitmapFormat.BGRA32;
            }
            else if (format == FrameFormat.Gray8)
            {
                return ZXing.RGBLuminanceSource.BitmapFormat.Gray8;
            }

            throw new ArgumentException();
        }
    }
}
