using System;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace IOSerialPortDevice
{
    class Program
    {
        static int Main(string[] args)
        {
            return MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task<int> MainAsync(string[] args)
        {
            SerialPort serialPort = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
            serialPort.NewLine = "\r";
            serialPort.Open();

            Console.WriteLine("> DEVICE STARTED");
            bool _continue = true;
            String input
                 , output;

            Random rng = new Random();

            do
            {
                input = await ReadDataAsync(serialPort);
                Console.WriteLine($">> {input.Length}:{input.ToUpper()}");

                switch (input.ToUpper())
                {
                    case "NUL":

                        output = $"{input.PadRight(3, ' ').Substring(0, 3).Trim().ToUpper()}x";
                        serialPort.WriteLine(output);
                        Console.WriteLine($"<< {output.Length}:{output}");

                        output = "DEV";
                        serialPort.WriteLine(output);
                        Console.WriteLine($"<< {output.Length}:{output}");

                        System.Threading.Thread.Sleep(5000);

                        output = "STR";
                        serialPort.WriteLine(output);
                        Console.WriteLine($"<< {output.Length}:{output}");

                        break;

                    case "BYE":
                        _continue = false;
                        break;

                    default:
                        output = $"{input.PadRight(3, ' ').Substring(0, 3).Trim().ToUpper()}x";
                        serialPort.WriteLine(output);
                        Console.WriteLine($"<< {output.Length}:{output}");
                        break;
                }
            } while (_continue);
            serialPort.Write($"KTHXBYE");
            Console.WriteLine("> DEVICE STOPPED");
            return 0;
        }

        private static async Task<string> ReadDataAsync(SerialPort serialPort)
        {
            byte[] buffer = new byte[serialPort.ReadBufferSize];

            int bytesRead = await serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);

            return Encoding.ASCII.GetString(buffer).Substring(0, bytesRead - serialPort.NewLine.Length);
        }
    }
}
