FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY css /usr/share/nginx/html/css
COPY js /usr/share/nginx/html/js
COPY js/config.js /usr/share/nginx/html/js/config.js
COPY fonts /usr/share/nginx/html/fonts
COPY images /usr/share/nginx/html/images

COPY pages /usr/share/nginx/html/pages

COPY nginx.conf /etc/nginx/conf.d/default.conf

RUN chown -R nginx:nginx /usr/share/nginx/html && \
    chmod -R 755 /usr/share/nginx/html

EXPOSE 80