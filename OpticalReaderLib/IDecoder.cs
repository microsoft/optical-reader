using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    public class DecodeResult
    {
        public string Text = null;
        public byte[] Data = null;
        public string Format = null;
        public List<Windows.Foundation.Point> InterestPoints = null;
    }

    public interface IDecoder
    {
        Task<DecodeResult> DecodeAsync(Frame frame);
    }
}
