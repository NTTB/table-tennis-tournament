const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:33524';

const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/hubs"
   ],
    target: target,
    secure: false,
    changeOrigin: true,
    ws: true,
  }
]

module.exports = PROXY_CONFIG;
