import json
from aitextgen import aitextgen
from socketserver import TCPServer, BaseServer
from http.server import BaseHTTPRequestHandler

class RequestServer:
    def __init__(self, address = "", port = 6666) -> None:
        self.address = address
        self.port = port
        self.handler = None
        self.gpt2 = None

    def inject(self, gpt2: aitextgen) -> None:
        self.gpt2 = gpt2

    def create_handler_class(self, gpt2):
        class CustomRequestHandler(BaseHTTPRequestHandler):
            def __init__(
                self, 
                request: bytes, 
                client_address: 
                tuple[str, int], 
                server: BaseServer
                ) -> None:
                self.gpt2 = gpt2
                super().__init__(request, client_address, server)

            def do_POST(self):
                print(self.path)
                print(self.headers)
                length = int(self.headers.get("Content-Length"))
                content = self.rfile.read(length)
                print(content)

                data = json.loads(content)

                if data["RequestType"] == 0:
                    self.do_GPT2(data["Content"])
                else:
                    self.send_response(200, "OK")
                    self.end_headers()
                    self.wfile.write(bytes(content))

            def do_GPT2(self, prompt: str):
                output = self.gpt2.generate(
                    prompt=prompt, 
                    max_length=64,
                    temperature=0.9,
                    return_as_list=True
                )[0]

                output = output.replace(prompt, "")

                response_data = {
                    "result": output
                }
                http_payload = json.dumps(response_data)

                self.send_response(200, "OK")
                self.end_headers()
                self.wfile.write(bytes(http_payload, encoding="utf8"))
                
        return CustomRequestHandler
    
    def run(self) -> None:
        handler = self.create_handler_class(self.gpt2)
        with TCPServer((self.address, self.port), handler) as server:
            print("serving at port ", self.port)
            server.serve_forever()
