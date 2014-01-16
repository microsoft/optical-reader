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
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    /// <summary>
    /// Processing result.
    /// </summary>
    public class ProcessResult
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
        /// Interest points in the original frame.
        /// </summary>
        public List<Windows.Foundation.Point> InterestPoints { get; set; }
    }

    /// <summary>
    /// Frame processor implementation interface.
    /// 
    /// Processors are all-in-one frame decoding systems. Depending on the implementation,
    /// they may use normalizers, enhancers and decoders to help in the job.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Fired when there is a debugging frame available.
        /// 
        /// Debug frames are meant to be used while developing normalizers,
        /// enhancers and processor, in order to get visual feedback on how the frame
        /// is modified during processing.
        /// 
        /// Debug frames are not meant to be displayed in final consumer applications.
        /// </summary>
        event EventHandler<DebugFrameEventArgs> DebugFrameAvailable;

        /// <summary>
        /// Attempts to decode an optically encoded code from the frame by processing it.
        /// </summary>
        /// <param name="frame">Frame to process.</param>
        /// <param name="area">Interesting frame area.</param>
        /// <param name="rotation">Frame rotation, how many degrees it should be rotated clockwise.</param>
        /// <returns>Processing result or null if no code was found.</returns>
        Task<ProcessResult> ProcessAsync(Frame frame, Windows.Foundation.Rect area, double rotation);
    }

    /// <summary>
    /// Debug frame available event arguments.
    /// </summary>
    public class DebugFrameEventArgs : EventArgs 
    {
        /// <summary>
        /// Debug frame.
        /// </summary>
        public Frame DebugFrame { get; set; }
    }
}
