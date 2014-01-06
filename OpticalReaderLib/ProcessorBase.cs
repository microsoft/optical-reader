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

    public abstract class ProcessorBase : IProcessor
    {
        private INormalizer _normalizer = null;
        private IEnhancer _enhancer = null;
        private IDecoder _decoder = null;

        public ProcessorBase(INormalizer normalizer, IEnhancer enhancer, IDecoder decoder)
        {
            _normalizer = normalizer;
            _enhancer = enhancer;
            _decoder = decoder;
        }

        public virtual async Task<ProcessResult> ProcessAsync(Frame frame, double rotation, double zoom)
        {
            var timedExecution = new TimedExecution();
            var normalizeTime = new TimeSpan(0);
            var enhanceTime = new TimeSpan(0);
            var decodeTime = new TimeSpan(0);

            NormalizeResult normalizeResult = null;

            //var result = await timedExecution.ExecuteAsync<DecodeResult>(_processor.DecodeAsync(frame).AsAsyncOperation());
            //var decodingTime = timedExecution.ExecutionTime;

            //System.Diagnostics.Debug.WriteLine(String.Format("Normalizing took {0}, enhancement took {1} ms, decoding took {2} ms, this makes total time of {3} ms",
            //    (int)(normalizingTime).TotalMilliseconds, (int)(enhancementTime).TotalMilliseconds, (int)(decodingTime).TotalMilliseconds,
            //    (int)(normalizingTime + enhancementTime + decodingTime).TotalMilliseconds));


            if (_normalizer != null)
            {
                normalizeResult = await timedExecution.ExecuteAsync<NormalizeResult>(
                    _normalizer.NormalizeAsync(frame, rotation, zoom).AsAsyncOperation());

                normalizeTime = timedExecution.ExecutionTime;
                frame = normalizeResult.Frame;
            }

            if (_enhancer != null)
            {
                var enhanceResult = await timedExecution.ExecuteAsync<EnhanceResult>(
                    _enhancer.EnhanceAsync(frame).AsAsyncOperation());

                enhanceTime = timedExecution.ExecutionTime;
                frame = enhanceResult.Frame;
            }

            var decodeResult = await timedExecution.ExecuteAsync<DecodeResult>(
                _decoder.DecodeAsync(frame).AsAsyncOperation());

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

                if (normalizeResult != null && normalizeResult.Translate != null)
                {
                    var translatedInterestPoints = new List<Windows.Foundation.Point>();

                    foreach (var point in interestPoints)
                    {
                        translatedInterestPoints.Add(normalizeResult.Translate(point));
                    }

                    interestPoints = translatedInterestPoints;
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

        public virtual async Task<WriteableBitmap> RenderPreviewAsync(Frame frame, Windows.Foundation.Size size)
        {
            using (var bitmap = new Bitmap(frame.Dimensions, Utilities.FrameFormatToColorMode(frame.Format), frame.Pitch, frame.Buffer.AsBuffer()))
            using (var source = new BitmapImageSource(bitmap))
            using (var renderer = new WriteableBitmapRenderer(source, new WriteableBitmap((int)size.Width, (int)size.Height), OutputOption.Stretch))
            {
                return await renderer.RenderAsync();
            }
        }
    }
}
