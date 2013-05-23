using System.Net;

namespace Aaron.Core.Services.Directory
{
    /// <summary>
    /// Country lookup helper
    /// </summary>
    public partial interface IGeoCountryLookup
    {
        string LookupCountryCode(string str);

        string LookupCountryCode(IPAddress addr);

        string LookupCountryName(string str);

        string LookupCountryName(IPAddress addr);
    }
}