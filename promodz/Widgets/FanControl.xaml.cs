using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace promodz.Widgets
{
    /// <summary>
    /// Логика взаимодействия для FanControl.xaml
    /// </summary>
    public partial class FanControl : UserControl
    {


        Storyboard myStoryboard = null;

        double maxFanSpeed = 2.5;

        double oldSliderValue = 0;

        public FanControl()
        {
            InitializeComponent();

            oldSliderValue = slider.Value;
        }
        private void InitAnimation(double period)
        {
            period = period >10000 ? 10000 : period;

            double angle = fanRotateTransform.Angle;

            DoubleAnimation fanAnimation = new DoubleAnimation();
            fanAnimation.From = angle;
            fanAnimation.To = 360 + angle;
            fanAnimation.Duration = new Duration(TimeSpan.FromSeconds(period));
            fanAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard oldStoryBoard =  myStoryboard;
            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(fanAnimation);
            Storyboard.SetTargetName(fanAnimation, "fanRotateTransform");
            Storyboard.SetTargetProperty(fanAnimation, new PropertyPath(RotateTransform.AngleProperty));
            myStoryboard.Begin(this);
            if (oldStoryBoard != null)
            {
                oldStoryBoard.Remove();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Speed.Text = ((Int32)(slider.Value)).ToString();
            //InitAnimation(1000);
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Speed.Text = ((Int32)(slider.Value)).ToString();

            if (Math.Abs(slider.Value - oldSliderValue) < Math.Abs(slider.Maximum - slider.Minimum) / 20 && slider.Value != 0)
            {
                return;
            }

            oldSliderValue = slider.Value;

            double period;

            if (slider.Value == 0)
            {
                period = 10000;
            }
            else
            {
                period = slider.Maximum / (slider.Value * maxFanSpeed);
            }

            

            InitAnimation(period);
        }
    

    }
}
