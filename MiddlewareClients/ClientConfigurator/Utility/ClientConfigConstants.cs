using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConfigurator.Utility
{
    /// <summary>
    /// Most constants should be here. Move them as you need them.
    /// </summary>
    public static class ClientConfigConstants
    {
        public static string AppGUID = "BBF39D55-1125-49ab-ACF4-1A7B19F95D82";
        public static string AppName = "MiddlewareConfigApp";

        public static string CLIENT_CONFIG_CATEGORY = "ClientConfig general";


        // Client category prefix
        public static string CLIENT_CATEGORY_NAME_PREFIX = "Client_";

        // Parameter names
        public static string CM_PORT_PARAM_NAME = "CALCMan Port";
        public static string CM_HOSTNAME_PARAM_NAME = "CALCMan Hostname";

        public static string CLIENT_GUID_PARAMNAME = "ClientGuid";
        public static string CALCMAN_CLIENT_HOSTNAME_PARAMNAME = "CALCMan client hostname";

        // ImageGen IG_
        public static string IG_PORT_PARAM_NAME = "ImageGen Port";

    }
}
