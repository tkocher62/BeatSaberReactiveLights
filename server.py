import socket
import sys
import threading
import os
import json
import time
import pigpio

# Initialize pigpio
pi = pigpio.pi()

# Define bright
bright = 255

# Pin number declarations
RED_PIN   = 24
GREEN_PIN = 22
BLUE_PIN  = 17

# Used for pulse calculations
step = 5
const = 0.4

# Total connected clients
clients = 0

# Current state of network
idle = True

# Set an individual light's brightness
def setLight(pin, brightness):
    pi.set_PWM_dutycycle(pin, brightness)

# Handles individual clients
def clientThread(client, addr):
    clientAddr = str(addr[0]) + ": " + str(addr[1])
    print("Client connected -", clientAddr)
    global clients
    global idle
    clients += 1
    idle = False
    threading.Thread(target=setLights, args=[0, 0, 0]).start()
    while client:
        try:
            data = client.recv(256)
            if len(data) > 0:
                threading.Thread(target=cmdHandle, args=(data,)).start()
        except:
            print("Dropped client -", clientAddr)
            clients -= 1
            if clients == 0:
                idle = True
                threading.Thread(target=cycle, args=[3]).start()
            break

# Pulses the specified color
def pulse(pin, value):
    b = value
    i = const / (value / step)
    print(pin)
    print(value)
    while b > 0:
        b -= 5
        setLight(pin, b)
        time.sleep(i)

# Cycles through all colors nicely
def cycle(step):
    colors = [255, 0, 0]
    while idle:
        for i in range(3):
            for _ in range(255 / step):
                if not idle:
                    break
                colors[i] -= step
                n = i + 1
                if n == 3:
                    n = 0
                colors[n] += step
                threading.Thread(target=setLights, args=[colors[0], colors[1], colors[2]]).start()
                time.sleep(.05)

# Sets the colors of all lights
def setLights(r, g, b):
    setLight(RED_PIN, r)
    setLight(GREEN_PIN, g)
    setLight(BLUE_PIN, b)

# Handle incoming network data
def cmdHandle(data):
    o = json.loads(data)
    t = o["type"]
    r = o["r"]
    g = o["g"]
    b = o["b"]
    print("COLOR " + t + ": (" + str(r) + "," + str(g) + "," + str(b) + ")")
    if t == "PULSE":
        if r == 200:
            threading.Thread(target=pulse, args=[RED_PIN, bright]).start()
        elif b == 210:
            threading.Thread(target=pulse, args=[BLUE_PIN, bright]).start()
        else:
            if r is not 0:
                threading.Thread(target=pulse, args=[RED_PIN, r]).start()
            if g is not 0:
                threading.Thread(target=pulse, args=[GREEN_PIN, g]).start()
            if b is not 0:
                threading.Thread(target=pulse, args=[BLUE_PIN, b]).start()
    elif t == "SET":
        setLight(RED_PIN, r)
        setLight(GREEN_PIN, g)
        setLight(BLUE_PIN, b)

# Startup
print("Starting server...")
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_address = ('192.168.1.222', 32750)
sock.bind(server_address)
sock.listen(1)
print("Server ready!")
print("Listening for clients...")
threading.Thread(target=cycle, args=[3]).start()

# Client listener
while True:
    (s, addr) = sock.accept()
    threading.Thread(target=clientThread, args=(s,addr)).start()