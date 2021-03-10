using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace AspNetMvcSample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ConfigureAuth(app);
        }
    }
}