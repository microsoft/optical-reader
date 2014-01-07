using Microsoft.Devices;
using Microsoft.Phone.Controls;
using OpticalReaderLib;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Windows.Phone.Media.Capture;

namespace OpticalReaderLib
{
    public partial class OpticalReaderPage : PhoneApplicationPage
    {
        private ZxingProcessor _processor = new ZxingProcessor();
        private double _zoom = 0;
        private double _rotation = 0;
        private bool _processing = false;
        private bool _focusing = false;
        private DateTime _lastSuccess = DateTime.Now;
        private PhotoCaptureDevice _device = null;

        public OpticalReaderPage()
        {
            InitializeComponent();

            OrientationChanged += ViewfinderPage_OrientationChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                InitializeCamera();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Initializing camera failed: {0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            try
            {
                UninitializeCamera();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Uninitializing camera failed: {0}\n{1}", ex.Message, ex.StackTrace));
            }

            OpticalReaderTask.CancelTask(e.NavigationMode == NavigationMode.Back);
        }

        private void ViewfinderPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            AdaptToOrientation();
        }

        private void InitializeCamera()
        {
            var firstWideResolution = new Func<Windows.Foundation.Size[], Windows.Foundation.Size>((array) =>
            {
                foreach (var resolution in array)
                {
                    if (resolution.Width / resolution.Height > 1.6)
                    {
                        return resolution;
                    }
                }

                throw new ArgumentException();
            });

            var captureResolutions = PhotoCaptureDevice.GetAvailableCaptureResolutions(CameraSensorLocation.Back).ToArray();
            var previewResolutions = PhotoCaptureDevice.GetAvailablePreviewResolutions(CameraSensorLocation.Back).ToArray();

            var captureResolution = firstWideResolution(captureResolutions);
            var previewResolution = firstWideResolution(previewResolutions);

            var task = PhotoCaptureDevice.OpenAsync(CameraSensorLocation.Back, captureResolution).AsTask();

            task.Wait(4000);

            _device = task.Result;
            _device.SetPreviewResolutionAsync(previewResolution).AsTask().Wait(1000);

            var objectSize = new Windows.Foundation.Size(65, 65);
            var objectResolutionSide = _device.PreviewResolution.Height * (ReaderBorder.Width - ReaderBorder.Margin.Top) / 480;
            var objectResolution = new Windows.Foundation.Size(objectResolutionSide, objectResolutionSide);
            var centerPoint = new Windows.Foundation.Point(previewResolution.Width / 2, previewResolution.Height / 2);
            var focusRegionSize = new Windows.Foundation.Size(objectResolutionSide, objectResolutionSide);

            _device.FocusRegion = new Windows.Foundation.Rect(
                centerPoint.X - focusRegionSize.Width / 2, centerPoint.Y - focusRegionSize.Height / 2,
                focusRegionSize.Width, focusRegionSize.Height);

            var parameters = OpticalReaderLib.Utilities.GetSuggestedParameters(_device.PreviewResolution, _device.SensorRotationInDegrees, objectSize, objectResolution);

            _zoom = parameters.Zoom;

            if (parameters.IsAccurate)
            {
                InformationTextBlock.Text = String.Format("Minimum focus distance is {0} cm", parameters.Distance / 10);
            }

            AdaptToOrientation();

            ViewfinderVideoBrush.SetSource(_device);

            _device.PreviewFrameAvailable += Camera_PreviewFrameAvailable;

            //Camera.SetProperty(KnownCameraGeneralProperties.AutoFocusRange, AutoFocusRange.Macro);
            //Camera.SetProperty(KnownCameraAudioVideoProperties.VideoTorchMode, VideoTorchMode.On);
        }

        private void UninitializeCamera()
        {
            if (_device != null)
            {
                _device.PreviewFrameAvailable -= Camera_PreviewFrameAvailable;

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

            Canvas.RenderTransform = new CompositeTransform()
            {
                CenterX = Canvas.Width / 2.0,
                CenterY = Canvas.Height / 2.0,
                ScaleX = _zoom,
                ScaleY = _zoom,
                Rotation = _rotation
            };

            InterestAreaPolygon.RenderTransform = new CompositeTransform()
            {
                ScaleX = Canvas.Width / _device.PreviewResolution.Width,
                ScaleY = Canvas.Width / _device.PreviewResolution.Width,
            };
        }

        private void Camera_PreviewFrameAvailable(ICameraCaptureDevice sender, object args)
        {
            if (!_processing && !_focusing)
            {
                _processing = true;

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
                    try
                    {
                        await ProcessFrameAsync(frame, _rotation, _zoom);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Processing frame failed: {0}\n{1}", ex.Message, ex.StackTrace));
                    }

                    _processing = false;
                });
            }
        }

        private async Task ProcessFrameAsync(OpticalReaderLib.Frame frame, double rotation, double zoom)
        {
            var result = await _processor.ProcessAsync(frame, rotation, zoom);

            InterestAreaPolygon.Points = null;

            if (result != null)
            {
                var thumbnail = GenerateThumbnail();
                var interestPointCollection = new PointCollection();

                foreach (var point in result.InterestPoints)
                {
                    interestPointCollection.Add(new System.Windows.Point(point.X, point.Y));
                }

                InterestAreaPolygon.Points = interestPointCollection;

                _lastSuccess = DateTime.Now;

                Dispatcher.BeginInvoke(() =>
                    {
                        OpticalReaderTask.CompleteTask(result, thumbnail);
                    });
            }
            else
            {
                if ((DateTime.Now - _lastSuccess).TotalMilliseconds > 2500)
                {
                    try
                    {
                        _focusing = true;

                        var status = await _device.FocusAsync();

                        _lastSuccess = DateTime.Now;

                        // todo use camera focus lock status
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Focusing camera failed: {0}\n{1}", ex.Message, ex.StackTrace));
                    }

                    _focusing = false;
                }
            }
        }

        private WriteableBitmap GenerateThumbnail()
        {
            var thumbnailBitmap = new WriteableBitmap((int)ReaderBorder.ActualWidth, (int)ReaderBorder.ActualHeight);
            var thumbnailTransform = new CompositeTransform()
            {
                CenterX = Canvas.Width / 2.0,
                CenterY = Canvas.Height / 2.0,
                ScaleX = _zoom,
                ScaleY = _zoom,
                Rotation = _rotation,
                TranslateX = -(Canvas.ActualWidth - ReaderBorder.ActualWidth) / 2,
                TranslateY = -(Canvas.ActualHeight - ReaderBorder.ActualHeight) / 2
            };

            thumbnailBitmap.Render(Canvas, thumbnailTransform);
            thumbnailBitmap.Invalidate();

            return thumbnailBitmap;
        }
    }
}