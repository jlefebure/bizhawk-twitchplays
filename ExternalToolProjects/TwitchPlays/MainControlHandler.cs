using System;
using System.Windows.Forms;

namespace TwitchPlays;

public class MainControlHandler
{
	
	public static void SafeInvoke(Control uiElement, Action updater, bool forceSynchronous)
	{
		if (uiElement == null)
		{
			throw new ArgumentNullException("uiElement");
		}

		if (uiElement.InvokeRequired)
		{
			if (forceSynchronous)
			{
				uiElement.Invoke((Action)delegate { SafeInvoke(uiElement, updater, forceSynchronous); });
			}
			else
			{
				uiElement.BeginInvoke((Action)delegate { SafeInvoke(uiElement, updater, forceSynchronous); });
			}
		}
		else
		{    
			updater();
		}
	}
}