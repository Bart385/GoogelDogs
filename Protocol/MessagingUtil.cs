using System;
using System.Collections.Generic;
using System.IO;
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

            await stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task<dynamic> ReceiveMessage(NetworkStream stream)
        {
            byte[] prefix = new byte[sizeof(int)];
            int bytesRead = 0;
            byte[] data;
            while (bytesRead < prefix.Length)
            {
                try
                {
                    bytesRead += await stream.ReadAsync(prefix, bytesRead, prefix.Length - bytesRead);
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error reading prefix");
                    return null;
                }
            }

            bytesRead = 0;
            try
            {
                data = new byte[BitConverter.ToInt32(prefix, 0)];
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Error converting byte[] prefix to size");
                return null;
            }

            while (bytesRead < data.Length)
            {
                try
                {
                    bytesRead += await stream.ReadAsync(data, bytesRead, data.Length - bytesRead);
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error reading data");
                    return null;
                }
            }

            try
            {
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data));
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Error converting byte[] data to string");
                return null;
            }
        }
    }
}