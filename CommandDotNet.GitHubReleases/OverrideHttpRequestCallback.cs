using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommandDotNet.NewerReleasesAlerts
{
    public delegate Task<string> OverrideHttpRequestCallback(HttpClient client, Uri requestUri);
}