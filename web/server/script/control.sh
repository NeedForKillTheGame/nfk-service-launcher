#!/bin/bash


if [[ $1 == "query" ]]
then
   systemctl status $2.service
fi

if [[ $1 == "stop" ]]
then
   sudo systemctl stop $2.service && systemctl status $2.service
fi

if [[ $1 == "start" ]]
then   
   sudo systemctl start $2.service && systemctl status $2.service
fi