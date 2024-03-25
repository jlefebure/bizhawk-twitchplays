using System;
using System.IO;
using System.Windows.Forms;
using BizHawk.Client.Common;
using Timer = System.Threading.Timer;

namespace TwitchPlays;

public class SaveService(ApiContainer apis, Control mainControl)
{
	private Timer _timer = null!;
	private static readonly string SavePrefix = "tppSaveAuto";
	private static readonly string SavePath = @"SaveTPP\State";
	private static readonly int MaxSaves = 30;

	private void ExecuteTask(object state)
	{
		this.Save();
	}

	public void StartTask()
	{
		_timer = new Timer(ExecuteTask, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
	}

	public void StopTask()
	{
		_timer.Dispose();
	}


	public void Load(int nMinus = 0)
	{
		string[] saveFiles = Directory.GetFiles(SavePath, SavePrefix + "-*.State");

		if (saveFiles.Length > 0)
		{
			Array.Sort(saveFiles, (x, y) => File.GetCreationTime(y).CompareTo(File.GetCreationTime(x)));
			if (nMinus < saveFiles.Length)
			{
				string mostRecentSave = saveFiles[nMinus];
				Load(mostRecentSave.Replace(".State", ""));
			}
			else
			{
				Console.WriteLine("Cannot load save number " + nMinus);
			}
		}
		else
		{
			Console.WriteLine("No save found.");
		}
	}

	public void CleanSavePath()
	{
		string[] saveFiles = Directory.GetFiles(SavePath, SavePrefix + "-*.State");

		if (saveFiles.Length > MaxSaves)
		{
			Array.Sort(saveFiles, (x, y) => File.GetCreationTime(y).CompareTo(File.GetCreationTime(x)));

			for (int i = MaxSaves; i < saveFiles.Length; i++)
			{
				File.Delete(saveFiles[i]);
				Console.WriteLine("Deleted save : " + saveFiles[i]);
			}
		}
	}

	private void CheckSaveDir()
	{
		if (!Directory.Exists(SavePath))
		{
			Directory.CreateDirectory(SavePath);
		}
	}

	public void Save()
	{
		CheckSaveDir();
		apis.EmuClient.Pause();
		
		MainControlHandler.SafeInvoke(mainControl, () =>apis.EmuClient.SaveState(SavePath + @"\" + SavePrefix + "-" + DateTime.Now.ToString("yyyyMMddHHmm")), false );

		apis.EmuClient.SaveRam();
		apis.EmuClient.Unpause();
		CleanSavePath();
	}

	private void Load(string saveName)
	{
		CheckSaveDir();
		apis.EmuClient.Pause();
		MainControlHandler.SafeInvoke(mainControl, () => { apis.EmuClient.LoadState(saveName); }, false );
		apis.EmuClient.Unpause();
	}
}