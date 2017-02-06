using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace promodz
{
    class SfCodecException : Exception
    {

        int sf_error;
        string message;


        public SfCodecException(int sf_error, string message)
        {
            this.sf_error = sf_error;
            this.message = message;
        }

        public int Sf_error
        {
            get
            {
                return sf_error;
            }

        }

        public override string Message 
        {
            get
            {
                return message;
            }
        }
    }
}
