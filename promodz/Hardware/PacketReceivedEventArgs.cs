using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace promodz.Hardware
{
    public class PacketReceivedEventArgs : EventArgs
    {
        private byte[] data;

        public byte[] Data
        {
            get
            {
                return data;
            }
        }

        public PacketReceivedEventArgs(object sender,byte[] data)
        {
            this.data = data;
        }

       
    }
}
