namespace OpticalReaderLib
{
    public enum FrameFormat
    {
        Unknown,
        Bgra32,
        Gray8
    }

    public class Frame
    {
        public byte[] Buffer = null;
        public uint Pitch = 0;
        public FrameFormat Format = FrameFormat.Unknown;
        public Windows.Foundation.Size Dimensions = new Windows.Foundation.Size(0, 0);
    }
}
