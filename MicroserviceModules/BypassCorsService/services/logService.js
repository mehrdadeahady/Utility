const db = require('./db');

// Ensure logs table exists
(async () => {
  await db.query(`
    CREATE TABLE IF NOT EXISTS logs (
      id SERIAL PRIMARY KEY,
      method TEXT,
      url TEXT,
      status INT,
      request_headers JSONB,
      response_headers JSONB,
      created_at TIMESTAMP DEFAULT NOW()
    );
  `);
  console.log("Logs table ready.");
})();

// Insert a log entry
exports.logRequest = async (entry) => {
  const query = `
    INSERT INTO logs (method, url, status, request_headers, response_headers, created_at)
    VALUES ($1, $2, $3, $4, $5, NOW())
  `;

  await db.query(query, [
    entry.method,
    entry.url,
    entry.status,
    JSON.stringify(entry.requestHeaders),
    JSON.stringify(entry.responseHeaders)
  ]);
};

// Dashboard stats
exports.getStats = async () => {
  const result = await db.query(`
    SELECT 
      COUNT(*) AS total_requests,
      COUNT(*) FILTER (WHERE status >= 400) AS errors,
      MAX(created_at) AS last_request
    FROM logs
  `);

  return result.rows[0];
};

// Recent logs
exports.getRecentLogs = async () => {
  const result = await db.query(`
    SELECT * FROM logs ORDER BY created_at DESC LIMIT 50
  `);

  return result.rows;
};
