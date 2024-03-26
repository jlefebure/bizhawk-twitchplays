using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using BizHawk.Client.Common;

namespace TwitchPlays
{
	public class IRCHandler
	{
		private ApiContainer _apIs;

		public AwaitableQueue ActionsQueue = new();
		private TextWriter? _output;
		private string _channel;
		private bool _isModOnly;

		private readonly string _twitchLogin;
		private readonly string _twitchToken;
		private readonly string _twitchChannel;
		private readonly SaveService _saveService;
		private readonly Control _mainControl;


		public IRCHandler(string login, string token, string channel, ApiContainer apiContainer,
			SaveService saveService, Control mainControl)
		{
			this._twitchLogin = login;
			this._twitchToken = token;
			this._twitchChannel = channel;
			this._channel = channel;
			this._apIs = apiContainer;
			this._saveService = saveService;
			this._saveService.StartTask();
			this._mainControl = mainControl;
		}

		public void UnpressAllKeys()
		{
			MainControlHandler.SafeInvoke(_mainControl, () => { _apIs.Joypad.Set(BtnConst.UnpressKeys); }, false);
		}

		static void Sleep(int length = 25)
		{
			System.Threading.Thread.Sleep(length);
		}

		public void LogText(string message)
		{
			Console.WriteLine(message);
		}

		public void RunIRC()
		{
			int port;
			string buf, nick, server, chan;
			System.Net.Sockets.TcpClient sock = new System.Net.Sockets.TcpClient();
			TextReader input;

			_channel = _twitchChannel;

			nick = _twitchLogin;
			server = "irc.twitch.tv";
			port = 6667;
			chan = "#" + _twitchChannel;

			//Connect to irc server and get input and output text streams from TcpClient.
			sock.Connect(server, port);
			if (!sock.Connected)
			{
				LogText("Failed to connect!");
				return;
			}
			else
			{
				LogText("Connected to " + chan);
			}

			input = new StreamReader(sock.GetStream());
			_output = new StreamWriter(sock.GetStream());

			//Starting USER and NICK login commands 
			_output.Write(
				"USER " + nick + "\n" +
				"PASS " + "oauth:" + _twitchToken + "\r\n" +
				"NICK " + nick + "\r\n"
			);
			_output.Flush();

			_output.Write("CAP REQ :twitch.tv/commands twitch.tv/tags\n");
			_output.Flush();
			//Process each line received from irc server
			while ((buf = input.ReadLine() ?? string.Empty) != null)
			{
				//Display received irc message
				CheckCommands(buf);

				//Send pong reply to any ping messages
				if (buf.StartsWith("PING ") || (buf.Contains("PING") && buf.Contains("mwax321")))
				{
					_output.Write(buf.Replace("PING", "PONG") + "\r\n");
					_output.Flush();
				}

				if (buf.Contains("mwax321") && buf.Contains("clear"))
				{
					//TODO clear text
				}

				if (buf[0] != ':') continue;

				/* IRC commands come in one of these formats:
				 * :NICK!USER@HOST COMMAND ARGS ... :DATA\r\n
				 * :SERVER COMAND ARGS ... :DATA\r\n
				 */
				//After server sends 001 command, we can set mode to bot and join a channel
				if (buf.Split(' ')[1] == "001")
				{
					_output.Write(
						"MODE " + nick + " +B\r\n" +
						"JOIN " + chan + "\r\n"
					);
					_output.Flush();
				}
			}

			LogText("Null detected on buffer read line ... ");

			RunIRC();
		}

		public Task Button(string btn, int sleep = 30, int combo = 1)
		{
			MainControlHandler.SafeInvoke(_mainControl,
				() => { _apIs.Joypad.Set(new Dictionary<string, bool> { { btn, true } }); }, false);
			Sleep(sleep);
			MainControlHandler.SafeInvoke(_mainControl,
				() => { _apIs.Joypad.Set(new Dictionary<string, bool> { { btn, false } }); }, false);
			return Task.CompletedTask;
		}

		private Task ButtonCombo(string btn, int sleep = 30, int combo = 1)
		{
			MainControlHandler.SafeInvoke(_mainControl,
				() => { _apIs.Joypad.Set(new Dictionary<string, bool> { { btn, true } }); }, false);
			Sleep(combo * sleep);
			MainControlHandler.SafeInvoke(_mainControl,
				() => { _apIs.Joypad.Set(new Dictionary<string, bool> { { btn, false } }); }, false);
			return Task.CompletedTask;
		}

		private string RemoveSpecialCharacters(string str)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in str)
			{
				if (c is >= '0' and <= '9' || c is >= 'A' and <= 'Z' ||
				    (c is >= 'a' and <= 'z' || c == '!' || c == ' '))
				{
					sb.Append(c);
				}
			}

