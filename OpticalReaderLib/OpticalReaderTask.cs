using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace OpticalReaderLib
{
    public class OpticalReaderResult : TaskEventArgs
    {
        public string Text { get; internal set; }
        public byte[] Data { get; internal set; }
        public string Format { get; internal set; }
        public WriteableBitmap Thumbnail { get; internal set; }

        public OpticalReaderResult(TaskResult taskResult)
            : base(taskResult)
        {
        }

        public OpticalReaderResult(TaskResult taskResult, ProcessResult processResult, WriteableBitmap thumbnail)
            : base(taskResult)
        {
            if (processResult != null)
            {
                Text = processResult.Text;
                Data = processResult.Data;
                Format = processResult.Format;
            }

            Thumbnail = thumbnail;
        }
    }

    public class OpticalReaderTask : Microsoft.Phone.Tasks.ChooserBase<OpticalReaderResult>
    {
        private static OpticalReaderTask _instance = null;

        public static bool TaskPending
        {
            get
            {
                return _instance != null;
            }
        }

        public static Windows.Foundation.Size ObjectSize { get; set; }

        public static void CompleteTask(ProcessResult processResult, WriteableBitmap thumbnail)
        {
            if (_instance != null)
            {
                var instance = _instance;

                _instance = null;

                var result = new OpticalReaderResult(TaskResult.OK, processResult, thumbnail);

                instance.FireCompleted(instance, result, null);
            }
        }

        public static void CancelTask(bool navigatedBack)
        {
            if (_instance != null)
            {
                var instance = _instance;

                _instance = null;

                if (navigatedBack)
                {
                    instance.FireCompleted(instance, new OpticalReaderResult(TaskResult.Cancel), null);
                }
                else
                {
                    instance.FireCompleted(instance, new OpticalReaderResult(TaskResult.None), null);
                }
            }
        }

        public override void Show()
        {
            _instance = this;

            var applicationFrame = (PhoneApplicationFrame)Application.Current.RootVisual;

            applicationFrame.Navigate(new Uri("/OpticalReaderLib;component/OpticalReaderPage.xaml", UriKind.Relative));
        }
    }
}
