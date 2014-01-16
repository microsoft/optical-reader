/**
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace OpticalReaderApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        private OpticalReaderLib.IProcessor _processor = null;
        private OpticalReaderLib.OpticalReaderTask _task = null;
        private OpticalReaderLib.OpticalReaderResult _taskResult = null;
        private int _size = 3;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetSizeTextBlockText(_size);

            if (_taskResult != null)
            {
                if (_taskResult.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
                {
                    System.Diagnostics.Debug.WriteLine("Code read successfully");

                    var byteCount = _taskResult.Data != null ? _taskResult.Data.Length : 0;

                    TypeTextBlock.Text = String.Format("{0} ({1} bytes)", _taskResult.Format, byteCount);
                    DescriptionTextBlock.Text = _taskResult.Text;
                    ThumbnailImage.Source = _taskResult.Thumbnail;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Code reading aborted");
                }

                _taskResult = null;
            }

            ResultStackPanel.Visibility = ThumbnailImage.Source != null ? Visibility.Visible : Visibility.Collapsed;
            GuideStackPanel.Visibility = ThumbnailImage.Source == null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ReadCodeButton_Click(object sender, EventArgs e)
        {
            TypeTextBlock.Text = "";
            DescriptionTextBlock.Text = "";
            ThumbnailImage.Source = null;

            if (_task != null)
            {
                _task.Completed -= OpticalReaderTask_Completed;
                _task.Dispose();
                _task = null;
            }

            var showDebugInformation = DebugCheckBox.IsChecked != null && (bool)DebugCheckBox.IsChecked;
            var useCustomProcessor = ProcessorCheckBox.IsChecked != null && (bool)ProcessorCheckBox.IsChecked;
            var focusInterval = new TimeSpan(0, 0, 0, 0, 2500);
            var objectSize = _size <= 10 ? new Windows.Foundation.Size(_size * 10, _size * 10) : new Windows.Foundation.Size(0, 0);
            var requireConfirmation = ConfirmationCheckBox.IsChecked != null && (bool)ConfirmationCheckBox.IsChecked;

            if (useCustomProcessor)
            {
                _processor = new CustomProcessor();

                focusInterval = new TimeSpan(0, 0, 0, 0, 4000);
            }
            else
            {
                _processor = new OpticalReaderLib.ZxingProcessor();
            }

            _task = new OpticalReaderLib.OpticalReaderTask()
            {
                Processor = _processor,
                ShowDebugInformation = showDebugInformation,
                FocusInterval = focusInterval,
                ObjectSize = objectSize,
                RequireConfirmation = requireConfirmation
            };

            _task.Completed += OpticalReaderTask_Completed;
            _task.Show();
        }

        private void OpticalReaderTask_Completed(object sender, OpticalReaderLib.OpticalReaderResult e)
        {
            _taskResult = e;
        }

        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _size = (int)e.NewValue;

            SetSizeTextBlockText(_size);
        }

        private void SetSizeTextBlockText(int size)
        {
            if (SizeTextBlock != null)
            {
                if (_size <= 10)
                {
                    SizeTextBlock.Text = String.Format("{0}x{0} cm", _size);
                }
                else
                {
                    SizeTextBlock.Text = "Default";
                }
            }
        }
    }
}