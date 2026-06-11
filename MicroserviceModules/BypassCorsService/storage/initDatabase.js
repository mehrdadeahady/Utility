// storage/initDatabase.js
const { Client } = require('pg');

let dbInitPromise = null;

async function ensureDatabaseExists(connectionString, dbName) {
  // Prevent multiple parallel calls
  if (dbInitPromise) return dbInitPromise;

  dbInitPromise = (async () => {
    const adminClient = new Client({
      connectionString: connectionString.replace(`/${dbName}`, '/postgres')
    });

    await adminClient.connect();

    const result = await adminClient.query(
      `SELECT 1 FROM pg_database WHERE datname = $1`,
      [dbName]
    );

    if (result.rowCount === 0) {
      console.log(`Database "${dbName}" does not exist. Creating...`);
      await adminClient.query(`CREATE DATABASE ${dbName}`);
      console.log(`Database "${dbName}" created.`);
    } else {
      console.log(`Database "${dbName}" already exists.`);
    }

    await adminClient.end();
  })();

  return dbInitPromise;
}

module.exports = ensureDatabaseExists;
