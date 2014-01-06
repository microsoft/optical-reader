using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    public interface IProcessor
    {
        Task<ProcessResult> ProcessAsync(Frame frame, double rotation, double zoom);
    }
}
