= API скрипт для установки на сервер =

Позволяет запускать, останавливать NFK серверы, изменять/отправлять определенные файлы - через простой API интерфейс.


== УСТАНОВКА ==

1) Сначала необходимо установить NFK сервер в качестве Windows службы через nfkservice.exe /i
Прежде необходимо сделать отдельную папку для серверов NFK и папку каждого сервера назвать в виде номера порта, с которым он запускается. 
Например, папка "servers", внутри неё папки с серверами: "29991", "29992"

Имена служб NFK необходимо назвать по номеру порта, с префиксом "NFK_" (меняется в конфиге nfkservice.xml).
Например, "NFK_29991", "NFK_29992"

2) Установить права этим папкам на чтение/запись пользователю, под которым запущен веб сервер. 
Например, для IIS в свойствах папки необходимо дать права пользователю IUSR

3) Установить права на запуск для script\control.cmd

3) Установить права аккаунту веб сервера на управление NFK службами. Сделать это можно через утилиту script\subinacl.exe (запускать с правами администратора)

Параметры запуска утилиты:
	subinacl.exe /service ИМЯСЛУЖБЫ /grant=ИМЯПОЛЬЗОВАТЕЛЯ=F

Например, чтобы добавить права службе NFK_29991 для IIS, нужно дать разрешение на её управление пользователю IUSR:
	subinacl.exe /service NFK_29991 /grant=IUSR=F


== НАСТРОЙКА ==

Переименовать config.php.example -> config.php:

	$ApiKey - ключ для доступа к скрипту извне (его же нужно будет указать со стороны клиентского интерфейса)

	$ServerPath - путь к папке "servers" со слешами в конце
		Например, 'c:\\servers\\nfk\\'

	$Servers - массив с номерами портов установленных серверов.
		Например, array(29991, 29992);
		
		
# Docker

```
docker run --name nfk_control --privileged \
	-v /home/nfk:/usr/local/nfk \
	-v /var/run/docker.sock:/var/run/docker.sock \
	-e PORTS_ENUM=29996,29997,29998 \
	-e APIKEY=CHANGE_ME \
	-p 8081:80 \
	harpywar/nfkcontrol
```

## docker-compose.yml

```
version: '3'

services:
  nfk_control:
    image: harpywar/nfkcontrol
    container_name: nfk_control
    privileged: true
    ports:
      - 8081:80
    environment:
      - "PORTS_ENUM=29996,29997,29998"
      - "APIKEY=CHANGE_ME"
    volumes:
      - /home/nfk:/usr/local/nfk
      - /var/run/docker.sock:/var/run/docker.sock
```
