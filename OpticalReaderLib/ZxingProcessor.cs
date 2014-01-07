using System.Threading.Tasks;

namespace OpticalReaderLib
{
    public class ZxingProcessor : BasicProcessor
    {
        public ZxingProcessor() : base(new ZxingDecoder())
        {
            Normalizer = new BasicNormalizer();
            Enhancer = new BasicEnhancer();
        }
    }
}
