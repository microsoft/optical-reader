using System.Threading.Tasks;

namespace OpticalReaderLib
{
    public class ZxingProcessor : ProcessorBase
    {
        public ZxingProcessor() : base(new BasicNormalizer(), new BasicEnhancer(), new ZxingDecoder())
        {
        }
    }
}
