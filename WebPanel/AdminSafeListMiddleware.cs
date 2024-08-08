﻿using System.Net;
using WebPanel.Misc;

namespace WebPanel
{
    public class AdminSafeListMiddleware
    {
            private readonly RequestDelegate _next;
            private readonly ILogger<AdminSafeListMiddleware> _logger;
            private readonly byte[][] _safelist;

            public AdminSafeListMiddleware(
                RequestDelegate next,
                ILogger<AdminSafeListMiddleware> logger,
                string safelist)
            {
                var ips = safelist.Split(';');
                _safelist = new byte[ips.Length][];
                for (var i = 0; i < ips.Length; i++)
                {
                Console.WriteLine(IPAddress.Parse(ips[i]));
                    _safelist[i] = IPAddress.Parse(ips[i]).GetAddressBytes();
                }

                _next = next;
                _logger = logger;
            }

            public async Task Invoke(HttpContext context)
            {
                if (context.Request.Method == HttpMethod.Get.Method)
                {
                    var remoteIp = context.Connection.RemoteIpAddress;
                    _logger.LogDebug("Request from Remote IP address: {RemoteIp}", remoteIp);

                    var bytes = remoteIp?.GetAddressBytes();
                    var badIp = true;

                    foreach (var address in _safelist)
                    {
                        if(bytes != null)
                        if (address.SequenceEqual(bytes))
                        {
                            badIp = false;
                            break;
                        }
                    }

                    if (badIp)
                    {
                        _logger.LogWarning(
                            "Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return;
                    }
                }

            await _next.Invoke(context);
            }
        
    }
}
