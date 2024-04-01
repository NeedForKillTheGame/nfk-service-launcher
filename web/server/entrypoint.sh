#!/bin/sh

cd /var/www/html

mv config.php.example config.php
sed -i "s/array('29991', '29992')/array($PORTS_ENUM)/" config.php
sed -i "s/1d4060d5-84a6-4d4b-9b31-c7cb3206f317/$APIKEY/" config.php

chown -R :www-data /usr/local/nfk

# download maps
git clone https://github.com/NeedForKillTheGame/nfk-maps /usr/local/nfk/maps

# add job to update maps
echo "* * * * * git pull /usr/local/nfk/maps" | crontab -


# Get the group ID of the Docker socket
DOCKER_SOCK_GID=$(stat -c '%g' /var/run/docker.sock)

# Get the group name associated with the Docker socket GID
DOCKER_SOCK_GROUP=$(getent group ${DOCKER_SOCK_GID} | cut -d: -f1)

# If a group with the Docker socket GID exists, add www-data to that group
if [ ! -z "${DOCKER_SOCK_GROUP}" ]; then
    usermod -aG ${DOCKER_SOCK_GROUP} www-data
else
    # If no group matches the GID, create one and add www-data to it
    groupadd -g ${DOCKER_SOCK_GID} dockerhost
    usermod -aG dockerhost www-data
fi


exec apache2-foreground