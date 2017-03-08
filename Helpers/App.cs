using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Acft_Track
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