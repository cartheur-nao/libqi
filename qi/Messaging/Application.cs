namespace qi
{
    namespace Messaging
    {
        public class Application
        {
            public Application(string[] args)
            {
                _applicationPrivate = new ApplicationPrivate(args);
            }

            public void Run()
            {
                _applicationPrivate.Run();
            }

            private ApplicationPrivate _applicationPrivate;
        }
    }
}
