require('dotenv').config();
const express = require('express');
const path = require('path');
const proxyRoute = require('./routes/proxyRoute');
const dashboardRoute = require('./routes/dashboardRoute');
const logger = require('./middleware/logger');
const swaggerUi = require('swagger-ui-express');
const YAML = require('yamljs');
const swaggerDocument = YAML.load('./swagger.yaml');
const storage = require('./storage/storage');
const reportsRoute = require('./routes/reportsRoute');

const app = express();

app.use(express.json());
app.use(logger);

// Serve Swagger UI BEFORE proxy
app.use('/docs', swaggerUi.serve, swaggerUi.setup(swaggerDocument));

// Serve static files BEFORE proxy
app.use('/dashboard-ui', express.static(path.join(__dirname, 'dashboard')));

app.use('/clients-ui', express.static(path.join(__dirname, 'clients')));

// Dashboard API BEFORE proxy
app.use('/dashboard', dashboardRoute);

// Reports API BEFORE proxy
app.use('/reports', reportsRoute);

// Proxy LAST (so it doesn't override everything)
app.use('/', proxyRoute);

const PORT = process.env.PORT || 8080;
app.listen(PORT, () => {
  const url = `http://localhost:${PORT}`;
  console.log(`Bypass CORS Proxy running on port ${PORT}`);
  console.log(`Open in browser: ${url}`);
  console.log("DATABASE_URL:", process.env.DATABASE_URL);
});

