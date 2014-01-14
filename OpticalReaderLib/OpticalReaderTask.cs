using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

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

    public class OpticalReaderTask : Microsoft.Phone.Tasks.ChooserBase<OpticalReaderResult>, IDisposable
    {
        private static OpticalReaderTask _instance = null;
        private IProcessor _processor = null;

        public IProcessor Processor
        {
            get
            {
                if (_processor == null)
                {
                    _processor = new ZxingProcessor();
                }

                return _processor;
            }

            set
            {
                _processor = value;
            }
        }

        public Windows.Foundation.Size ObjectSize { get; set; }

        public TimeSpan FocusInterval { get; set; }

        public bool ShowDebugInformation { get; set; }

        public bool RequireConfirmation { get; set; }

        public OpticalReaderTask()
        {
            if (_instance == null)
            {
                _instance = this;

                FocusInterval = new TimeSpan(0, 0, 0, 0, 2500);
            }
            else
            {
                throw new Exception("Only one instance of OpticalReaderTask is supported at a time and another instance already exists.");
            }
        }

        public override void Show()
        {
            var applicationFrame = (PhoneApplicationFrame)Application.Current.RootVisual;

            applicationFrame.Navigate(new Uri("/OpticalReaderLib;component/OpticalReaderPage.xaml", UriKind.Relative));
        }

        public void Dispose()
        {
            _instance = null;

            Processor = null;
        }

        #region Internal methods

        internal static OpticalReaderTask Instance
        {
            get
            {
                return _instance;
            }

            set
            {
                _instance = value;
            }
        }

        internal static bool TaskPending
        {
            get
            {
                return true;
            }
        }

        internal static void CompleteTask(ProcessResult processResult, WriteableBitmap thumbnail)
        {
            if (_instance != null)
            {
                var instance = _instance;

                var result = new OpticalReaderResult(TaskResult.OK, processResult, thumbnail);

                instance.FireCompleted(instance, result, null);
            }
        }

        internal static void CancelTask(bool navigatedBack)
        {
            if (_instance != null)
            {
                var instance = _instance;

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

        #endregion Internal methods
    }
}
