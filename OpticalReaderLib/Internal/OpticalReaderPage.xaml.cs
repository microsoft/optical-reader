/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using Microsoft.Devices;
using Microsoft.Phone.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Windows.Phone.Media.Capture;

namespace OpticalReaderLib.Internal
{
    public partial class OpticalReaderPage : PhoneApplicationPage
    {
        private double _zoom = 0;
        private double _rotation = 0;
        private bool _processing = false;
        private DateTime _lastSuccess = DateTime.Now;
        private PhotoCaptureDevice _device = null;
        private Tuple<ProcessResult, WriteableBitmap> _resultTuple = null;
        private bool _active = false;
        private DispatcherTimer _timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };

        public OpticalReaderPage()
        {
            InitializeComponent();

            _timer.Tick += NavigationTimer_Tick;
        }

        private void NavigationTimer_Tick(object sender, EventArgs e)
        {
            if (!_processing && !_active && NavigationService.CanGoBack)
            {
                _timer.Stop();

                ProgressBar.IsIndeterminate = false;
                ProgressBar.Visibility = System.Windows.Visibility.Collapsed;

                NavigationService.GoBack();
            }
        }

        ~OpticalReaderPage()
        {
            if (_device != null)
            {
                UninitializeCamera();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode != NavigationMode.Back && OpticalReaderTask.Instance != null)
            {
                if (_device == null)
                {
                    InitializeCamera();

                    AdaptToOrientation();

                    _device.PreviewFrameAvailable += PhotoCaptureDevice_PreviewFrameAvailable;
                }

                if (OpticalReaderTask.Instance.ShowDebugInformation)
                {
                    OpticalReaderTask.Instance.Processor.DebugFrameAvailable += Processor_DebugFrameAvailable;
                    DebugGrid.Visibility = System.Windows.Visibility.Visible;
                }

                ReaderBorder.Visibility = System.Windows.Visibility.Visible;

                _active = true;
            }
            else
            {
                ReaderBorder.Visibility = System.Windows.Visibility.Collapsed;

                NavigationService.GoBack();
            }
        }

        void Processor_DebugFrameAvailable(object sender, DebugFrameEventArgs e)
        {
            Dispatcher.BeginInvoke(async () =>
                {
                    DebugImage.Source = await OpticalReaderLib.Utilities.RenderPreviewAsync(e.DebugFrame, e.DebugFrame.Dimensions);
                });
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_processing && e.IsCancelable)
            {
                e.Cancel = true;

                if (!_timer.IsEnabled)
                {
                    _timer.Start();
                    
                    ProgressBar.IsIndeterminate = true;
                    ProgressBar.Visibility = System.Windows.Visibility.Visible;
                }
            }

            _active = false;

            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (_device != null)
            {
                if (OpticalReaderTask.Instance.ShowDebugInformation)
                {
                    OpticalReaderTask.Instance.Processor.DebugFrameAvailable -= Processor_DebugFrameAvailable;
                    DebugGrid.Visibility = System.Windows.Visibility.Collapsed;
                }

                _device.PreviewFrameAvailable -= PhotoCaptureDevice_PreviewFrameAvailable;

                UninitializeCamera();
            }

            if (OpticalReaderTask.TaskPending)
            {
                if (e.NavigationMode == NavigationMode.Back)
                {
                    if (_resultTuple != null)
                    {
                        OpticalReaderTask.CompleteTask(_resultTuple.Item1, _resultTuple.Item2);
                    }
                    else
                    {
                        OpticalReaderTask.CancelTask(true);
                    }
                }
                else
                {
                    OpticalReaderTask.CancelTask(false);
                }
            }

            ResultTextBlock.Text = "";
            ResultImage.Source = null;
            ResultGrid.Visibility = System.Windows.Visibility.Collapsed;

            _resultTuple = null;
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            AdaptToOrientation();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            _resultTuple = null;
            
