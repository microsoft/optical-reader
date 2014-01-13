using System.Threading.Tasks;

namespace OpticalReaderApp
{
    class CustomEnhancer : OpticalReaderLib.IEnhancer
    {
        private uint _threshold = 0;
        private uint _jump = 16;
        private uint _min = 48;
        private uint _max = 160;

        public async Task<OpticalReaderLib.EnhanceResult> EnhanceAsync(OpticalReaderLib.Frame frame)
        {
            _threshold = _threshold + _jump;

            if (_threshold > _max)
            {
                _threshold = _min;
            }

            var f = await Task.Run<OpticalReaderLib.Frame>(() =>
            {
                for (int i = 0; i < frame.Buffer.Length; i++)
                {
                    frame.Buffer[i] = (byte)(frame.Buffer[i] < _threshold ? 0x00 : 0xff);
                }

                return frame;
            });

            return new OpticalReaderLib.EnhanceResult()
            {
                Frame = f
            };
        }
    }
}
