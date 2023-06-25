import subprocess
import threading

def read_output(server):
    while True:
        line = server.stdout.readline()
        if not line:
            break
        print(line.strip())


def write_input(server):
    while True:
        line = input()
        if not line:
            break
        server.stdin.write(line + '\n')
        server.stdin.flush()


def main():
    server = subprocess.Popen(
        ['java', '-Xmx1024M', '-Xms1024M', '-jar', 'server.jar', 'nogui'],
        stdin=subprocess.PIPE,
        stdout=subprocess.PIPE,
        universal_newlines=True
    )

    print('Starting server...')

    threading.Thread(target=read_output, args=(server,)).start()
    threading.Thread(target=write_input, args=(server,)).start()


if __name__ == '__main__':
    main()
