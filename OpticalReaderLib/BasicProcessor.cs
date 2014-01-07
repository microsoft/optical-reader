using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OpticalReaderLib
{
    public class ProcessResult
    {
        public string Text = null;
        public byte[] Data = null;
        public string Format = null;
        public List<Windows.Foundation.Point> InterestPoints = null;
    }

    public abstract class BasicProcessor : IProcessor
    {
        public INormalizer Normalizer { get; set; }
        public IEnhancer Enhancer { get; set; }
        public IDecoder Decoder { get; private set; }

        public BasicProcessor(IDecoder decoder)
        {
            Decoder = decoder;
        }

        public virtual async Task<ProcessResult> ProcessAsync(Frame frame, double rotation, double zoom)
        {
            var timedExecution = new Internal.TimedExecution();
            var normalizeTime = new TimeSpan(0);
            var enhanceTime = new TimeSpan(0);
            var decodeTime = new TimeSpan(0);

            NormalizeResult normalizeResult = null;

            if (Normalizer != null)
            {
                normalizeResult = await timedExecution.ExecuteAsync<NormalizeResult>(
                    Normalizer.NormalizeAsync(frame, rotation, zoom).AsAsyncOperation());

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

            if (decodeResult != null)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format(
                    "Normalizing took {0} ms, enhancing took {1} ms, decoding took {2}, which totals to {3} ms",
                    (int)normalizeTime.TotalMilliseconds, (int)enhanceTime.TotalMilliseconds, (int)decodeTime.TotalMilliseconds,
                    (int)(normalizeTime + enhanceTime + decodeTime).TotalMilliseconds));
#endif // DEBUG

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
