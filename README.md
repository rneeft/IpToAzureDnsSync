# MyIp Sync
Application that will sync your current IP Address with a A record domain in Azure

You can use the following docker compose to setup the image, in your local environment.

```yaml
version: "3.8"

services:
  myipservice:
    image: rneeft/myip:latest
    ports:
      - "8080:8080"
      - "443:443"
    environment:
      AzureDns__SubscriptionId: "0574a3b5-5e81-4710-94ee-80824c764a71"
      AzureDns__ResourceGroupName: "home"
      AzureDns__DnsZoneName: "kersenveld.nl"
      AzureDns__RecordSetName: "home"
      Sync__DoSync: "true" # set to false if you want to run the image in a what-if scenario.
      Sync__Timeout: "00:05:00"
    restart: always

``` 

## Health checks
Health checks are included on http://..../healtz