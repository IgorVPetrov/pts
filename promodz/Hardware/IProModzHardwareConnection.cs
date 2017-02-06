using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace promodz.Hardware
{
    

    interface IProModzHardwareConnection:IDisposable
    {
        bool Open();
        bool TransmitPacket(byte[] packet);
        event PacketReceivedEventHandler PacketReceived;
        event EventHandler DeviceDisconnected;

    }
}
