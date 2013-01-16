NFK Server Launcher
==============

Console Launcher for NFK dedicated server ([Need For Kill](http://needforkill.ru)).

![](http://habrastorage.org/storage2/5cd/eba/5b0/5cdeba5b06d7aa5e9537c8350e33a36b.png)


## Features
* Console interface (native for dedicated)
* NFK server logging in realtime with timestamp
* Autorestart NFK server when it crashed
* Set processor affinity and priority to NFK server process
* Easy to install as a windows service
* Autoupdate for NFK server files (optional)

### Requirements
* [.NET Framework 3.5](http://www.microsoft.com/en-us/download/details.aspx?id=21) (but it can be compiled with version >=2.0)



### Config

There are custom parameters can be defined in `nfkservice.exe.config`:
* `Title` - console title (it also used for windows service description)
* `ServerExeFile` - path to server.dat (nfk dedicated server executable)
* `ExeParameters` - parameters to start nfk server
* `LogFile` - realtime log from nfk server
* `ProcessorAffinity` - set [processor affinity](http://bit.ly/ZWkGpM) mask to the server process (hex value)
* `ProcessorPriority` - set [processor priority](http://bit.ly/Urr7Rn) to the server process (numeric value 0-5)
* `ServiceName` - windows service name, used on install/uninstall event only
* `AutoUpdate` - enable/disable autoupdate
* `AutoUpdateUrl` - url to xml file that contain information with latest server version and files to update

if config file doesn't exist, then default values are used.



### Installation as a windows service

Just run launcher with one of the parameters:

* `/install` or `/i` - install new service
* `/uninstall` or `/u` - uninstall exist service

For example: `nfkservice.exe /i`

'Note: administrator rights are required'