            base.OnBackKeyPress(e);
        }

        private void InitializeCamera()
        {
            var captureResolutions = PhotoCaptureDevice.GetAvailableCaptureResolutions(CameraSensorLocation.Back).ToArray();
            var previewResolutions = PhotoCaptureDevice.GetAvailablePreviewResolutions(CameraSensorLocation.Back).ToArray();

            Windows.Foundation.Size captureResolution = new Windows.Foundation.Size(640, 480);
            Windows.Foundation.Size previewResolution = new Windows.Foundation.Size(640, 480);

            try
            {
                captureResolution = GetFirstWideResolution(captureResolutions);
                previewResolution = GetFirstWideResolution(previewResolutions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to get wide resolution for viewfinder, using 640x480");
            }

            var task = PhotoCaptureDevice.OpenAsync(CameraSensorLocation.Back, captureResolution).AsTask();

            task.Wait();

            _device = task.Result;
            _device.SetPreviewResolutionAsync(previewResolution).AsTask().Wait();

            var objectResolutionSide = _device.PreviewResolution.Height * (ReaderBorder.Height - 2 * ReaderBorder.Margin.Top) / 480;
            var objectResolution = new Windows.Foundation.Size(objectResolutionSide, objectResolutionSide);
            var focusRegionSize = new Windows.Foundation.Size(objectResolutionSide, objectResolutionSide);
            var objectSize = OpticalReaderLib.OpticalReaderTask.Instance.ObjectSize;

            if (objectSize.Width * objectSize.Height > 0)
            {
                var parameters = OpticalReaderLib.Utilities.GetSuggestedParameters(_device.PreviewResolution, _device.SensorRotationInDegrees, objectSize, objectResolution);

                _zoom = Math.Max(parameters.Zoom, 1.0);
            }
            else
            {
                _zoom = 1.0;
            }

            var centerPoint = new Windows.Foundation.Point(previewResolution.Width / 2, previewResolution.Height / 2);

            _device.FocusRegion = new Windows.Foundation.Rect(
                centerPoint.X - focusRegionSize.Width / 2, centerPoint.Y - focusRegionSize.Height / 2,
                focusRegionSize.Width, focusRegionSize.Height);

            ViewfinderVideoBrush.SetSource(_device);
        }

        private void UninitializeCamera()
        {
            if (_device != null)
            {
                _device.PreviewFrameAvailable -= PhotoCaptureDevice_PreviewFrameAvailable;

                _device.Dispose();
                _device = null;
            }
        }

        private void AdaptToOrientation()
        {
            if (System.Windows.Application.Current.Host.Content.ScaleFactor == 100)
            {
                // WVGA
                Canvas.Width = 800;
            }
            else if (System.Windows.Application.Current.Host.Content.ScaleFactor == 160)
            {
                // WXGA
                Canvas.Width = 800;
            }
            else if (System.Windows.Application.Current.Host.Content.ScaleFactor == 150)
            {
                // 720p
                Canvas.Width = 853;
            }

            Canvas.Height = _device.PreviewResolution.Height * Canvas.Width / _device.PreviewResolution.Width;

            var fillScreenScaler = 480.0 / Canvas.Height;

            Canvas.Width *= fillScreenScaler;
            Canvas.Height *= fillScreenScaler;

            if (Orientation.HasFlag(PageOrientation.LandscapeLeft))
            {
                _rotation = _device.SensorRotationInDegrees - 90;
            }
            else if (Orientation.HasFlag(PageOrientation.LandscapeRight))
            {
                _rotation = _device.SensorRotationInDegrees + 90;
            }
            else // PageOrientation.PortraitUp
            {
                _rotation = _device.SensorRotationInDegrees;
            }

            ViewfinderVideoBrush.RelativeTransform = new ScaleTransform()
            {
                CenterX = 0.5,
                CenterY = 0.5,
                ScaleX = _zoom,
                ScaleY = _zoom
            };

            Canvas.RenderTransform = new CompositeTransform()
            {
                CenterX = Canvas.Width / 2.0,
                CenterY = Canvas.Height / 2.0,
                Rotation = _rotation
            };

            InterestAreaPolygon.RenderTransform = new CompositeTransform()
            {
                CenterX = 0.5,
                CenterY = 0.5,
                ScaleX = Canvas.Width / _device.PreviewResolution.Width * _zoom,
                ScaleY = Canvas.Height / _device.PreviewResolution.Height * _zoom,
                TranslateX = -(Canvas.Width * _zoom - Canvas.Width) / 2,
                TranslateY = -(Canvas.Height * _zoom - Canvas.Height) / 2
            };

            InterestAreaPolygon.StrokeThickness = 8.0 / _zoom;
        }

        private void PhotoCaptureDevice_PreviewFrameAvailable(ICameraCaptureDevice sender, object args)
        {
            if (_active && !_processing && _device != null)
            {
                _processing = true;
                _device.PreviewFrameAvailable -= PhotoCaptureDevice_PreviewFrameAvailable;

                var width = (uint)_device.PreviewResolution.Width;
                var height = (uint)_device.PreviewResolution.Height;

                byte[] buffer = new byte[width * height];

                sender.GetPreviewBufferY(buffer);

                var frame = new Frame()
                {
                    Buffer = buffer,
                    Pitch = width,
                    Format = FrameFormat.Gray8,
                    Dimensions = new Windows.Foundation.Size(width, height)
                };

                Dispatcher.BeginInvoke(async () =>
                {
                    if (_active)
                    {
                        var tuple = await ProcessFrameAsync(frame);

                        _processing = false;

                        if (_active && _device != null)
                        {
                            if (tuple != null && ResultTextBlock.Text != tuple.Item1.Text)
                            {
                                _resultTuple = tuple;

                                ResultTextBlock.Text = _resultTuple.Item1.Text;
                                ResultImage.Source = _resultTuple.Item2;

                                if (OpticalReaderTask.Instance.RequireConfirmation)
                                {
                                    ResultGrid.Visibility = System.Windows.Visibility.Visible;
                                }
                                else
                                {
                                    NavigationService.GoBack();
                                }
                            }

                            _device.PreviewFrameAvailable += PhotoCaptureDevice_PreviewFrameAvailable;
                        }
                    }
                    else
                    {
                        _processing = false;
                    }
                });
            }
        }

        private async Task<Tuple<ProcessResult, WriteableBitmap>> ProcessFrameAsync(OpticalReaderLib.Frame frame)
        {
            //System.Diagnostics.Debug.WriteLine("Start processing");
            
            var rectSize = new Windows.Foundation.Size(
                ReaderBorder.ActualWidth / Canvas.ActualWidth * frame.Dimensions.Width / _zoom,
                ReaderBorder.ActualHeight / Canvas.ActualHeight * frame.Dimensions.Height / _zoom);

            var rectOrigin = new Windows.Foundation.Point(
                frame.Dimensions.Width / 2 - rectSize.Width / 2,
                frame.Dimensions.Height / 2 - rectSize.Height / 2);

            var area = new Windows.Foundation.Rect(rectOrigin, rectSize);

            ProcessResult result = null;

            try
            {
                result = await OpticalReaderTask.Instance.Processor.ProcessAsync(frame, area, _rotation);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Processing frame failed: {0}\n{1}", ex.Message, ex.StackTrace));
            }

            //System.Diagnostics.Debug.WriteLine("Stop processing");

            InterestAreaPolygon.Points = null;

            if (result != null)
            {
                _lastSuccess = DateTime.Now;

                var thumbnail = GenerateThumbnail();

                var interestPointCollection = new PointCollection();

                foreach (var point in result.InterestPoints)
                {
                    interestPointCollection.Add(new System.Windows.Point(point.X, point.Y));
                }

                InterestAreaPolygon.Points = interestPointCollection;

                return new Tuple<ProcessResult, WriteableBitmap>(result, thumbnail);
            }
            else
            {
                var sinceLastSuccess = DateTime.Now - _lastSuccess;

                if (sinceLastSuccess > OpticalReaderTask.Instance.FocusInterval)
                {
                    try
                    {
                        var status = await _device.FocusAsync();

                        _lastSuccess = DateTime.Now;

                        // todo use camera focus lock status
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Focusing camera failed: {0}\n{1}", ex.Message, ex.StackTrace));
                    }
                }

                return null;
            }
        }

        private Windows.Foundation.Size GetFirstWideResolution(Windows.Foundation.Size[] resolutions)
        {
            foreach (var resolution in resolutions)
            {
                if (resolution.Width / resolution.Height > 1.6)
                {
                    return resolution;
                }
            }

            throw new ArgumentException();
        }

        private WriteableBitmap GenerateThumbnail()
        {
            var thumbnailBitmap = new WriteableBitmap((int)ReaderBorder.ActualWidth, (int)ReaderBorder.ActualHeight);
            var thumbnailTransform = new CompositeTransform()
            {
                CenterX = Canvas.Width / 2.0,
                CenterY = Canvas.Height / 2.0,
                Rotation = _rotation,
                TranslateX = -(Canvas.ActualWidth - ReaderBorder.ActualWidth) / 2,
                TranslateY = -(Canvas.ActualHeight - ReaderBorder.ActualHeight) / 2
            };

            thumbnailBitmap.Render(Canvas, thumbnailTransform);
            thumbnailBitmap.Invalidate();

            return thumbnailBitmap;
        }

        private void ResultGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_active)
            {
                _active = false;

                NavigationService.GoBack();
            }
        }
    }
}