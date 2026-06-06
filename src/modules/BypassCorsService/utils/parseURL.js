const url = require('url');

module.exports = function parseURL(raw) {
  try {
    if (!/^https?:\/\//i.test(raw)) raw = 'http://' + raw;
    return url.parse(raw);
  } catch {
    return null;
  }
};
