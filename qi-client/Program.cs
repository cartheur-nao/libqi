﻿using qi.Messaging;

namespace qi.client
{
    class Program
    {
        static void Main(string[] args)
        {
            Application _app = new Application(args);
            string connectionAddr;

            // Avoid painfull warning
            if (_app == null)
                return;
            if (args.Length != 2)
            {
                Console.WriteLine("Usage : /qi-client-dotnet master-address");
                Console.WriteLine("Assuming master-address is tcp://127.0.0.1:9559");
                connectionAddr = "tcp://127.0.0.1:9559";
            }
            else
            {
                connectionAddr = args[1];
            }

            Session session = new Session();
            if (session.Connect(connectionAddr) == false)
            {
                Console.WriteLine("Cannot connect to service directory (" + connectionAddr + ")");
                return;
            }

            GenericObject obj = session.Service("serviceTest");

            if (obj == null)
            {
                Console.WriteLine("Service serviceTest is not reachable.");
                return;
            }

            string textToSend = "plaf";
            Message message = new Message();
            Console.WriteLine("Send: " + textToSend);
            message.WriteString(textToSend);
            // It's gonna be...
            Future future = obj.Call("reply::(s)", message);

            // wait for it
            future.Wait();

            if (future.IsError() == true)
                Console.WriteLine("An error occurred");

            // LEGENDARY
            if (future.IsReady() == true && future.IsError() == false)
            {
                Message answer = future.GetValue();
                string value = answer.ReadString();
                Console.WriteLine("Reply: " + value);
            }

            session.Disconnect();
        }
    }
}
