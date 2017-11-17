using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FontNameSpace
{
    public static class App
    {

#if DEBUG
        public static Boolean isDebugMode = true;
#else 
        public static Boolean isDebugMode = false;
#endif
    }
}