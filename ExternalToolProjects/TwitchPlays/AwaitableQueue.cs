using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitchPlays
{
	public class AwaitableQueue
	{
		private readonly Queue<InputTask> _taskQueue = new();
		private bool _isExecuting;
		private int _maxCombo = 3;

		public bool IsPaused;

		private int _nbComboInQueue;

		public void PauseQueue()
		{
			IsPaused = true;
		}

		public void ResumeQueue()
		{
			IsPaused = false;
		}

		public void EnqueueTask(InputTask task)
		{
			if (!IsPaused)
			{
				if (task.IsCombo)
				{
					if (_nbComboInQueue >= _maxCombo)
					{
						return;
					}
					else
					{
						++_nbComboInQueue;
					}
				}

				_taskQueue.Enqueue(task);
				if (!_isExecuting)
				{
					_isExecuting = true;
					Task.Run(() => ExecuteTasksAsync());
				}
			}
		}

		private async Task ExecuteTasksAsync()
		{
			while (_taskQueue.Count > 0)
			{
				var task = _taskQueue.Dequeue();

				await task.Task();
				if (task.IsCombo)
				{
					--_nbComboInQueue;
				}
			}

			_isExecuting = false;
		}

		public void EmptyQueue()
		{
			_taskQueue.Clear();
			_nbComboInQueue = 0;
		}

		public void SetMaxCombo(int value)
		{
			_maxCombo = value;
		}
	}
}