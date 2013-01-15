NFK Server Launcher
==============

Console Launcher for NFK dedicated server ([Need For Kill](http://needforkill.ru)).

![](http://habrastorage.org/storage2/5cd/eba/5b0/5cdeba5b06d7aa5e9537c8350e33a36b.png)


## Features
* Console interface (native for dedicated)
* Realtime NFK server logging 
* Autorestart NFK server when it crash
* Set processor affinity to NFK server process
* It can be installed as a windows service


## Requirements
* [.NET Framework 3.5](http://www.microsoft.com/en-us/download/details.aspx?id=21) (but it can be compiled with >=2.0)



### Config

You can define custom variables in `nfkservice.exe.config`:
* `ServerExeFile` - path to server.dat (nfk dedicated server executable)
* `ExeParameters` - parameters to start nfk
* `LogFile` - realtime log from nfk
* `ProcessorAffinity` - set processor affinity mask to the server process ([hex value]( http://msdn.microsoft.com/en-us/library/ms187104.aspx))
* `ServiceName` - windows service name, used on install/uninstall only

if config file doesn't exist, then default values will be used.



### Install as a windows service

Just run launcher with one of the parameters:

* `nfkservice.exe /i` or `/install` - install new service
* `nfkservice.exe /u` or `/uninstall` - uninstall exist service
