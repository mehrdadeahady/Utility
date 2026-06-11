🚀 BypassCorsService

A modular, production‑ready CORS bypass proxy microservice built with Node.js and Express.
It forwards HTTP(S) requests to any target URL while injecting permissive CORS headers, allowing browsers to access resources that normally block cross‑origin requests.

This service includes:

    Request/response logging middleware

    PostgreSQL persistence

    Dashboard UI with analytics

    A built‑in test client for debugging

    Swagger/OpenAPI documentation

---

## 📁 Project Structure


```
BypassCorsService
.
│   .env
│   .gitignore
│   package-lock.json
│   package.json
│   README.md
│   server.js
│   swagger.yaml
│
├───clients
│       index.html
│
├───core
│       createHandler.js
│       onProxyResponse.js
│       proxyRequest.js
│
├───dashboard
│       dashboard.css
│       dashboard.js
│       index.html
│
├───middleware
│       logger.js
│
├───routes
│       dashboardRoute.js
│       proxyRoute.js
│       reportsRoute.js
│
├───services
│       db.js
│       logService.js
│
├───storage
│       initDatabase.js
│       storage.js
│
└───utils
        applyCORS.js
        isValidHost.js
        parseURL.js
```


---

## 🧰 Tech Stack

- **Node.js** — runtime  
- **Express.js** — routing & middleware  
- **http-proxy** — low‑level proxy engine  
- **PostgreSQL** — request/response logging  
- **pg** — PostgreSQL client  
- **Vanilla JS / HTML** — dashboard & test client  

---

🚀 Running the Service

1. Install dependencies

npm install

2. Set environment variables

export DATABASE_URL=postgres://user:pass@host:5432/dbname
export PORT=8080

3. Start the server

npm start

The service will run at:

http://localhost:8080

🌐 Client Usage

To use the API, prefix the target URL with the proxy URL.

Example:

http://localhost:8080/https://example.com

This fetches https://example.com while bypassing CORS.

📄 Auto‑CORS Snippet (Vanilla JavaScript)

(function() {
    var cors_api_host = 'localhost:8080';
    var cors_api_url = 'http://' + cors_api_host + '/';
    var slice = [].slice;
    var origin = window.location.protocol + '//' + window.location.host;
    var open = XMLHttpRequest.prototype.open;

    XMLHttpRequest.prototype.open = function() {
        var args = slice.call(arguments);
        var targetOrigin = /^https?:\/\/([^\/]+)/i.exec(args[1]);

        if (targetOrigin && targetOrigin[0].toLowerCase() !== origin &&
            targetOrigin[1] !== cors_api_host) {
            args[1] = cors_api_url + args[1];
        }

        return open.apply(this, args);
    };
})();

📄 Auto‑CORS Snippet (jQuery)

jQuery.ajaxPrefilter(function(options) {
    if (options.crossDomain && jQuery.support.cors) {
        options.url = 'http://localhost:8080/' + options.url;
    }
});

🧪 Test Client

Open:

clients/test-client.html

📊 Dashboard

UI:

http://localhost:8080/dashboard-ui

API:

    /dashboard/stats

    /dashboard/recent

📘 Using the Swagger / OpenAPI Documentation

Your project includes a full OpenAPI 3.0 specification (swagger.yaml) and an optional Swagger UI endpoint for interactive API exploration.
🔹 1. View the Swagger UI (Interactive Docs)

If you enabled Swagger UI in server.js Then open:

http://localhost:8080/docs

This display:

    Interactive API testing

    Schema visualization

    Automatic request/response examples

    Try‑it‑out buttons

🔹 2. View the Raw OpenAPI File

The full API specification is available in:

swagger.yaml

You can import this file into:

    Postman

    Insomnia

    SwaggerHub

    ReDoc

    Stoplight Studio

    VS Code OpenAPI plugins

🔹 3. Using the Proxy Endpoint via Swagger

The proxy endpoint is defined as:

GET /{targetUrl}
POST /{targetUrl}

To use it in Swagger UI:

    URL‑encode the target URL

    Paste it into the targetUrl field

Example:

/https://example.com/api

/https://example.com/api

Swagger UI will automatically send the request through your proxy.

🔹 4. Dashboard API in Swagger

Swagger also documents:

    Dashboard stats

    Recent logs

These endpoints return:

    Total requests

    Error count

    Last request timestamp

    Recent request logs

🔹 5. Regenerating or Extending the API Docs

You can extend the swagger.yaml file to include:

    Authentication

    Rate limiting

    Additional proxy options

    Admin endpoints