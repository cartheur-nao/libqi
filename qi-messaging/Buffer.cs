using System.Runtime.InteropServices;

namespace qi
{
    namespace Messaging
    {
        public class Buffer : SafeBuffer
        {
            public Buffer(bool ownsHandle = true)
                : base(ownsHandle)
            {  }

            protected override bool ReleaseHandle()
            {
                return true;
            }
        }
    }
}
