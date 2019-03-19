using System;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace IOSerialPortTest
{
    class Program
    {
        static int Main(string[] args)
        {
            return MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task<int> MainAsync(string[] args)
        {
            SerialPort serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            serialPort.NewLine = "\r";
            serialPort.Open();

            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();

            Console.WriteLine("> Connected");

            string[] answer = default(string[]);
            serialPort.WriteLine("CON");
            answer = await ReadDataAsync(serialPort);
            HandleData(answer);
            serialPort.WriteLine("AUX");
            answer = await ReadDataAsync(serialPort);
            HandleData(answer);

            bool _continue = true;
            do
            {
                //String input = Console.ReadLine();
                //serialPort.WriteLine(input);
                //answer = await ReadDataAsync(serialPort);
                //HandleData(answer);

                serialPort.WriteLine("NUL");
                answer = await ReadDataAsync(serialPort);
                HandleData(answer);

                answer = await ReadDataAsync(serialPort);
                if(answer[0] == "DEV")
                {
                    HandleData(answer);

                    answer = await ReadDataAsync(serialPort);
                    HandleData(answer);
                }

                System.Threading.Thread.Sleep(1000);

            } while (_continue);
            Console.Write("> Disconnected");
            return 0;
        }

        private static void HandleData(string[] data)
        {
            for (int idx = 0, lgt = data.Length; idx < lgt; idx++)
            {
                if (data[idx] == string.Empty) continue;

                ConsoleColor bgOld = Console.BackgroundColor
                           , fgOld = Console.ForegroundColor;

                Console.BackgroundColor = fgOld;
                Console.ForegroundColor = bgOld;
                Console.WriteLine($"<< {data[idx]}");

                Console.BackgroundColor = bgOld;
                Console.ForegroundColor = fgOld;
            }
        }


        private static async Task<string[]> ReadDataAsync(SerialPort serialPort)
        {
            byte[] buffer = new byte[serialPort.ReadBufferSize];

            int bytesRead = await serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);
            serialPort.DiscardInBuffer();
            return Encoding.ASCII.GetString(buffer).Substring(0, bytesRead).Split(serialPort.NewLine.ToCharArray());
        }
    }
}