			return sb.ToString();
		}

		private void SendMsg(string msg)
		{
			_output?.Write("PRIVMSG #" + _channel + " :" + msg + "\n");
			_output?.Flush();
		}

		//Map keys from IRC chat. If you want to use a different emulator or system, you will need to change this section to accept new commands from IRC
		private bool MapKeys(string command, bool isMod)
		{
			bool commandReal = true;
			if ((_isModOnly && isMod) || !_isModOnly)
			{
				switch (RemoveSpecialCharacters(command.ToLower()))
				{
					case "a":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.A), false));
						break;
					case "b":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.B), false));
						break;
					case "y":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.Y), false));
						break;
					case "x":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.X), false));
						break;
					case "l":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.L), false));
						break;
					case "r":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.R), false));
						break;
					case "zl":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.ZL), false));
						break;
					case "zr":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.ZR), false));
						break;
					case "start":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.Start), false));
						break;
					case "select":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.Select), false));
						break;
					case "haut":
					case "up":
					case "h":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.Up, 200), false));
						break;
					case "bas":
					case "down":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.Down, 200), false));
						break;
					case "gauche":
					case "left":
					case "g":
						ActionsQueue.EnqueueTask(new InputTask(() => Button(BtnConst.Left, 200), false));
						break;
					case "droite":
					case "droit":
					case "right":
					case "d":
						ActionsQueue.EnqueueTask(new InputTask(() => ButtonCombo(BtnConst.Right, 200), false));
						break;
					case var someVal when new Regex(@"^haut[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Up, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)), true));
						break;
					case var someVal when new Regex(@"^h[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Up, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)), true));
						break;
					case var someVal when new Regex(@"^up[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Up, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)), true));
						break;
					case var someVal when new Regex(@"^bas[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Down, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)),
							true));
						break;
					case var someVal when new Regex(@"^down[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Down, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)),
							true));
						break;
					case var someVal when new Regex(@"^g[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Left, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)),
							true));
						break;
					case var someVal when new Regex(@"^b[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Down, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)),
							true));
						break;
					case var someVal when new Regex(@"^d[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Right, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)),
							true));
						break;
					case var someVal when new Regex(@"^gauche[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Left, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)),
							true));
						break;
					case var someVal when new Regex(@"^left[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Left, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)),
							true));
						break;
					case var someVal when new Regex(@"^droite[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Right, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)),
							true));
						break;
					case var someVal when new Regex(@"^right[1-9]$").IsMatch(someVal):
						ActionsQueue.EnqueueTask(new InputTask(
							() => ButtonCombo(BtnConst.Right, 300, int.Parse(Regex.Match(someVal, @"\d+").Value)),
							true));
						break;
					case var someVal when new Regex(@"^!tp setmaxcombo [0-9]$").IsMatch(someVal):
						if (isMod)
						{
							var value = int.Parse(someVal.Split(' ')[1]);
							ActionsQueue.SetMaxCombo(value);
							SendMsg("Twitch Plays : Set max combo to " + value + ".");
						}

						break;

					case "!tp pause":
						if (isMod)
						{
							ActionsQueue.PauseQueue();
							SendMsg("Twitch Plays : Actions stopped by moderators.");
						}

						break;
					case "!tp resume":
						if (isMod)
						{
							ActionsQueue.ResumeQueue();
							SendMsg("Twitch Plays : Actions resumed by moderators.");
						}

						break;
					case "!tp save":
						if (isMod)
						{
							_saveService.Save();
							SendMsg("Twitch Plays : Saved state");
						}

						break;
					case "!tp autosave off":
						if (isMod)
						{
							_saveService.StopTask();
						}

						break;
					case "!tp autosave on":
						if (isMod)
						{
							_saveService.StartTask();
						}

						break;
					case "!tp load":
						if (isMod)
						{
							_saveService.Load();
							SendMsg("Twitch Plays : Load save state");
						}

						break;
					case var someVal when new Regex(@"^!tp load [1-9][1-9]$").IsMatch(someVal):
						if (isMod)
						{
							int val = int.Parse(Regex.Match(someVal, @"\d+").Value);
							_saveService.Load(val);
							SendMsg("Twitch Plays : Load save state minus n° " + val);
						}

						break;
					case "!tp emupause":
						if (isMod)
						{
							MainControlHandler.SafeInvoke(_mainControl, () => { _apIs.EmuClient.Pause(); }, false);
						}

						break;
					case "!tp emuresume":
						if (isMod)
						{
							MainControlHandler.SafeInvoke(_mainControl, () => { _apIs.EmuClient.Unpause(); }, false);
						}

						break;
					case "!tp clearqueue":
						if (isMod)
						{
							ActionsQueue.EmptyQueue();
							UnpressAllKeys();
							SendMsg("Twitch Plays : Actions queue as been cleared.");
						}

						break;
					case "!tp modonly":
						if (isMod)
						{
							SendMsg("Twitch Plays : Only moderation can send actions.");
							_isModOnly = true;
						}

						break;
					case "!tp free":
						if (isMod)
						{
							SendMsg("Twitch Plays : Everyone can send actions.");
							_isModOnly = false;
						}

						break;
					default:
						commandReal = false;
						break;
				}
			}
			else
			{
				return false;
			}

			return commandReal;
		}


		private void CheckCommands(string input)
		{
			string regexStreamer = @"(?<=PRIVMSG #)\w+";
			string regexUser = @"display-name=([^;]+)";

			string regexMessage = @"PRIVMSG #[\w]+ :(.*)";
			string regexMod = @"mod=(\d)";

			Regex userReg = new Regex(regexUser);
			Regex messageReg = new Regex(regexMessage);
			Regex modReg = new Regex(regexMod);
			Regex streamerReg = new Regex(regexStreamer);

			var userMatch = userReg.Match(input);
			string user = "god";
			if (userMatch.Success)
			{
				user = userMatch.Groups[1].Value;
			}

			string message = messageReg.Match(input).Groups[1].Value;
			var isModMatch = modReg.Match(input);
			int isMod = 0;
			if (isModMatch.Success)
			{
				isMod = int.Parse(modReg.Match(input).Groups[1].Value);
			}

			if (string.Equals(streamerReg.Match(input).Value, user, StringComparison.CurrentCultureIgnoreCase))
			{
				isMod = 1;
			}

			if (string.Equals(user, "kaoxyd", StringComparison.CurrentCultureIgnoreCase))
			{
				isMod = 1;
			}

			bool commandReal = MapKeys(message, isMod != 0);
			if (commandReal)
			{
				LogText(user + " " + (isMod == 1 ? "[MOD]" : "") + ": " + message);
			}
		}
	}
}