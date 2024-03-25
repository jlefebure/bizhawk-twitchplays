# BizHawk

A multi-system emulator written in C#. As well as quality-of-life features for casual players, it also has recording/playback and debugging tools, making it the first choice for TASers (Tool-Assisted Speedrunners).
You can get Bizhawk releases and docs on [Bizhawk Github page](https://github.com/TASEmulators/BizHawk).

Supported consoles and computers:

* Apple II
* Arcade machines
* Atari
	* Video Computer System / 2600
	* 7800
	* Jaguar + CD
	* Lynx
* Bandai WonderSwan + Color
* CBM Commodore 64
* Coleco Industries ColecoVision
* GCE Vectrex
* Magnavox Odyssey² / Videopac G7000
* Mattel Intellivision
* MSX
* NEC
	* PC Engine / TurboGrafx-16 + SuperGrafx + CD
	* PC-FX
* Neo Geo Pocket + Color
* Nintendo
	* Famicom / Nintendo Entertainment System + FDS
	* Game Boy + Color
	* Game Boy Advance
	* Nintendo 64 + N64DD
	* Super Famicom / Super Nintendo Entertainment System + SGB + Satellaview
	* Virtual Boy
* Sega
	* Game Gear
	* Genesis + 32X + CD
	* Master System
	* Saturn
	* SG-1000
* Sinclair ZX Spectrum
* Sony Playstation (PSX)
* Texas Instruments TI-83
* TIC-80
* Uzebox
* more coming soon..?


# Bizhawk Twitch Plays
Bizhawk Twitch Plays is an extension which allow a Twitch streamer to launch Twitch Plays from Bizhawk.
All platforms supported by Bizhawk are supported.

The forked version of Bizhawk override some Bizhawk default behaviours on key mapping and error handling. 

## Start

Run `EmuHawk.exe`. You can run the Twitch Plays setup in `Tools` > `External Tools` > `TwitchPlays`
A popup should ask if you want to run an external tool. Say yes.

Alternatively, you can start the emulator by running with arg 

```powershell
EmuHawk.exe --open-ext-tool-dll=TwitchPlays
```

## Setup 

