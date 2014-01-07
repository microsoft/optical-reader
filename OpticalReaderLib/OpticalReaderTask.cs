using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace OpticalReaderLib
{
    public class OpticalReaderResult : TaskEventArgs
    {
        public string Text { get; private set; }
        public byte[] Data { get; private set; }
        public string Format { get; private set; }

        public OpticalReaderResult(ProcessResult processResult)
            : base(processResult != null ? TaskResult.OK : TaskResult.Cancel)
        {
            if (processResult != null)
            {
                Text = processResult.Text;
                Data = processResult.Data;
                Format = processResult.Format;
            }
        }
    }

    public class OpticalReaderTask : Microsoft.Phone.Tasks.ChooserBase<OpticalReaderResult>
    {
        private static OpticalReaderTask _instance = null;
        private static PhoneApplicationFrame _applicationFrame = null;

        public static void CompleteTask(ProcessResult processResult)
        {
            if (_instance != null)
            {
                var instance = _instance;

                _instance = null;

                _applicationFrame.GoBack();

                var result = new OpticalReaderResult(processResult);

                instance.FireCompleted(instance, result, null);
            }
        }

        public static void CancelTask(bool navigatedBack)
        {
            if (_instance != null)
            {
                var instance = _instance;

                _instance = null;

                if (!navigatedBack)
                {
                    _applicationFrame.GoBack();
                }

                instance.FireCompleted(instance, new OpticalReaderResult(null), null);
            }
        }

        public override void Show()
        {
            _instance = this;
            _applicationFrame = (PhoneApplicationFrame)Application.Current.RootVisual;

            _applicationFrame.Navigate(new Uri("/OpticalReaderLib;component/OpticalReaderPage.xaml", UriKind.Relative));
        }
    }
}
