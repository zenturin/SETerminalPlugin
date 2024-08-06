# SETerminalPlugin

## Prerequisites

You will need to use either:

A compatible script written with this plugin in mind:
None Yet

Or Write your own ingame script using the TerminalAPI
https://github.com/zenturin/TerminalAPI

## This plugin is still in development and is extremely buggy and missing features.
I am currently working on a V2.0 with the knowledge i gained making the first.

### Setup (ONLY FOR V2)

1. You need a grid with a ShipController, ProgrammableBlock and Power.
2. You must enter '#terminal' in the shipcontrollers customdata.
3. if all goes well after reopening the customdata you should be able to specify a Tag for your PB. | Defalt: [NA]
4. Then go to the PB you want to use for your program and add the same Tag anywhere in the name.
5. Inside the PB customdata if nothing is there type '#terminal' and reload the customdata.
6. Exit the terminal and press '`' (Tilda) the key under escape.
7. Follow the script instructions for more or utilise the api to write your own

### Notes
- The Api should be fully documented upon release. The V1 Api is documented but V1 is frankly awful
- The Api will utilise arguments and customdata for communication scripts and mods that effect this may be incompatible
- The Api will contain a full example and Template for making your own scripts (WIP)
https://github.com/zenturin/TerminalAPI

