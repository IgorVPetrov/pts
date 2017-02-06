using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace promodz.Hardware
{
    interface IMessage
    {
        ProModzHardware.CCS_ID_list CCS_ID { get; }
    }
}
