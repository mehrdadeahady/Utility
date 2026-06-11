const applyCORS = require('../utils/applyCORS');

module.exports = function onProxyResponse(proxyRes, req, res) {
  const state = req.corsState;

  delete proxyRes.headers['set-cookie'];
  delete proxyRes.headers['set-cookie2'];

  proxyRes.headers['x-final-url'] = state.location.href;
  applyCORS(proxyRes.headers, req);

  return true;
};
