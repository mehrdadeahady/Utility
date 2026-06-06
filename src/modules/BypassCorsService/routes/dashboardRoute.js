const path = require('path');
const express = require('express');
const router = express.Router();
const logService = require('../services/logService');

router.get('/', (req, res) => {
  return res.sendFile(path.join(__dirname, '../dashboard/index.html'));
});

router.get('/stats', async (req, res) => {
  try {
    const stats = await logService.getStats();
    return res.json(stats);   // IMPORTANT: return
  } catch (e) {
    console.error('Stats error:', e);
    return res.status(500).json({ error: 'Failed to load stats' });
  }
});

router.get('/recent', async (req, res) => {
  try {
    const logs = await logService.getRecentLogs();
    return res.json(logs);    // IMPORTANT: return
  } catch (e) {
    console.error('Recent logs error:', e);
    return res.status(500).json({ error: 'Failed to load logs' });
  }
});

module.exports = router;
