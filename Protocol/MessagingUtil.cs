using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Protocol
{
    public class MessagingUtil
    {
        public static async Task SendMessage(NetworkStream stream, IMessage message)
        {
            string serialized = message.ToJson();
            byte[] lengthPrefix = BitConverter.GetBytes(serialized.Length);
            byte[] data = Encoding.UTF8.GetBytes(serialized);
            byte[] buffer = new byte[lengthPrefix.Length + data.Length];
            lengthPrefix.CopyTo(buffer, 0);
            data.CopyTo(buffer, lengthPrefix.Length);

            try
            {
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task<dynamic> ReceiveMessage(NetworkStream stream)
        {
            byte[] prefix = new byte[sizeof(int)];
            int bytesRead = 0;
            byte[] data;

            try
            {
                while (bytesRead < prefix.Length)
                {
                    bytesRead += await stream.ReadAsync(prefix, bytesRead, prefix.Length - bytesRead);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            bytesRead = 0;
            try
            {
                data = new byte[BitConverter.ToInt32(prefix, 0)];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            try
            {
                while (bytesRead < data.Length)
                {
                    bytesRead += await stream.ReadAsync(data, bytesRead, data.Length - bytesRead);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            try
            {
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}