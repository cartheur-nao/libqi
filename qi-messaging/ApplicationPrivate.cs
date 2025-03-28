using System.Runtime.InteropServices;

namespace qi
{
    namespace Messaging
    {
        unsafe class ApplicationPrivate
        {
            [DllImport("qimessaging.dll")]
            public static extern qi_application_t* qi_application_create(int* argc, char** argv);

            [DllImport("qimessaging.dll")]
            public static extern void qi_application_stop(qi_application_t* app);

            [DllImport("qimessaging.dll")]
            public static extern void qi_application_run(qi_application_t* app);

            public ApplicationPrivate(string[] args)
            {
                int ac = 0;//args.Length;
                //char** argv = Convertor.ToCharPtr(args);
                _application_t = qi_application_create(&ac, null);
                //args = QiMessaging.Convertor.ToDotNet(ac, argv);
            }

            public void Run()
            {
                qi_application_run(_application_t);
            }

            ~ApplicationPrivate()
            {
                qi_application_stop(_application_t);
            }

            private qi_application_t* _application_t;
        }
    }
}
