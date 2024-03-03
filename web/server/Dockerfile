# harpywar/nfkcontrol

FROM docker:latest as docker-cli

FROM php:7.3-apache

# copy the Docker CLI binaries from the docker-cli stage
COPY --from=docker-cli /usr/local/bin/docker /usr/local/bin/docker


COPY . /var/www/html

VOLUME ["/usr/local/nfk"]

ENV PORTS_ENUM=29998
ENV APIKEY=1d4060d5-84a6-4d4b-9b31-c7cb3206f317

WORKDIR /var/www/html

RUN chown -R :www-data .

RUN chmod +x /var/www/html/script/control.sh
RUN chmod +x /var/www/html/entrypoint.sh

CMD ["/var/www/html/entrypoint.sh"]