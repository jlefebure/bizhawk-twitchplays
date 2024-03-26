# Bizhawk Twitch Plays 

Bizhawk is an emulation project. EmuHawk is a multi-system emulator written in C#. As well as quality-of-life features for casual players, it also has recording/playback and debugging tools, making it the first choice for TASers (Tool-Assisted Speedrunners). More info on [Bizhawk GitHub page](https://github.com/TASEmulators/BizHawk)

Bizhawk Twitch Plays is an external module in C#. It allow a Twitch streamer to host and cast Twitch Plays. The module read the inputs in chat and transmit it to the emulator, whatever the game.

## Run

Run the emulator by launching `EmuHawk.exe`. You can launch de TwitchPlays setup in `Tools` > `External Tools` > `Twitch Plays`
On first launch, a popup ask you to trust the external tool launch.

Alternatively, you can launch Bizhawk with Twitch Plays module in cmd

```powershell 
EmuHawk.exe --open-ext-tool-dll=TwitchPlays
```
## Setup

<img width="320" alt="bizhawkconf" src="https://github.com/jlefebure/bizhawk-twitchplays/assets/5576211/faba1e9b-5241-4c79-9edc-366e5cc210f6">

Bizhawk Twitch Plays needs an account that will read inputs from your Twitch chat. It does not matter if the account is yours, or a moderator.

### Setup read account

Type the read account login in Twitch Login. You will need a OAuth access token to login on Twitch. You can get one on [Twitch Token Generator](https://twitchtokengenerator.com/). Be sure to select a bot access token, we only need to read and send inputs on Twitch chat, nothing more.
Get the access token and paste it in Oauth Token field.

Finally, enter your twitch channel on the corresponding field. 

## Operate 

You can load a rom directly from BizHawk windows. Once you're ready and your game is loaded, you can click the `Start` button. It will log on the account specified previously and will start to read viewers inputs.
You can stop input reading by clicking on `Stop`.

You can manually launch or load a save state.

### Saving system

The system automatically save the current state every 10 minutes, in case of crashing. Last 30 saves are keeped. 

### Chat inputs

Every user can send inputs directly via your Twitch chat. Following inputs are currently mapped. Inputs are not case sensitive.

#### Simple inputs

Input|Corresponding key
| --- | --- |
`up`, `haut`, `h` | Up
`down`, `bas` | Down
`left`, `gauche`, `g` | Left
`right`, `droite`, `droit`, `d` | Right
`a` | A
`b` | B
`x` | X
`y` | Y
`zr` | ZR
`zl` | ZL
`l` | L
`r` | R

#### Combo moves
Users can use combo moves. For example, using `h8` will keep the `Up` input longer than a simple input `h`. On many games, it will allow a character to move 8 steps to the top. 

Streamer and moderator can disable this feature via Twitch command. It is also possible to set the maximum combo move. By default, it is possible to make 9 combo moves. To avoid spamming, only 3 combo moves can be queued at the same time. This value is editable.

Input|Corresponding key
| --- | --- |
`h[1-9]`, `up[1-9]`, `haut[1-9]` | Up
`down[1-9]`, `bas[1-9]`, `b[1-9]` | Down
`left[1-9]`, `gauche[1-9]`, `g[1-9]` | Left
`right[1-9]`, `droite[1-9]`, `d[1-9]` | Right

### Twitch Commands

Streamer and moderators of the Twitch channel can setup various things by typing these commands on the chat. 

Command|Behaviour
| --- | --- |
`!tp pause` | Pause the input reading for everyone
`!tp resume` | Resume the input reading for everyone
`!tp save` | Launch a save state
`!tp load [x]` | Load a save state. Without arg, it will load the last save state. With an argument, it will load the n-x last save state. For exemple, `!tp load 2` will load the 2 most recent save state.
`!tp autosave off` | Disable the automatic save state every 10 minutes
`!tp autosave on` | Enable the automatic save state every 10 minutes (default behaviour)
`!tp emupause` | Pause the emulator
`!tp emuresume` | Resume the emulator
`!tp clearqueue` | Clear the queued actions and release all inputs that have been pressed
`!tp modonly` | Only moderators and streamer can send actions
`!tp free` | Remove the previous behaviour
`!tp setmaxcombo [1-9]` | Set the maximum combo move value

## Support

You can open a ticket on this repo, or contact me via Discord (Kaoxyd) or [Twitch](https://twitch.tv/kaoxyd)

## Credits

A massive thanks to all fellow viewers that help us during the tests, and to the streamer that host Twitch Plays sessions: 

* [XababTV](https://twitch.tv/xababtv): French partner, multigamer streamer
* [RomainJacques_](https://twitch.tv/romainjacques_): Swiss partner, multigamer, challenger and speedrunner streamer
* [Kalyoz](https://twitch.tv/kalyoz): French partner, retro gamer, cast and react of TV shows of the 90s
