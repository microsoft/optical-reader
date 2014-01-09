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
        private OpticalReaderLib.OpticalReaderTask _task = new OpticalReaderLib.OpticalReaderTask();
        private OpticalReaderLib.OpticalReaderResult _taskResult = null;
        private int _size = 3;

        public MainPage()
        {
            InitializeComponent();

            _task.Completed += OpticalReaderTask_Completed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SizeTextBlock.Text = String.Format("{0}x{0} cm", _size);

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

            OpticalReaderLib.OpticalReaderTask.ObjectSize = new Windows.Foundation.Size(_size * 10, _size *10);

            _task.Show();
        }

        private void OpticalReaderTask_Completed(object sender, OpticalReaderLib.OpticalReaderResult e)
        {
            _taskResult = e;
        }

        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _size = (int)e.NewValue;

            if (SizeTextBlock != null)
            {
                SizeTextBlock.Text = String.Format("{0}x{0} cm", _size);
            }
        }
    }
}