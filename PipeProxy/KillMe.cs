using System.IO.Pipes;

namespace PipeProxy
{
    internal class KillMe
    {
        public string Name { get; set; }
        byte[] buffer = new byte[65535];
        bool connectedOrWaiting;

        NamedPipeServerStream pipeServer;
        public KillMe(string name)
        {
            connectedOrWaiting = false;
            Name = name;
            pipeServer = new NamedPipeServerStream(Name, PipeDirection.InOut, 1, PipeTransmissionMode.Byte);

            Console.WriteLine($"[Server | {Name}] Starting..");
        }

        public bool Cancel { get; set; }

        public void ExecuteLongRunningTask()
        {
            while (!Cancel)
            {
                if (!connectedOrWaiting)
                {
                    pipeServer.BeginWaitForConnection((a) => { pipeServer.EndWaitForConnection(a); }, null);

                    connectedOrWaiting = true;
                }

                if (pipeServer.IsConnected)
                {
                    Console.WriteLine($"[Server | {Name}] IsConnected!");
                    int count = pipeServer.Read(buffer, 0, 65535);
                    //Console.WriteLine($"User: {pipeServer.GetImpersonationUserName()}");
                    //pipeServer.RunAsClient(new PipeStreamImpersonationWorker(x));

                    if (count > 0)
                    {
                        MemoryStream ms = new(count);
                        ms.Write(buffer, 0, count);
                        var BufferDone = ms.ToArray();
                        ms.Dispose();
                        ms.Close();
                        string ReadedAsCoolBytes = BitConverter.ToString(BufferDone);
                        File.AppendAllText($"req_as_bytes_{Name.Replace("\\","")}.txt", ReadedAsCoolBytes + "\n");
                        Console.WriteLine($"[Server | {Name}] Message got readed!\n" + ReadedAsCoolBytes);
                    }
                }
                Thread.Sleep(10);
            }
            Console.WriteLine($"[Server | {Name}] Is Cancelled!");
            if (pipeServer.IsConnected)
            {
                pipeServer.Disconnect();
            }
            pipeServer.Dispose();
        }

    }
}
