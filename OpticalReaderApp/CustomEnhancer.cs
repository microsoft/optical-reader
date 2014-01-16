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
using System.Threading.Tasks;

namespace OpticalReaderApp
{
    /// <summary>
    /// Dumb little custom enhancer that turns all frame pixels to either white or black
    /// depending on wether the grayscale value of the pixel exceeds a threshold or not,
    /// rotating the threshold for adjacent frames.
    /// </summary>
    class CustomEnhancer : OpticalReaderLib.IEnhancer
    {
        private uint _threshold = 0;
        private uint _jump = 16;
        private uint _min = 48;
        private uint _max = 160;

        public async Task<OpticalReaderLib.EnhanceResult> EnhanceAsync(OpticalReaderLib.Frame frame)
        {
            if (frame.Format == OpticalReaderLib.FrameFormat.Gray8)
            {
                _threshold = _threshold + _jump;

                if (_threshold > _max)
                {
                    _threshold = _min;
                }

                var f = await Task.Run<OpticalReaderLib.Frame>(() =>
                {
                    for (int i = 0; i < frame.Buffer.Length; i++)
                    {
                        frame.Buffer[i] = (byte)(frame.Buffer[i] < _threshold ? 0x00 : 0xff);
                    }

                    return frame;
                });

                return new OpticalReaderLib.EnhanceResult()
                {
                    Frame = f
                };
            }
            else
            {
                throw new Exception("Dumb little custom enhancer only supports Gray8 encoded frames");
            }
        }
    }
}
