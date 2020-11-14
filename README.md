# BeatSaberReactiveLights

This program was a project I wanted to make for several months. It makes real life lights react to events happening in the Virtual Reality game [Beat Saber](https://beatsaber.com/). In this game, your goal is to slice blue and red cubes with the corresponding light saber color. This was done using an LED strip and a Raspberry Pi.

### Wiring
The first step was connecting the Raspberry Pi to the LED strip. I did this using a breadboard, a twelve volt power supply, and a few MOSFETs to control current flow. The Rasspberry Pi controls the amount of current to the MOSFETs through software, which changes the amount of electricity that makes it to the LED strip from the twelve volt power supply for each individual color. Being able to change the brightness of red, green, and blue on the LED trip allows me to create any color I want.

![](https://github.com/tkocher62/BeatSaberReactiveLights/blob/main/images/all.png)

### Software
The software I wrote for the Raspberry Pi was designed to function as a server, allowing any program to connect to it and control the lights. This makes the system completely wireless and very versatile. Following this, I wrote a plugin for Beat Saber that connects to the server and sends a message to it every time a block is cut. The plugin detects the color of the block cut and passes that as an argument to the server. The server will then modify the pulse rate of the respective GPIO pins on the Raspberry Pi to change the brightness of the individual colors on the LED strip.

### Finished Product

![](https://github.com/tkocher62/BeatSaberReactiveLights/blob/main/images/lights.gif)
