/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

namespace OpticalReaderLib
{
    /// <summary>
    /// Frame data buffer format.
    /// </summary>
    public enum FrameFormat
    {
        /// <summary>
        /// Unknown format or format not set.
        /// </summary>
        Unknown,

        /// <summary>
        /// 32-bit blue-green-red-alpha format.
        /// </summary>
        Bgra32,

        /// <summary>
        /// 8-bit gray format.
        /// </summary>
        Gray8
    }

    /// <summary>
    /// Image frame.
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// Frame data buffer consisting the image data.
        /// </summary>
        public byte[] Buffer = null;

        /// <summary>
        /// Amount of bytes in buffer that represent one horizontal scan line in the frame.
        /// </summary>
        public uint Pitch = 0;

        /// <summary>
        /// Frame data buffer format.
        /// </summary>
        public FrameFormat Format = FrameFormat.Unknown;

        /// <summary>
        /// Frame pixel dimensions.
        /// </summary>
        public Windows.Foundation.Size Dimensions = new Windows.Foundation.Size(0, 0);
    }
}
