// core/createHandler.js
const parseURL = require('../utils/parseURL');
const isValidHost = require('../utils/isValidHost');
const applyCORS = require('../utils/applyCORS');
const proxyRequest = require('./proxyRequest');
const getProxyForUrl = require('proxy-from-env').getProxyForUrl;

const storage = require('../storage/storage');

module.exports = function createHandler(options, proxy) {
  const cfg = {
    getProxyForUrl,
    maxRedirects: 5,
    originBlacklist: [],
    originWhitelist: [],
    removeHeaders: [],
    setHeaders: {},
    corsMaxAge: 0,
    ...options
  };

  return function (req, res) {
    // Capture raw request body
    let rawBody = '';
    req.on('data', chunk => rawBody += chunk);
    req.on('end', () => req.rawBody = rawBody);

    // Save request immediately (async)
    const requestIdPromise = storage.saveRequest({
      method: req.method,
      url: req.url,
      headers: req.headers,
      body: rawBody || null
    });

    req.corsState = {
      getProxyForUrl: cfg.getProxyForUrl,
      maxRedirects: cfg.maxRedirects,
      corsMaxAge: cfg.corsMaxAge
    };

    const corsHeaders = applyCORS({}, req);

    if (req.method === 'OPTIONS') {
      res.writeHead(200, corsHeaders);
      return res.end();
    }

    const rawUrl = req.url.slice(1);
    const location = parseURL(rawUrl);

    if (!location || !isValidHost(location.hostname)) {
      res.writeHead(400, corsHeaders);
      return res.end('Invalid target URL');
    }

    cfg.removeHeaders.forEach((h) => delete req.headers[h]);
    Object.assign(req.headers, cfg.setHeaders);

    const isHttps =
      req.connection.encrypted ||
      /^https/i.test(req.headers['x-forwarded-proto'] || '');

    req.corsState.location = location;
    req.corsState.proxyBaseUrl = `${isHttps ? 'https' : 'http'}://${req.headers.host}`;

    // Pass requestIdPromise to proxyRequest
    proxyRequest(req, res, proxy, requestIdPromise);
  };
};
