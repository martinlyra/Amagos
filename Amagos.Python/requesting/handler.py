from http.server import BaseHTTPRequestHandler

class RequestHandler(BaseHTTPRequestHandler):
    def do_POST(self):
        print(self.path)
        print(self.headers)
        length = int(self.headers.get("Content-Length"))
        content = self.rfile.read(length)
        print(content)

        self.send_response(200, "OK")
        self.end_headers()
        self.wfile.write(bytes(content))
        