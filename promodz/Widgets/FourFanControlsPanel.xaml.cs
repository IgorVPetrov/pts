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
    /// Логика взаимодействия для FourFanControlsPanel.xaml
    /// </summary>
    public partial class FourFanControlsPanel : UserControl
    {
        public FourFanControlsPanel()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            fan1.slider.Value = 4000;
            fan1.SystemName.Text = "FAN03:";
            fan1.UserName.Text = "PROCESSOR";

            fan2.slider.Value = 3000;
            fan2.SystemName.Text = "FAN04:";
            fan2.UserName.Text = "VIDEOCARD1";

            fan3.slider.Value = 2000;
            fan3.SystemName.Text = "FAN05:";
            fan3.UserName.Text = "VIDECARD2";

            fan4.slider.Value = 1000;
            fan4.SystemName.Text = "FAN06:";
            fan4.UserName.Text = "BRIDGE";
        }

    }
}
