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

namespace OpticalReaderLib
{
    /// <summary>
    /// Normalization result.
    /// </summary>
    public class NormalizeResult
    {
        /// <summary>
        /// Normalized frame.
        /// </summary>
        public Frame Frame { get; set; }

        /// <summary>
        /// Function to translate points from the normalized frame to the original frame.
        /// </summary>
        public Func<Windows.Foundation.Point, Windows.Foundation.Point> Translate { get; set; }
    }

    /// <summary>
    /// Frame normalizer implementation interface.
    /// 
    /// Normalizers handle cropping and rotating requested regions from larger frames.
    /// Normalizers are meant to be used before enhancers and decoders.
    /// 
    /// Normalizers may modify dimensions of frames, but if they do, they must provide
    /// a point translation function in the result so that possible interest points
    /// can be mapped back to the original frame.
    /// </summary>
    public interface INormalizer
    {
        /// <summary>
        /// Normalizes the frame, cropping it to the requested area and rotating the result.
        /// </summary>
        /// <param name="frame">Frame to normalize.</param>
        /// <param name="area">Frame area to crop to.</param>
        /// <param name="rotation">Degrees to clockwise rotate the selected area.</param>
        /// <returns>Normalization result.</returns>
        Task<NormalizeResult> NormalizeAsync(Frame frame, Windows.Foundation.Rect area, double rotation);
    }
}
