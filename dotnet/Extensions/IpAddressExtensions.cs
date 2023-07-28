﻿namespace LancachePrefill.Common.Extensions
{
    public static class IpAddressExtensions
    {
        /// <summary>
        /// An extension method to determine if an IP address is a private address, as specified in RFC1918
        /// </summary>
        /// <param name="toTest">The IP address that will be tested</param>
        /// <returns>Returns true if the IP is a private address, false if it isn't private</returns>
        public static bool IsPrivateAddress(this IPAddress toTest)
        {
            if (IPAddress.IsLoopback(toTest))
            {
                return true;
            }

            byte[] bytes = toTest.GetAddressBytes();
            switch (bytes[0])
            {
                case 10:
                    return true;
                case 172:
                    return bytes[1] < 32 && bytes[1] >= 16;
                case 192:
                    return bytes[1] == 168;
                default:
                    return false;
            }
        }
    }
}
