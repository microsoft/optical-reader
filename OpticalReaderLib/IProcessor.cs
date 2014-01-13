using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    public class ProcessResult
    {
        public string Text = null;
        public byte[] Data = null;
        public string Format = null;
        public List<Windows.Foundation.Point> InterestPoints = null;
    }

    public interface IProcessor
    {
        event EventHandler<DebugFrameEventArgs> DebugFrameAvailable;

        Task<ProcessResult> ProcessAsync(Frame frame, Windows.Foundation.Rect area, double rotation);
    }

    public class DebugFrameEventArgs : EventArgs 
    {
        public Frame DebugFrame = null;
    }
}
