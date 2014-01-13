using System.Threading.Tasks;

namespace OpticalReaderApp
{
    public class CustomProcessor : OpticalReaderLib.BasicProcessor
    {
        public CustomProcessor() : base(new OpticalReaderLib.ZxingDecoder())
        {
            Normalizer = new OpticalReaderLib.BasicNormalizer();
            Enhancer = new CustomEnhancer();
        }
    }
}
