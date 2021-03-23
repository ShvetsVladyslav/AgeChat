using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeChatServer
{
    class FrameParser
    {
        public string ParsePayloadFromFrame(byte[] incomingFrameBytes)
        {
            var b = incomingFrameBytes[1];
            var payloadLength = 0L;
            var totalLength = 0L;
            var keyStartIndex = 0L;


            // 125 or less.
            // When it's below 126, second byte is the payload length.
            if (b - 128 < 126)
            {
                payloadLength = incomingFrameBytes[1] & 0x7F;
                keyStartIndex = 2;
                totalLength = payloadLength + 6;
            }

            // 126-65535.
            // When it's 126, the payload length is in the following two bytes
            if (b - 128 == 126)
            {
                payloadLength = BitConverter.ToInt16(new[] { incomingFrameBytes[3], incomingFrameBytes[2] }, 0);
                keyStartIndex = 4;
                totalLength = payloadLength + 8;
            }

            // 65536 +
            // When it's 127, the payload length is in the following 8 bytes.
            if (b - 128 == 127)
            {
                payloadLength = BitConverter.ToInt64(new[] { incomingFrameBytes[9], incomingFrameBytes[8], incomingFrameBytes[7], incomingFrameBytes[6], incomingFrameBytes[5], incomingFrameBytes[4], incomingFrameBytes[3], incomingFrameBytes[2] }, 0);
                keyStartIndex = 10;
                totalLength = payloadLength + 14;
            }

            if (totalLength > incomingFrameBytes.Length)
            {
                throw new Exception("The buffer length is smaller than the data length.");
            }

            var payloadStartIndex = keyStartIndex + 4;

            byte[] key = { incomingFrameBytes[keyStartIndex], incomingFrameBytes[keyStartIndex + 1], incomingFrameBytes[keyStartIndex + 2], incomingFrameBytes[keyStartIndex + 3] };

            var payload = new byte[payloadLength];
            Array.Copy(incomingFrameBytes, payloadStartIndex, payload, 0, payloadLength);
            for (int i = 0; i < payload.Length; i++)
            {
                payload[i] = (byte)(payload[i] ^ key[i % 4]);
            }

            return Encoding.UTF8.GetString(payload);
        }

        public byte[] CreateFrameFromString(string message)
        {
            var payload = Encoding.UTF8.GetBytes(message);

            byte[] frame;

            if (payload.Length < 126)
            {
                frame = new byte[1 /*op code*/ + 1 /*payload length*/ + payload.Length /*payload bytes*/];
                frame[1] = (byte)payload.Length;
                Array.Copy(payload, 0, frame, 2, payload.Length);
            }
            else if (payload.Length >= 126 && payload.Length <= 65535)
            {
                frame = new byte[1 /*op code*/ + 1 /*payload length option*/ + 2 /*payload length*/ + payload.Length /*payload bytes*/];
                frame[1] = 126;
                frame[2] = (byte)((payload.Length >> 8) & 255);
                frame[3] = (byte)(payload.Length & 255);
                Array.Copy(payload, 0, frame, 4, payload.Length);
            }
            else
            {
                frame = new byte[1 /*op code*/ + 1 /*payload length option*/ + 8 /*payload length*/ + payload.Length /*payload bytes*/];
                frame[1] = 127; // <-- Indicates that payload length is in following 8 bytes.
                frame[2] = (byte)((payload.Length >> 56) & 255);
                frame[3] = (byte)((payload.Length >> 48) & 255);
                frame[4] = (byte)((payload.Length >> 40) & 255);
                frame[5] = (byte)((payload.Length >> 32) & 255);
                frame[6] = (byte)((payload.Length >> 24) & 255);
                frame[7] = (byte)((payload.Length >> 16) & 255);
                frame[8] = (byte)((payload.Length >> 8) & 255);
                frame[9] = (byte)(payload.Length & 255);
                Array.Copy(payload, 0, frame, 10, payload.Length);
            }

            frame[0] = (byte)((byte)Opcode.Text | 0x80 /*FIN bit*/);

            return frame;
        }
    }
}