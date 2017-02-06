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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace promodz.Widgets
{
    /// <summary>
    /// Логика взаимодействия для SmallLiqLevelView.xaml
    /// </summary>
    public partial class SmallLiqLevelView : UserControl
    {

        static LinearGradientBrush default10 = new LinearGradientBrush(

            new GradientStopCollection(new List<GradientStop>() {
                new GradientStop(Colors.Red,0),
                new GradientStop(Color.FromRgb(0xf6, 0x36, 0),0.43),
                new GradientStop(Color.FromRgb(0xf0, 0x5f, 0),0.81),
                new GradientStop(Color.FromRgb(0xed, 0x6f, 0),1),
            }),
            new Point(0,0),
            new Point(1,0)
            
            );
        static LinearGradientBrush default100 = new LinearGradientBrush(

            new GradientStopCollection(new List<GradientStop>() {
                new GradientStop(Colors.Aqua,0),
                new GradientStop(Color.FromRgb(0, 0xd9, 0xfc),0.13),
                new GradientStop(Color.FromRgb(0, 0x7d, 0xf6),0.45),
                new GradientStop(Color.FromRgb(0, 0x3a, 0xf1),0.71),
                new GradientStop(Color.FromRgb(0, 0x10, 0xee),0.9),
                new GradientStop(Color.FromRgb(0, 0, 0xed),1),
            }),
            new Point(0, 0),
            new Point(1, 0)

            );

        public static readonly DependencyProperty HgBrush10Property = DependencyProperty.Register(
            "HgBrush10",
            typeof(LinearGradientBrush),
            typeof(SmallLiqLevelView),
            new FrameworkPropertyMetadata(default10)
        );
        
        public static void SetHgBrush10(UIElement element, LinearGradientBrush value)
        {
            element.SetValue(HgBrush10Property, value);
        }
        public static LinearGradientBrush GetHgBrush10(UIElement element)
        {
            return (LinearGradientBrush)element.GetValue(HgBrush10Property);
        }
        

        public LinearGradientBrush HgBrush10
        {
            get { return (LinearGradientBrush)GetValue(HgBrush10Property); }
            set { SetValue(HgBrush10Property, value); }
        }

        public static readonly DependencyProperty HgBrush100Property = DependencyProperty.Register(
            "HgBrush100",
            typeof(LinearGradientBrush),
            typeof(SmallLiqLevelView),
            new FrameworkPropertyMetadata(default100)
        );

        public static void SetHgBrush100(UIElement element, LinearGradientBrush value)
        {
            element.SetValue(HgBrush100Property, value);
        }
        public static LinearGradientBrush GetHgBrush100(UIElement element)
        {
            return (LinearGradientBrush)element.GetValue(HgBrush100Property);
        }


        public LinearGradientBrush HgBrush100
        {
            get { return (LinearGradientBrush)GetValue(HgBrush100Property); }
            set { SetValue(HgBrush100Property, value); }
        }


        public SmallLiqLevelView()
        {
            InitializeComponent();

        }
    }
}
