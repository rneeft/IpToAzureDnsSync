# MyIp Sync
Application that will sync your current IP Address with a A record domain in Azure

You can use the following docker compose to setup the image, in your local environment.

```yaml
services:
  ipazuresync:
    image: rneeft/ipazuresync:latest
    ports:
      - "100:8080"   # Maps host port 100 to container port 8080
      - "101:443"    # Maps host port 101 to container port 443
    environment:
      AzureDns__SubscriptionId: "0574a3b5-5e81-4710-94ee-80824c764a71"
      AzureDns__ResourceGroupName: "<your-azure-resource-group-hosting-your-dns>"
      AzureDns__DnsZoneName: "<your-azure-dns-zone>"
      AzureDns__RecordSetName: "<your-azure-dns-a-record>"
      Sync__DoSync: "true"
      Sync__Timeout: "00:01:00"
      AZURE_CLIENT_ID: "<your-app-azure-client-id>"
      AZURE_TENANT_ID: "<your-app-azure-tenant-id>"
      AZURE_CLIENT_SECRET: "<your-app-azure-client-secret>"
    restart: always  # Keeps the container running until manually stopped
``` 

## Health checks
Health checks are included on http://localhost:100/healtz