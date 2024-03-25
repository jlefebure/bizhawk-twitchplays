using System;
using System.Threading.Tasks;

namespace TwitchPlays
{
    public class InputTask(Func<Task> task, bool isCombo)
    {
	    public Func<Task> Task { get; set; } = task;
        public bool IsCombo { get; set; } = isCombo;
    }
}