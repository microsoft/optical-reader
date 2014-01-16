/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    /// <summary>
    /// Decoding result.
    /// </summary>
    public class DecodeResult
    {
        /// <summary>
        /// Textual representation of the result content.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Raw result data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Raw result data type.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Interest points in the decoded frame.
        /// </summary>
        public List<Windows.Foundation.Point> InterestPoints { get; set; }
    }

    /// <summary>
    /// Frame decoder implementation interface.
    /// 
    /// Decoders attempt to find optically encoded information (for example 1D and
    /// 2D barcodes, QR codes, data matrices) from frames.
    /// </summary>
    public interface IDecoder
    {
        /// <summary>
        /// Attempts to decode an optically encoded code from the frame.
        /// </summary>
        /// <param name="frame">Frame to decode.</param>
        /// <returns>Decoding result or null if no code was found.</returns>
        Task<DecodeResult> DecodeAsync(Frame frame);
    }
}
