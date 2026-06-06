// core/proxyRequest.js
const http = require('http');
const https = require('https');

const storage = require('../storage/storage');

module.exports = function proxyRequest(req, res, proxy, requestIdPromise) {
  const target = req.corsState.location;

  const proxyReq = (target.protocol === 'https:' ? https : http).request(
    target.href,
    {
      method: req.method,
      headers: req.headers
    },
    (proxyRes) => {
      let chunks = [];

      proxyRes.on('data', (chunk) => chunks.push(chunk));

      proxyRes.on('end', () => {
        const buffer = Buffer.concat(chunks);

        // Detect binary by checking for null bytes
        const isBinary = buffer.includes(0x00);

        // Encode safely
        const bodyString = isBinary
          ? buffer.toString('base64')
          : buffer.toString('utf8');

        // Save response AFTER requestId resolves
        requestIdPromise.then(requestId => {
          storage.saveResponse(requestId, {
            status: proxyRes.statusCode,
            headers: proxyRes.headers,
            body: bodyString,
            is_base64: isBinary
          });
        });

        // Send response to client
        res.writeHead(proxyRes.statusCode, proxyRes.headers);

        if (isBinary) {
          // Send raw binary to client
          res.end(buffer);
        } else {
          res.end(bodyString);
        }
      });
    }
  );

  proxyReq.on('error', (err) => {
    res.writeHead(500);
    res.end('Proxy error: ' + err.message);
  });

  if (req.rawBody) {
    proxyReq.write(req.rawBody);
  }

  proxyReq.end();
};
