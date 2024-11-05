﻿using System.Net;

namespace MyIp;

public class InMemoryDatabase
{
    public List<IPAddress> UsedIpAddresses { get; } = [];

    public DateTime LastRetrieval { get; set; }

    public IPAddress? CurrentIpAddress { get; set; }

    public IPAddress? InDnsZone { get; set; }
}
