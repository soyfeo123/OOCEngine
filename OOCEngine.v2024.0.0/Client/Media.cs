using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOCEngine
{
    public class OOCEncodedVideo
    {
        public int frameRate;
        public string[] frames;
    }

    public class OOCVideo
    {
        public int frameRate;
        public IntPtr[] frames;
    }

    public static class Media
    {
        public static void PlayVideo()
        {

        }
    }
}
