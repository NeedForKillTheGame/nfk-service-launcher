NFK Server Launcher
==============

Console Launcher for NFK dedicated server ([Need For Kill](http://needforkill.ru)).

![](http://habrastorage.org/storage2/62a/fa1/3eb/62afa13ebd5c19015dc48aa48501bc84.png)


## Features
* Console interface (native for dedicated)
* Autorestart NFK server when it crashed
* Set processor affinity and priority to NFK server process
* Easy to install as a windows service
* Autoupdate NFK server files ([optional](https://github.com/HarpyWar/nfk-service-launcher/wiki/%D0%90%D0%B2%D1%82%D0%BE%D0%BE%D0%B1%D0%BD%D0%BE%D0%B2%D0%BB%D0%B5%D0%BD%D0%B8%D0%B5))

### Requirements
* [.NET Framework 3.5](http://www.microsoft.com/en-us/download/details.aspx?id=21) (but it can be compiled with version >=2.0)



### Config

There are custom parameters can be defined in `nfkservice.xml`:
* `Title` - console title (it also used for windows service description)
* `ServerExeFile` - path to server.dat (nfk dedicated server executable)
* `ExeParameters` - parameters to start nfk server
* `LogFile` - realtime.log from nfk server
* `ProcessorAffinity` - set [processor affinity](http://bit.ly/ZWkGpM) mask to the server process (hex value)
* `ProcessorPriority` - set [processor priority](http://bit.ly/Urr7Rn) to the server process (numeric value 0-5)
* `ServiceName` - windows service name, used on install/uninstall event only
* `AutoUpdate` - enable/disable autoupdate
* `AutoUpdateUrl` - url to xml file that contain information with latest server version and files to update

Config file name must match exe file name. For example: `launcher.exe` and `launcher.xml`.

if config file doesn't exist, then default values are used.



### Installation as a windows service

Just run launcher with one of the parameters:

* `/install` or `/i` - install new service
* `/uninstall` or `/u` - uninstall exist service

For example: `nfkservice.exe /i`

*Note: Administrator rights are required.*

### Control using external scripts

* `sc stop NFK` - stop service
* `sc stop NFK` - start service
