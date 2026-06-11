module.exports = function applyCORS(headers, req) {
  headers['access-control-allow-origin'] = '*';
  headers['access-control-expose-headers'] = Object.keys(headers).join(',');

  if (req.headers['access-control-request-method']) {
    headers['access-control-allow-methods'] =
      req.headers['access-control-request-method'];
  }

  if (req.headers['access-control-request-headers']) {
    headers['access-control-allow-headers'] =
      req.headers['access-control-request-headers'];
  }

  return headers;
};
