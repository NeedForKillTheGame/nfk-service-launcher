#!/bin/sh

# download maps from remote location to local
# run everyday at 6:00 AM
#

# root path to the maps directory
# make symlink there for each server in SERVER/maps -> /usr/local/nfk/maps
maps_path=/usr/local/nfk

cd $maps_path && wget -r --no-parent -nH -i "https://nfk.harpywar.com/api.php?action=maps&format=txt" >/dev/null
