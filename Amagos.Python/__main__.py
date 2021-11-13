from aitextgen import aitextgen

from requesting.server import RequestServer

if __name__ == "__main__":
    gpt2 = aitextgen()

    server = RequestServer()
    server.inject(gpt2)
    server.run()
