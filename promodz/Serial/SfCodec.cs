using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace promodz
{
    class SfCodec
    {
        char sf_start_symbol = '!';
        char sf_end_symbol = '@';

        public byte[] sf_decode(string  source_string )
        {
            byte value_hi;
            byte value_lo;

            int source_string_size;
            int i = 0;

            if ((source_string.Length & 0x01) == 1) throw new SfCodecException(1, "sf_decode : нечётная длина строки");
            source_string_size = source_string.Length >> 1;

            List<byte> destination_data = new List<byte>();

            while (source_string_size > 0)
            {
                source_string_size--;

                value_hi = (byte)(source_string[i] - '0');
                if (value_hi > 9) value_hi -= 7;
                if (value_hi > 0xf) throw new SfCodecException(2, "sf_decode : value_hi > 0xf");
                i++;

                value_lo = (byte)(source_string[i] - '0'); 
                if (value_lo > 9) value_lo -= 7;
                if (value_lo > 0xf) throw new SfCodecException(3, "sf_decode : value_lo > 0xf");
                i++;

                destination_data.Add ((byte)((value_hi << 4) + value_lo));
               
            }


            return destination_data.ToArray();
        }

        public byte[] sf_decode_packet(string source_string )
        {

            int data_size;

            if (source_string[0] != sf_start_symbol)
            {
                throw new SfCodecException(4, "sf_decode_packet : стартовый маркер не найден");

            }
            source_string = source_string.Substring(1);

            data_size = source_string.LastIndexOf(sf_end_symbol);
            if (data_size == -1)
            {
                throw new SfCodecException(5, "sf_decode_packet : завершаюший маркер не найден");
                
            }

            source_string = source_string.Substring(0, data_size);
            if (source_string.Length < 4)
            {
                throw new SfCodecException(6, "sf_decode_packet : длина строки меньше 4-х символов");
            }

            byte[] data = sf_decode(source_string.Substring(0, source_string.Length - 4));

           
            UInt16 crc = CRC16.Get(data);

            byte[] crc_dec_arr = sf_decode(source_string.Substring(source_string.Length - 4));

            UInt16 crc_dec = (UInt16)(crc_dec_arr[1] * 256 + crc_dec_arr[0]);

            if (crc != crc_dec)
            {
                throw new SfCodecException(7, "sf_decode_packet : ошибка CRC");
            }

            return data;
        }

        public string sf_encode(byte[] source_data)
        {
            byte symbol;
            byte value;
            int source_data_size = source_data.Length;
            int i = 0;
            List<char> result = new List<char>();
            while (source_data_size > 0)
            {
                source_data_size--;
                value = source_data[i];
                i++;

                symbol = (byte)(((value >> 4) & 0xf) + '0');
                if (symbol > '9') symbol += 7;
                result.Add((char)symbol);
                

                symbol = (byte)((value & 0xf) + '0');
                if (symbol > '9') symbol += 7;
                result.Add((char)symbol);
            }

            return new string(result.ToArray());
        }

        public string sf_encode_packet(byte[] source_data )
        {
            UInt16 crc = CRC16.Get(source_data);
            byte[] crc_bytes = new byte[2];
            crc_bytes[0] = (byte)(crc & 0xFF);
            crc_bytes[1] = (byte)((crc >> 8) & 0xFF);

            StringBuilder result = new StringBuilder(256);

            result.Append( sf_start_symbol );

            result.Append(sf_encode(source_data));

            result.Append(sf_encode(crc_bytes));

            result.Append(sf_end_symbol);
            //result.Append('\r');
            //result.Append('\n');

            return result.ToString();

        }

    }
}
