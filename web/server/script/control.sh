#!/bin/bash

service_name=nfk_$2

if [[ $1 == "query" ]]
then
   docker ps -a --format "{{.Names}}: {{.Status}}" | grep $service_name
fi

if [[ $1 == "stop" ]]
then
   docker stop $service_name && docker ps -a --format "{{.Names}}: {{.Status}}" | grep $service_name
fi

if [[ $1 == "start" ]]
then   
   docker start $service_name && docker ps -a --format "{{.Names}}: {{.Status}}" | grep $service_name
fi