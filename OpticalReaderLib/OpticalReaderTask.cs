/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OpticalReaderLib
{
    /// <summary>
    /// Optical reader task result event arguments.
    /// </summary>
    public class OpticalReaderResult : TaskEventArgs
    {
        /// <summary>
        /// Textual representation of the result content.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Raw result data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Raw result data type.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Thumbnail preview of the detected optical code.
        /// </summary>
        public WriteableBitmap Thumbnail { get; set; }

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

    /// <summary>
    /// Optical reader task is a Windows Phone chooser task implementation that allows
    /// easy and quick integration of 1D and 2D optical code reading functionality.
    /// </summary>
    public class OpticalReaderTask : Microsoft.Phone.Tasks.ChooserBase<OpticalReaderResult>, IDisposable
    {
        private static OpticalReaderTask _instance = null;
        private IProcessor _processor = null;

        /// <summary>
        /// Processor to use for processing the frames.
        /// 
        /// Zxing processor is used if no processor is set explicitly.
        /// </summary>
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

        /// <summary>
        /// Target object real-life millimeter size. This affects the zoom factor
        /// used in the reader viewfinder.
        /// 
        /// Default is no zoom.
        /// </summary>
        public Windows.Foundation.Size ObjectSize { get; set; }

        /// <summary>
        /// Reader camera focus interval, meaning the time that needs to pass without
        /// the reader finding anything before it attempts to re-focus the lense.
        /// 
        /// Default is 2500 milliseconds.
        /// </summary>
        public TimeSpan FocusInterval { get; set; }

        /// <summary>
        /// Set to true to see debug frames in the reader viewfinder. Default is false,
        /// meaning that debug frames are not displayed.
        /// 
        /// Debug frames are meant to be used while developing normalizers,
        /// enhancers and processor, in order to get visual feedback on how the frame
        /// is modified during processing.
        /// 
        /// Debug frames are not meant to be displayed in final consumer applications.
        /// </summary>
        public bool ShowDebugInformation { get; set; }

        /// <summary>
        /// Set to true to require the user to confirm a found result by tapping on a
        /// result preview. If false, the first result found will be used automatically.
        /// 
        /// Default is false.
        /// </summary>
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

        /// <summary>
        /// Show the optical reader viewfinder.
        /// 
        /// Application is navigated to a optical reader viewfinder page and the Completed
        /// event is fired when user either navigates away from the viewfinder or if an
        /// optical code is detected.
        /// </summary>
        public override void Show()
        {
            var applicationFrame = (PhoneApplicationFrame)Application.Current.RootVisual;

            applicationFrame.Navigate(new Uri("/OpticalReaderLib;component/Internal/OpticalReaderPage.xaml", UriKind.Relative));
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
