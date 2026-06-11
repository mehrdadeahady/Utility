from dashboard.server.app import app

if __name__ == "__main__":
    print("Starting Dashboard on http://localhost:8080")
    app.run(port=8080, debug=True)

