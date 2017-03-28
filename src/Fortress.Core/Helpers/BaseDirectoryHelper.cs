using System;

namespace Castle.Helpers
{
    public static class BaseDirectoryHelper
    {
        public static string BaseDirectory
        {
            get
            {
#if FEATURE_APPDOMAIN
                return AppDomain.CurrentDomain.BaseDirectory;
#else
                return AppContext.BaseDirectory;
#endif
            }
        }
    }
}
