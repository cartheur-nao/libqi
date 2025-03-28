using NUnit.Framework;
using qi.Messaging;

namespace qi.tests
{
    [TestFixture]
    unsafe class InterfaceTests
    {
        private string[] args;

        // QiMethod
        public void Reply(char* signature, qi_message_t* message_c, qi_message_t* answer_c, void* data)
        {
            Message message = new Message(new MessagePrivate(message_c, false));
            Message answer = new Message(new MessagePrivate(answer_c, false));
            string str = message.ReadString();

            str += "bim";
            answer.WriteString(str);
        }
        [Test]
        public void SanityCheck()
        {
            Application app = new Application(args);
            string ServiceDirectoryAddress = "tcp://127.0.0.1:9559";

            if (app == null)
            {
                Console.WriteLine("Application initialization failed.");
                Assert.Fail();
            }

            // Declare an object and a method
            GenericObject obj = new GenericObject();
            QiMethod method = new QiMethod(Reply);
            Messaging.Buffer buff = new Messaging.Buffer();

            // Then bind method to object
            if (!obj.RegisterMethod("reply::s(s)", method, buff))
            {
                Console.WriteLine("Method registration failed.");
                Assert.Fail();
            }

            Session session = new Session();

            // Set up your session
            if (!session.Connect(ServiceDirectoryAddress))
            {
                Console.WriteLine("Cannot connect to service directory (" + ServiceDirectoryAddress + ")");
                Assert.Fail();
            }
            session.Listen("tcp://0.0.0.0:0");

            // Expose your object to the world
            int id = session.RegisterService("test call", obj);
            if (id == 0)
            {
                Console.WriteLine("Cannot register service 'test call'.");
                Assert.Fail();
            }

            // Create a client session
            Session client = new Session();
            if (!client.Connect(ServiceDirectoryAddress))
            {
                Console.WriteLine("Client cannot connect to service directory (" + ServiceDirectoryAddress + ")");
                Assert.Fail();
            }

            // Get proxy on service test call
            GenericObject proxy = client.Service("test call");
            if (proxy == null)
            {
                Console.WriteLine("Cannot get proxy on service 'test call'.");
                Assert.Fail();
            }

            // Call reply function
            Message msg = new Message();
            msg.WriteString("plaf");
            Future fut = proxy.Call("reply::(s)", msg);
            if (fut == null)
            {
                Console.WriteLine("Future object is null.");
                Assert.Fail();
            }

            // Wait for answer
            fut.Wait(1000);
            if (fut.IsError())
            {
                Console.WriteLine("Future object has an error.");
                Assert.Fail();
            }
            if (!fut.IsReady())
            {
                Console.WriteLine("Future object is not ready.");
                Assert.Fail();
            }

            // Get answer
            Message answermessage = fut.GetValue();
            if (answermessage == null)
            {
                Console.WriteLine("Answer message is null.");
                Assert.Fail();
            }
            string answer = answermessage.ReadString();
            if (answer != "plafbim")
            {
                Console.WriteLine("Unexpected answer: " + answer);
                Assert.Fail();
            }

            client.Disconnect();
            session.UnregisterService(id);
            session.Disconnect();
        }

    }
}
