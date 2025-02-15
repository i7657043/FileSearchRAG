FROM node:18-alpine AS build

WORKDIR /app

COPY package*.json yarn.lock ./

RUN yarn

COPY . .

ARG REACT_APP_API_BASE_URL
ENV REACT_APP_API_BASE_URL=$REACT_APP_API_BASE_URL

RUN yarn build

FROM nginx:alpine

WORKDIR /usr/share/nginx/html

RUN rm -rf ./*

COPY --from=build /app/build .

EXPOSE 80

#"daemon off;" forces nginx to run in foreground, keeping container alive
CMD ["nginx", "-g", "daemon off;"]
