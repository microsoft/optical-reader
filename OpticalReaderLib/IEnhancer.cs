/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System.Threading.Tasks;

namespace OpticalReaderLib
{
    /// <summary>
    /// Enhancing result.
    /// </summary>
    public class EnhanceResult
    {
        /// <summary>
        /// Enhanced frame.
        /// </summary>
        public Frame Frame { get; set; }
    }

    /// <summary>
    /// Frame enhancer implementation interface.
    /// 
    /// Enhancers improve the image quality of frames, attempting to make it easier
    /// for decoding algorithms to do their work.
    /// 
    /// Enhancers are not allowed to modify frame dimensions.
    /// </summary>
    public interface IEnhancer
    {
        /// <summary>
        /// Enhances the frame for easier decoding.
        /// </summary>
        /// <param name="frame">Frame to enhance.</param>
        /// <returns>Enhancing result.</returns>
        Task<EnhanceResult> EnhanceAsync(Frame frame);
    }
}
