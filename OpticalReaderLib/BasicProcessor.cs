/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OpticalReaderLib
{
    /// <summary>
    /// Basic generic abstract processor implementation. Requires a decoder.
    /// </summary>
    public abstract class BasicProcessor : IProcessor
    {
        public INormalizer Normalizer { get; set; }
        public IEnhancer Enhancer { get; set; }
        public IDecoder Decoder { get; private set; }
        public event EventHandler<DebugFrameEventArgs> DebugFrameAvailable;

        public BasicProcessor(IDecoder decoder)
        {
            Decoder = decoder;
        }

        public virtual async Task<ProcessResult> ProcessAsync(Frame frame, Windows.Foundation.Rect area, double rotation)
        {
            var timedExecution = new Internal.TimedExecution();
            var normalizeTime = new TimeSpan(0);
            var enhanceTime = new TimeSpan(0);
            var decodeTime = new TimeSpan(0);

            NormalizeResult normalizeResult = null;

            if (Normalizer != null)
            {
                normalizeResult = await timedExecution.ExecuteAsync<NormalizeResult>(
                    Normalizer.NormalizeAsync(frame, area, rotation).AsAsyncOperation());

                normalizeTime = timedExecution.ExecutionTime;
                frame = normalizeResult.Frame;
            }

            if (Enhancer != null)
            {
                var enhanceResult = await timedExecution.ExecuteAsync<EnhanceResult>(
                    Enhancer.EnhanceAsync(frame).AsAsyncOperation());

                enhanceTime = timedExecution.ExecutionTime;
                frame = enhanceResult.Frame;
            }

            var decodeResult = await timedExecution.ExecuteAsync<DecodeResult>(
                Decoder.DecodeAsync(frame).AsAsyncOperation());

            decodeTime = timedExecution.ExecutionTime;

#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format(
                "Normalizing took {0} ms, enhancing took {1} ms, decoding took {2}, which totals to {3} ms",
                (int)normalizeTime.TotalMilliseconds, (int)enhanceTime.TotalMilliseconds, (int)decodeTime.TotalMilliseconds,
                (int)(normalizeTime + enhanceTime + decodeTime).TotalMilliseconds));
#endif // DEBUG

            if (DebugFrameAvailable != null)
            {
                DebugFrameAvailable(this, new DebugFrameEventArgs() { DebugFrame = frame });
            }

            if (decodeResult != null)
            {
                var interestPoints = decodeResult.InterestPoints;

                if (interestPoints != null)
                {
                    if (normalizeResult != null && normalizeResult.Translate != null)
                    {
                        var translatedInterestPoints = new List<Windows.Foundation.Point>();

                        foreach (var point in interestPoints)
                        {
                            translatedInterestPoints.Add(normalizeResult.Translate(point));
                        }

                        interestPoints = translatedInterestPoints;
                    }
                }

                return new ProcessResult()
                {
                    Data = decodeResult.Data,
                    Format = decodeResult.Format,
                    Text = decodeResult.Text,
                    InterestPoints = interestPoints
                };
            }
            else
            {
                return null;
            }
        }
    }
}
