using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HolidayDesktop.Common;

namespace HolidayDesktop.Animation
{
    class AnimationEngine
    {
        private Canvas _canvas;
        private Image _image;
        private IList<ImageFileInfo> _imageFiles;
        private DispatcherTimer _timerForShow;
        private DispatcherTimer _timerForHide;
        private bool _isWorking;
        private int _currentImageIndex;

        private ImageFileInfo CurrentImageFile
        {
            get
            {
                _currentImageIndex = _currentImageIndex >= _imageFiles.Count - 1 ? 0 : _currentImageIndex + 1;
                return _imageFiles[_currentImageIndex];
            }
        }

        public IList<string> FileNames { get; set; }

        public TimeSpan IntervalBetweenImages { get; set; }

        public TimeSpan IntervalForShowImage { get; set; }

        public AnimationEngine(Canvas canvas, TimeSpan intervalBetweenImages, TimeSpan intervalForShowImage, params string[] fileNames)
        {
            _canvas = canvas;
            IntervalBetweenImages = intervalBetweenImages;
            IntervalForShowImage = intervalForShowImage;
            _imageFiles = new List<ImageFileInfo>();                
            _timerForShow = new DispatcherTimer();
            _timerForShow.Interval = intervalBetweenImages;
            _timerForShow.Tick += TimerForShowTick;
            _timerForHide = new DispatcherTimer();
            _timerForHide.Interval = intervalForShowImage;
            _timerForHide.Tick += TimerForHideTick;
            _isWorking = false;
            _currentImageIndex = -1;
            _image = new Image();
            _image.Stretch = Stretch.None;
            _canvas.Children.Add(_image);
            FileNames = fileNames;
        }

        private void TimerForHideTick(object sender, EventArgs e)
        {
            _timerForHide.Stop();
            _image.ChangeSource(new BitmapImage(new Uri("pack://application:,,,/Images/1x1.png")), TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));
            _timerForShow.Start();
        }

        private void TimerForShowTick(object sender, EventArgs e)
        {
            _timerForShow.Stop();
            var currentImage = CurrentImageFile;
            var canvasWidth = (int)_canvas.RenderSize.Width;
            var canvasHeight = (int)_canvas.RenderSize.Height;
            var random = new Random();
            var positionX = random.Next(0, canvasWidth);
            var positionY = random.Next(0, canvasHeight);
            positionX = (canvasWidth - positionX) < currentImage.Image.Width ? canvasWidth - currentImage.Image.Width : positionX;
            positionY = (canvasHeight - positionY) < currentImage.Image.Height ? canvasHeight - currentImage.Image.Height : positionY;
            Canvas.SetLeft(_image, positionX);
            Canvas.SetTop(_image, positionY);
            _image.Width = currentImage.Image.Width;
            _image.Height = currentImage.Image.Height;
            _image.ChangeSource(new BitmapImage(new Uri(currentImage.FileName)), TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));
            _timerForHide.Start();
        }

        public void Start()
        {
            if (_isWorking)
            {
                return;
            }
            _isWorking = true;
            _imageFiles = FileNames.Select(x => new ImageFileInfo { FileName = x, Image = new System.Drawing.Bitmap(x) }).ToList();
            _timerForHide.Interval = IntervalForShowImage;
            _timerForShow.Interval = IntervalBetweenImages;
            _timerForShow.Start();
        }

        public void Stop()
        {
            if (!_isWorking)
            {
                return;
            }
            _isWorking = false;
            _currentImageIndex = -1;
            _timerForShow.Stop();
            _timerForHide.Stop();
        }
    }
}