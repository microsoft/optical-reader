using System.Threading.Tasks;

namespace OpticalReaderLib
{
    /// <summary>
    /// Processor implementation that utilizes the ZXing decoder.
    /// </summary>
    public class ZxingProcessor : BasicProcessor
    {
        public ZxingProcessor() : base(new ZxingDecoder())
        {
            Normalizer = new BasicNormalizer();
            Enhancer = new BasicEnhancer();
        }
    }
}
