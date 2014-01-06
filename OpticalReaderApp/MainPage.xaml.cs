using Microsoft.Devices;
using Microsoft.Phone.Controls;
using OpticalReaderLib;
using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using Windows.Phone.Media.Capture;

namespace OpticalReaderApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ZxingProcessor _processor = new ZxingProcessor();
        private double _zoom = 1;
        private double _rotation = 0;
        private bool _processing = false;
        private DateTime _lastSuccess = DateTime.Now;

        public MainPage()
        {
            InitializeComponent();

            var sensorRotation = App.Camera.SensorRotationInDegrees;
            var resolution = App.Camera.PreviewResolution;
            var objectSize = new Windows.Foundation.Size(65, 65);
            var parameters = OpticalReaderLib.Information.GetSuggestedParameters(resolution, sensorRotation, objectSize);

            // Although the library might suggest zooming out we don't want to do it as
            // we're using a VideoBrush and UniformToFill to render the viewfinder ->
            // the whole viewfinder might not render correctly.
            //_zoom = Math.Max(parameters.Zoom, 1.0);

            if (parameters.IsAccurate)
            {
                InformationTextBlock.Text = String.Format("MFD {0} cm", parameters.Distance / 10);
            }

            AdaptToOrientation();

            OrientationChanged += MainPage_OrientationChanged;
        }

        void MainPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            AdaptToOrientation();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewfinderVideoBrush.SetSource(App.Camera);

            App.Camera.PreviewFrameAvailable += Camera_PreviewFrameAvailable;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            App.Camera.PreviewFrameAvailable -= Camera_PreviewFrameAvailable;
            App.CameraSemaphore.WaitOne();
        }

        private void AdaptToOrientation()
        {
            if (App.Camera != null)
            {
                if (Orientation.HasFlag(PageOrientation.LandscapeLeft))
                {
                    _rotation = App.Camera.SensorRotationInDegrees - 90;
                }
                else if (Orientation.HasFlag(PageOrientation.LandscapeRight))
                {
                    _rotation = App.Camera.SensorRotationInDegrees + 90;
                }
                else // PageOrientation.PortraitUp
                {
                    _rotation = App.Camera.SensorRotationInDegrees;
                }

                var transform = new CompositeTransform()
                {
                    CenterX = 0.5,
                    CenterY = 0.5,
                    ScaleX = _zoom,
                    ScaleY = _zoom,
                    Rotation = _rotation
                };

                ViewfinderVideoBrush.RelativeTransform = transform;
                InterestAreaPolygon.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                InterestAreaPolygon.RenderTransform = transform;
            }
        }

        private void Camera_PreviewFrameAvailable(ICameraCaptureDevice sender, object args)
        {
            if (!_processing)
            {
                _processing = true;

                var width = (uint)App.Camera.PreviewResolution.Width;
                var height = (uint)App.Camera.PreviewResolution.Height;

                byte[] buffer = new byte[width * height];
                sender.GetPreviewBufferY(buffer);
                var frame = new Frame()
                {
                    Buffer = buffer,
                    Pitch = width,
                    Format = FrameFormat.Gray8,
                    Dimensions = new Windows.Foundation.Size(width, height)
                };

                //var bgraBuffer = new int[width * height * 4];
                //sender.GetPreviewBufferArgb(bgraBuffer);
                //byte[] buffer = new byte[bgraBuffer.Length * 4];
                //Buffer.BlockCopy(bgraBuffer, 0, buffer, 0, buffer.Length);
                //var frame = new Frame()
                //{
                //    Buffer = buffer,
                //    Pitch = width * 4,
                //    Format = FrameFormat.Bgra32,
                //    Dimensions = new Windows.Foundation.Size(width, height)
                //};

                Dispatcher.BeginInvoke(async () =>
                {
                    await ProcessFrameAsync(frame, _rotation, _zoom);

                    _processing = false;
                });
            }
        }

        private async Task ProcessFrameAsync(OpticalReaderLib.Frame frame, double rotation, double zoom)
        {
            var result = await _processor.ProcessAsync(frame, rotation, zoom);

            PreviewImage.Source = await _processor.RenderPreviewAsync(frame, frame.Dimensions);

            if (result != null)
            {
                ResultTextBlock.Text = String.Format("{0}\n{1}", result.Format, result.Text);

                var pointCollection = new PointCollection();
                var scaler = ViewfinderGrid.ActualHeight / frame.Dimensions.Height;
                var horizontalMargin = (frame.Dimensions.Width * scaler - ViewfinderGrid.ActualWidth) / 2;
         
                foreach (var point in result.InterestPoints)
                {
                    pointCollection.Add(new System.Windows.Point(point.X * scaler - horizontalMargin, point.Y * scaler));
                }

                InterestAreaPolygon.Points = pointCollection;

                _lastSuccess = DateTime.Now;
            }
            else
            {
                ResultTextBlock.Text = "";
                InterestAreaPolygon.Points = null;

                if ((DateTime.Now - _lastSuccess).TotalMilliseconds > 2500)
                {
                    var status = await App.Camera.FocusAsync();

                    if (status == CameraFocusStatus.Locked)
                    {
                        StatusTextBlock.Text = "Focus locked";
                    }
                    else
                    {
                        StatusTextBlock.Text = "No focus lock";

                    }

                    _lastSuccess = DateTime.Now;
                }
            }
        }
    }
}