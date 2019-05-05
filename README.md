Resgrid Email Processor
===========================

Resgrid Email Processor is CLI application and service for importing emails from an on premises location and utilizing the Resgrid API to guarantee call creation.

*********

[![Build status](https://ci.appveyor.com/api/projects/status/github/resgrid/emailprocessor?svg=true)](https://ci.appveyor.com/api/projects/status/github/resgrid/emailprocessor)

About Resgrid
-------------
Resgrid is a software as a service (SaaS) logistics, management and communications platform for first responders, volunteer fire departments, career fire, EMS, Search and Rescue (SAR), public safety, HAZMAT, CERT, disaster response, etc.

[Sign up for your free Resgrid Account Today!](https://resgrid.com)

## System Requirements ##

* Windows 7 or newer
* .Net Framework 4.6.2
* 1.8Ghz Single Core Processor
* 8GB of RAM
* 2GB of Free Disk Space
* For Scanner audio a Scanner with an Audio Line Out i.e. [WS1065](https://amzn.to/2Kuck8k) is needed

## Configuration

```json
// settings.json
{
  "ApiUrl": "https://api.resgrid.com",
  "Username": "TEST",
  "Password": "TEST"
}
```

## Settings

### Settings.json Values
<table>
  <tr>
    <th>Setting</th>
    <th>Description</th>
  </tr>
  <tr>
    <td>ApiUrl</td>
    <td>
      The URL to talk to the Resgrid API (Services) for our hosted production system this is "https://api.resgrid.com"
    </td>
  </tr>
  <tr>
    <td>Username</td>
    <td>
      Resgrid system login Username that can create calls
    </td>
  </tr>
  <tr>
    <td>Password</td>
    <td>
      Resgrid system login Password for the Username above
    </td>
  </tr>
</table>

## Installation ##

You should download the latest sable release from our <a href="https://github.com/Resgrid/EmailProcessor/releases">Release page</a>.

## Notes ##


## Author's ##
* Shawn Jackson (Twitter: @DesignLimbo Blog: http://designlimbo.com)
* Jason Jarrett (Twitter: @staxmanade Blog: http://staxmanade.com)

## License ##
[Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0)

## Acknowledgments

Resgrid Relay makes use of the following OSS projects:

- Consolas released under the BSD 2-Clause license: https://github.com/rickardn/Consolas/blob/develop/LICENSE