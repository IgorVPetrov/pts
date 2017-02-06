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
using System.Windows.Media.Animation;
using promodz.Hardware;
using System.Timers;

namespace promodz
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        IProModzHardwareConnection connection = new SerialPortConnection();
        Timer hardwareSearchTimer = new Timer(1000);
        public delegate void addStringToTextBoxDelegate(string msg);
        public delegate void setStringToTextBoxDelegate(string msg);
        

        public MainWindow()
        {
            InitializeComponent();
            connection.PacketReceived += Connection_PacketReceived;
            connection.DeviceDisconnected += Connection_DeviceDisconnected;
            hardwareSearchTimer.Elapsed += searchTimerEvent;
            hardwareSearchTimer.Start();
        }

        private void Connection_DeviceDisconnected(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new setStringToTextBoxDelegate(setText), "Потеряна связь с устройством");
            hardwareSearchTimer.Start();
        }

        private void Connection_PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(new addStringToTextBoxDelegate(appendText), simpleBytesToString(e.Data));
        }
        private void searchTimerEvent(object source, ElapsedEventArgs e)
        {
            hardwareSearchTimer.Stop();
            if (connection.Open())
            {
                this.Dispatcher.Invoke(new setStringToTextBoxDelegate(setText), "Устройство найдено"+Environment.NewLine);
                return;
            }

            
            //this.textBox.Text = "Поиск устройства";
            this.Dispatcher.Invoke(new setStringToTextBoxDelegate(setText), "Поиск устройства");
            hardwareSearchTimer.Start();

        }

        private string simpleBytesToString(byte[] data)
        {
            StringBuilder data_str = new StringBuilder();
            foreach (byte d in data)
            {
                data_str.Append(d.ToString("X"));
                data_str.Append(' ');
            }
            return data_str.ToString();
        }

        public void setText(string m)
        {
            //textBox.Text = m;
        }
        public void appendText(string m)
        {
            //textBox.AppendText(m + Environment.NewLine);
            //textBox.LineDown();
            //if(textBox.LineCount > 1000)
            //{
            //    textBox.Clear();
            //}

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            connection.Dispose();
        }

        private void titleBarGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void closeWndButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
