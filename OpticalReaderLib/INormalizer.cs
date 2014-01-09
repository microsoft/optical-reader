using System;
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    public class NormalizeResult
    {
        public Frame Frame { get; set; }
        public Func<Windows.Foundation.Point, Windows.Foundation.Point> Translate { get; set; }
    }

    public interface INormalizer
    {
        Task<NormalizeResult> NormalizeAsync(Frame frame, Windows.Foundation.Rect area, double rotation);
    }
}
