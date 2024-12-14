namespace LancachePrefill.Common
{
    /// <summary>
    /// Attempts to automatically resolve the Lancache's IP address, allowing users to be able to run the prefill on the same machine as the Lancache.
    ///
    /// Will automatically try to detect the Lancache through the poisoned DNS entries, however if that is not possible it will then check
    /// 'localhost' to see if the Lancache is available locally.  If the server is not available on 'localhost', then 172.17.0.1 will be checked to see if
    /// the prefill is running from a docker container
    /// </summary>
    public static class LancacheIpResolver
    {
        private static IAnsiConsole _ansiConsole;
        private static DetectedServer _detectedServer;

        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromMilliseconds(500) };

        public static async Task<string> ResolveLancacheIpAsync(IAnsiConsole ansiConsole, string cdnUrl)
        {
            _ansiConsole ??= ansiConsole;

            // Returned cached server if previously detected
            if (_detectedServer != null)
            {
                return _detectedServer.IpAddress.ToString();
            }

            await _ansiConsole.StatusSpinner().StartAsync("Detecting Lancache server...", async _ =>
            {
                _detectedServer = await DetectLancacheServerAsync(cdnUrl);
            });

            if (_detectedServer != null)
            {
                _ansiConsole.LogMarkupLine($"Detected Lancache server at {Cyan(_detectedServer.Url)} [[{MediumPurple(_detectedServer.IpAddress)}]]");
                return _detectedServer.IpAddress.ToString();
            }

            // If no server was detected, checks for common configuration issues
            await DetectPublicIpAsync(cdnUrl);
            await IsLancacheServerRunningAsync(cdnUrl);

            throw new LancacheNotFoundException("Unable to detect Lancache server!");
        }

        private static async Task<DetectedServer> DetectLancacheServerAsync(string cdnUrl)
        {
            // Tries to resolve poisoned DNS record, then localhost, then Docker's host, and finally the local machine
            var localMachineName = Dns.GetHostName();
            var possibleLancacheUrls = new List<string> { cdnUrl, "localhost", "172.17.0.1", localMachineName };

            foreach (var url in possibleLancacheUrls)
            {
                _ansiConsole.LogMarkupVerbose($"Checking for Lancache at {Cyan(url)}");
                //TODO make this do ipv6 correctly
                // Gets a list of ipv4 addresses, Lancache cannot use ipv6 currently
                var ipAddresses = (await Dns.GetHostAddressesAsync(url))
                    .Where(e => e.AddressFamily == AddressFamily.InterNetwork)
                    .ToArray();

                // If there are no private IPs, then continue onto the next url.  Lancache's IP must resolve to an RFC 1918 address
                if (!ipAddresses.Any(e => e.IsPrivateAddress()))
                {
                    continue;
                }

                // DNS hostnames can possibly resolve to more than one IP address (one-to-many), so we must check each one for a Lancache server
                foreach (var ip in ipAddresses)
                {
                    try
                    {
                        // If the IP resolves to a private subnet, then we want to query the Lancache server to see if it is actually there.
                        var response = await _httpClient.GetAsync(new Uri($"http://{ip}/lancache-heartbeat"));
                        if (response.Headers.Contains("X-LanCache-Processed-By"))
                        {
                            return new DetectedServer(url, ip);
                        }
                    }
                    catch (Exception e) when (e is HttpRequestException | e is TaskCanceledException)
                    {
                        // Target machine refused connection errors are to be expected if there is no Lancache at that IP address.
                        // We will be catching those exceptions as well as timeout exceptions, so we can try the next IP address.
                    }
                }
            }
            return null;
        }

        private static async Task DetectPublicIpAsync(string cdnUrl)
        {
            var ipAddresses = await Dns.GetHostAddressesAsync(cdnUrl);
            var resolvedIp = ipAddresses.First(e => e.AddressFamily == AddressFamily.InterNetwork);

            if (ipAddresses.Any(e => e.IsPrivateAddress()))
            {
                return;
            }

            // If a public IP address is resolved, then it means that the Lancache is not configured properly, and we would end up downloading from the internet.
            _ansiConsole.MarkupLine(LightYellow($" Warning!  {White(cdnUrl)} is resolving to a public IP address {Cyan($"({resolvedIp})")}.\n" +
                                                        $" {White(cdnUrl)} must resolve to a private RFC1918 address to work correctly.\n" +
                                                         " Please check your Lancache configuration and try again.\n"));

            throw new LancacheNotFoundException($"Lancache server is resolving to a public IP : {resolvedIp}");
        }

        private static async Task IsLancacheServerRunningAsync(string cdnUrl)
        {
            var ipAddresses = await Dns.GetHostAddressesAsync(cdnUrl);
            var resolvedIp = ipAddresses.First(e => e.AddressFamily == AddressFamily.InterNetwork);

            try
            {
                // Attempting to see if the Lancache server at the resolved IP is running
                await _httpClient.GetAsync(new Uri($"http://{resolvedIp}/lancache-heartbeat"));
            }
            catch (Exception e) when (e is HttpRequestException | e is TaskCanceledException)
            {
                _ansiConsole.MarkupLine(Red($" Error!  {White(cdnUrl)} is resolving to a private IP address {Cyan($"({resolvedIp})")},\n" +
                                            " however no Lancache can be found at that address.  The Lancache server may possibly not be running." +
                                            " Please check your configuration, and try again.\n"));
                throw new LancacheNotFoundException($"No Lancache server detected at {resolvedIp}");
            }
        }

        private sealed class DetectedServer
        {
            public string Url { get; }
            public IPAddress IpAddress { get; }

            public DetectedServer(string url, IPAddress ipAddress)
            {
                Url = url;
                IpAddress = ipAddress;
            }
        }
    }
}