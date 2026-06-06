// storage/storage.js
const { Pool } = require('pg');
const ensureDatabaseExists = require('./initDatabase');

class Storage {
  constructor() {
    const connectionString = process.env.DATABASE_URL;
    const dbName = connectionString.split('/').pop();

    this.ready = ensureDatabaseExists(connectionString, dbName)
      .then(() => {
        this.pool = new Pool({ connectionString });
        return this._initDb();
      })
      .catch(err => {
        console.error("Failed to initialize database:", err);
      });
  }

  async _initDb() {
    const client = await this.pool.connect();
    try {
      await client.query(`
        CREATE TABLE IF NOT EXISTS requests (
          id SERIAL PRIMARY KEY,
          method TEXT,
          url TEXT,
          headers JSONB,
          body TEXT,
          created_at TIMESTAMP DEFAULT NOW()
        );
      `);

      await client.query(`
        CREATE TABLE IF NOT EXISTS responses (
          id SERIAL PRIMARY KEY,
          request_id INTEGER REFERENCES requests(id),
          status INTEGER,
          headers JSONB,
          body TEXT,
          created_at TIMESTAMP DEFAULT NOW()
        );
      `);

      console.log("Tables initialized.");
    } finally {
      client.release();
    }
  }

  async saveRequest(data) {
    await this.ready;
    const result = await this.pool.query(
      `INSERT INTO requests (method, url, headers, body)
       VALUES ($1, $2, $3, $4)
       RETURNING id`,
      [
        data.method,
        data.url,
        JSON.stringify(data.headers),
        data.body
      ]
    );
    return result.rows[0].id;
  }

  async saveResponse(requestId, data) {
    await this.ready;
    await this.pool.query(
      `INSERT INTO responses (request_id, status, headers, body)
       VALUES ($1, $2, $3, $4)`,
      [
        requestId,
        data.status,
        JSON.stringify(data.headers),
        data.body
      ]
    );
  }
}

const storageInstance = new Storage();
module.exports = storageInstance;
