from aitextgen import aitextgen
from socketserver import TCPServer
from http.server import SimpleHTTPRequestHandler

from .handler import RequestHandler

class RequestServer:
    def __init__(self, address = "", port = 6666) -> None:
        self.address = address
        self.port = port
        self.handler = RequestHandler

    def inject(self, gpt2: aitextgen) -> None:
        self.gpt2 = gpt2
    
    def run(self) -> None:
        with TCPServer((self.address, self.port), self.handler) as server:
            print("serving at port ", self.port)
            server.serve_forever()
