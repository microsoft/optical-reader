using System.Threading.Tasks;

namespace OpticalReaderLib
{
    public class EnhanceResult
    {
        public Frame Frame { get; set; }
    }

    public interface IEnhancer
    {
        Task<EnhanceResult> EnhanceAsync(Frame frame);
    }
}
