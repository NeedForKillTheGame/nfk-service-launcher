#!/bin/bash

service_name=nfk_$2.service

if [[ $1 == "query" ]]
then
   systemctl status $service_name
fi

if [[ $1 == "stop" ]]
then
   sudo systemctl stop $service_name && systemctl status $service_name
fi

if [[ $1 == "start" ]]
then   
   sudo systemctl restart $service_name && systemctl status $service_name
fi