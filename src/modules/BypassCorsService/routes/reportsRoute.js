const express = require('express');
const router = express.Router();
const storage = require('../storage/storage');

// Correct: handle /reports and /reports/
router.get('/', async (req, res) => {
  try {
    const db = storage.pool;

    const requests = await db.query(`
      SELECT * FROM requests
      ORDER BY created_at DESC
      LIMIT 500
    `);

    const responses = await db.query(`
      SELECT * FROM responses
    `);

    const responseMap = new Map();
    responses.rows.forEach(r => responseMap.set(r.request_id, r));

    const merged = requests.rows.map(reqRow => ({
      ...reqRow,
      response: responseMap.get(reqRow.id) || null
    }));

    res.json(merged);
  } catch (err) {
    console.error('Error fetching reports:', err);
    res.status(500).json({ error: err.message });
  }
});

module.exports = router;
