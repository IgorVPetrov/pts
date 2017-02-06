using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Timers;

namespace promodz.Hardware
{
    class SerialPortConnection : IProModzHardwareConnection
    {

        SfCodec codec = new SfCodec();
        SerialPort port;
        Int32 speed = 115200;
        Double devicePresentControlTimerInterval = 1500;
        System.Timers.Timer devicePresentControlTimer = new System.Timers.Timer();

        object objectLock1 = new Object();
        object objectLock2 = new Object();

        event PacketReceivedEventHandler packet_received;
        event EventHandler device_disconnected;

        event PacketReceivedEventHandler IProModzHardwareConnection.PacketReceived
        {
            add
            {
                lock (objectLock1)
                {
                    packet_received += value;
                }
            }

            remove
            {
                lock (objectLock1)
                {
                    packet_received -= value;
                }
            }
        }

        event EventHandler IProModzHardwareConnection.DeviceDisconnected
        {
            add
            {
                lock (objectLock2)
                {
                    device_disconnected += value;
                }
            }

            remove
            {
                lock (objectLock2)
                {
                    device_disconnected += value;
                }
            }
        }

        public SerialPortConnection()
        {
            devicePresentControlTimer.Interval = devicePresentControlTimerInterval;
            devicePresentControlTimer.Elapsed += DevicePresentControlTimer_Elapsed;
        }

        private void DevicePresentControlTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            port.Dispose();
            devicePresentControlTimer.Stop();
            if (device_disconnected != null)
            {
                device_disconnected(this, new EventArgs());
            }

        }

        bool IProModzHardwareConnection.Open()
        {
            string[] portNames = SerialPort.GetPortNames();//Win32Exception
            foreach (string portName in portNames)
            {
                if (TryConnectToHardvare(portName))
                {
                    devicePresentControlTimer.Start();
                    return true;
                }
            }

            return false;
        }

        bool TryConnectToHardvare(string portName)
        {
            try {
                port = new SerialPort(portName, this.speed, Parity.None, 8, StopBits.One);
            }
            catch(IOException)
            {
                return false;
            }
            bool result = false;
            port.ReadTimeout = 500;
            try {
                port.Open();
            }
            catch (Exception)
            {
                port.Dispose();
                return false;
            }
            Thread.Sleep(300);
            string message;
            for(int i = 0; i < 6; i++)
            {
                try
                {
                    message = port.ReadLine();
                    codec.sf_decode_packet(message);
                    result = true;
                    port.DataReceived += new SerialDataReceivedEventHandler(portDataReceived);
                    port.ErrorReceived += new SerialErrorReceivedEventHandler(portErrorReceived);
                    port.PinChanged += new SerialPinChangedEventHandler(portPinChanged);
                }
                catch (SfCodecException)
                {
                    Thread.Sleep(100);
                    continue;
                }
                catch (InvalidOperationException)
                {
                    Thread.Sleep(200);
                    continue;
                }
                catch (TimeoutException)
                {
                    port.Dispose();
                    break;
                }
            }

            return result;
        }



        bool IProModzHardwareConnection.TransmitPacket(byte[] packet)
        {
            throw new NotImplementedException();
        }


        public void portDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string packet_str;
            byte[] packet;
            try
            {
                packet_str = port.ReadLine();
                //this.Invoke(new addStringToTextBoxDelegate(decode), message);
                packet = codec.sf_decode_packet(packet_str);

                
            }
            catch (Exception ee)
            {
                return;
                //message = ee.Message;
                //this.Invoke(new addStringToTextBoxDelegate(addText), message);
            }
            if (packet_received != null)
            {
                packet_received(this, new PacketReceivedEventArgs(this, packet));
            }
            devicePresentControlTimer.Stop();
            devicePresentControlTimer.Start();

        }

        public void portErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            string message = e.ToString();
            //this.Invoke(new addStringToTextBoxDelegate(addText), message);
        }
        public void portPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            string message = e.ToString();
            //this.Invoke(new addStringToTextBoxDelegate(addText), message);
        }

        public void Dispose()
        {
            if(port != null)port.Dispose();
        }
    }
}
