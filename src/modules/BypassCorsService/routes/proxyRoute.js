const express = require('express');
const router = express.Router();
const httpProxy = require('http-proxy');
const createHandler = require('../core/createHandler');

const proxy = httpProxy.createServer({ xfwd: true });
const handler = createHandler({}, proxy);

router.use((req, res) => {
  handler(req, res);
});

module.exports = router;
