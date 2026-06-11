const logService = require('../services/logService');

module.exports = (req, res, next) => {
  const originalWriteHead = res.writeHead;

  res.writeHead = function (statusCode, headers) {
    res.__statusCode = statusCode;
    res.__headers = headers || {};
    return originalWriteHead.apply(this, arguments);
  };

  res.on('finish', async () => {
    try {
      await logService.logRequest({
        method: req.method,
        url: req.originalUrl || req.url,
        status: res.__statusCode || res.statusCode,
        requestHeaders: req.headers,
        responseHeaders: res.__headers || {}
      });
    } catch (e) {
        // PostgreSQL not installed or not running
        if (e.code === 'ECONNREFUSED') {
          console.error(
            'Logging disabled: PostgreSQL is not running or not installed.\n' +
            'Install PostgreSQL and ensure the service is running to enable request logging.'
          );
          return;
        }

        // Database does not exist
        if (e.code === '3D000') {
          console.error(
            'Logging disabled: The PostgreSQL database does not exist.\n' +
            'Create the database or update DATABASE_URL.'
          );
          return;
        }

        // Table does not exist
        if (e.code === '42P01') {
          console.error(
            'Logging disabled: The "logs" table does not exist.\n' +
            'Create it using your database tool or SQL migration, or wait for the database to be initialized.'
          );
          return;
        }

        // Wrong password / authentication
        if (e.code === '28P01') {
          console.error(
            'Logging disabled: PostgreSQL authentication failed.\n' +
            'Check your username/password in DATABASE_URL.'
          );
          return;
        }

        // Generic fallback
        console.error('Failed to log request:', {
          message: e.message,
          code: e.code,
          detail: e.detail,
          hint: e.hint
        });
      }

  });

  next();
};
