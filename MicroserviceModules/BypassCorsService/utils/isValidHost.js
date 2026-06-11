const net = require('net');

module.exports = function isValidHost(host) {
  return net.isIPv4(host) || net.isIPv6(host) || /^[a-z0-9.-]+$/i.test(host);
};
