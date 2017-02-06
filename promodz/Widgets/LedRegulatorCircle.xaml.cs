using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace promodz.Widgets
{
    /// <summary>
    /// Логика взаимодействия для LedRegulatorCircle.xaml
    /// </summary>

    public partial class LedRegulatorCircle : UserControl
    {

        double angleOffset = -135;
        bool isDrag = false;
        Point centerPoint = new Point(131, 156);
        double thumbAngle;
        double thumbEdgeDeltaAngle = Math.Asin(10.5 / 67.0) * 180.0 / Math.PI;
        double mouseAngle;
        double mouseDeltaAngle;
        Point currentMousePosition;


        static LinearGradientBrush defaultBrush = new LinearGradientBrush(

            new GradientStopCollection(new List<GradientStop>() {
                new GradientStop(Color.FromRgb(0x00, 0xff, 0xde),0),
                new GradientStop(Color.FromRgb(0x36, 0x36, 0xff),0.2),
                new GradientStop(Color.FromRgb(0xe9, 0x13, 0x88),0.4),
                new GradientStop(Color.FromRgb(0xeb, 0x2d, 0x2e),0.6),
                new GradientStop(Color.FromRgb(0xfd, 0xe9, 0x2b),0.8),
                new GradientStop(Color.FromRgb(0x00, 0x9e, 0x54),1),
            }),
            new Point(0, 0),
            new Point(1, 0)

            );

        public static readonly DependencyProperty DiscBrushProperty = DependencyProperty.Register(
            "DiscBrush",
            typeof(LinearGradientBrush),
            typeof(SmallLiqLevelView),
            new FrameworkPropertyMetadata(defaultBrush)
        );

        public static void SetDiscBrush(UIElement element, LinearGradientBrush value)
        {
            element.SetValue(DiscBrushProperty, value);
        }
        public static LinearGradientBrush GetHgBrush10(UIElement element)
        {
            return (LinearGradientBrush)element.GetValue(DiscBrushProperty);
        }


        public LinearGradientBrush DiscBrush
        {
            get { return (LinearGradientBrush)GetValue(DiscBrushProperty); }
            set { SetValue(DiscBrushProperty, value); }
        }




        public LedRegulatorCircle()
        {

            InitializeComponent();

            InitColorDisc(1);
        }

        private void InitThumb()
        {




        }

        private void InitColorDisc(double sectorAngle)
        {

            Path sectorPath = null;
            for (double angle = 0; angle < 360; angle += sectorAngle)
            {
                sectorPath = GetSectorPath(sectorAngle);
                sectorPath.Fill = ConvertAngleToBrush(angle);
                sectorPath.Stroke = sectorPath.Fill;
                sectorPath.StrokeThickness = 1;
                sectorPath.RenderTransform = new RotateTransform(angle + angleOffset, 131, 156);

                canvas.Children.Add(sectorPath);
            }

        }

        private Path GetSectorPath(double sectorAngle)
        {

            Path sectorPath = new Path();
            double sectorArcX = 131 + 92 * Math.Cos(sectorAngle * Math.PI / 180);
            double sectorArcY = 156 - 92 * Math.Sin(sectorAngle * Math.PI / 180);
            Point arcFinPoint = new Point(sectorArcX, sectorArcY);
            PathFigure sectorFigure = new PathFigure();

            sectorFigure.StartPoint = new Point(131, 156);
            LineSegment sectorLine = new LineSegment(new Point(221, 156), true);

            ArcSegment sectorArc = new ArcSegment(arcFinPoint, new Size(92, 92), 0, false, SweepDirection.Counterclockwise, true);
            sectorFigure.Segments.Add(sectorLine);
            sectorFigure.Segments.Add(sectorArc);

            PathGeometry sectorGeometry = new PathGeometry();
            sectorGeometry.Figures.Add(sectorFigure);

            sectorPath.Data = sectorGeometry;


            return sectorPath;
        }

        private SolidColorBrush ConvertAngleToBrush(double angle)
        {

            angle = angle % 360.0;
            angle = angle / 360.0;

            IEnumerable<GradientStop> low = DiscBrush.GradientStops.TakeWhile(gs => gs.Offset <= angle);
            IEnumerable<GradientStop> high = DiscBrush.GradientStops.SkipWhile(gs => gs.Offset <= angle);

            GradientStop lowGradient = low.Last();
            GradientStop highGradient = high.First();


            double lowOffset = lowGradient.Offset;
            double highOffset = highGradient.Offset;


            Color lowColor = lowGradient.Color;
            Color highColor = highGradient.Color;

            double ScA = 1;
            double ScB = equality(angle, lowOffset, lowColor.ScB, highOffset, highColor.ScB);
            double ScR = equality(angle, lowOffset, lowColor.ScR, highOffset, highColor.ScR);
            double ScG = equality(angle, lowOffset, lowColor.ScG, highOffset, highColor.ScG);
            Color color = Color.FromScRgb((float)ScA, (float)ScR, (float)ScG, (float)ScB);

            return new SolidColorBrush(color);
        }

        private double equality(double x, double x1, double y1, double x2, double y2)
        {
            return y1 + ((y2 - y1) * (x - x1)) / (x2 - x1);
        }

        private void CalcMouseAngle()
        {
            currentMousePosition = System.Windows.Input.Mouse.GetPosition(canvas);
            double mouseX = currentMousePosition.X - centerPoint.X;
            double mouseY = currentMousePosition.Y - centerPoint.Y;
            double mouseR = Math.Sqrt(mouseX * mouseX + mouseY * mouseY);
            mouseAngle = Math.Asin(mouseY / mouseR) * 180.0 / Math.PI;

        }

        private void thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            thumb.CaptureMouse();
            isDrag = true;
            CalcMouseAngle();
            mouseDeltaAngle = ((RotateTransform)(thumb.RenderTransform)).Angle - mouseAngle;
            if (currentMousePosition.X > centerPoint.X)
            {
                mouseDeltaAngle = ((RotateTransform)(thumb.RenderTransform)).Angle - mouseAngle;
            }
            else
            {
                mouseDeltaAngle = 180 - ((RotateTransform)(thumb.RenderTransform)).Angle - mouseAngle;
            }
            e.Handled = true;
        }

        private void thumb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDrag = false;
            thumb.ReleaseMouseCapture();
            e.Handled = true;
        }

        private void thumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrag)
            {
                CalcMouseAngle();

                if (currentMousePosition.X > centerPoint.X)
                {
                    ((RotateTransform)(thumb.RenderTransform)).Angle = mouseAngle + mouseDeltaAngle;
                }
                else
                {
                    ((RotateTransform)(thumb.RenderTransform)).Angle = 180.0 - mouseAngle + mouseDeltaAngle;
                }
                e.Handled = true;

            }
        }


    }
}
