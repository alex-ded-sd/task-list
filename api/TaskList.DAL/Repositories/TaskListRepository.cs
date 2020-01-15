namespace TaskList.DAL.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using DAL;
	using Entities;

	public enum Operations
	{
		Add,
		Update,
		Delete
	}

	public class TaskListRepository : IRepository<TaskItem>
	{
		private static readonly List<TaskItem> _mockDbTaskItems;

		private readonly List<(Operations operation, TaskItem taskListItem)> _modifiedItems;
		private bool _modifiedState;

		static TaskListRepository()
		{
			_mockDbTaskItems = new List<TaskItem>() {
				new TaskItem {Id = Guid.NewGuid(), Priority = 1, Name = "Write clean code"},
				new TaskItem {Id = Guid.NewGuid(), Priority = 2, Name = "Read book about Angular"},
				new TaskItem {Id = Guid.NewGuid(), Priority = 3, Name = "Have a rest"}
			};
		}

		public TaskListRepository()
		{
			_modifiedItems = new List<(Operations operation, TaskItem item)>();
		}

		private void Modify(Operations operation, TaskItem item)
		{
			TaskItem dbItem = _mockDbTaskItems.FirstOrDefault(existedItem => existedItem.Id == item.Id);
			if (dbItem == null) return;
			_modifiedItems.Add((Operations.Delete, item));
			_modifiedState = true;
		}

		private void SaveToDb(Operations operation, TaskItem taskListItem)
		{
			switch (operation)
			{
				case Operations.Add:
					_mockDbTaskItems.Add(taskListItem);
					break;
				case Operations.Update:
					int index = _mockDbTaskItems.FindIndex(dbItem => dbItem.Id == taskListItem.Id);
					if (index > -1) _mockDbTaskItems[index] = taskListItem;
					break;
				case Operations.Delete:
					_mockDbTaskItems.Remove(taskListItem);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
			}
		}

		public List<TaskItem> Get()
		{
			return _mockDbTaskItems;
		}

		public TaskItem Get(Guid id)
		{
			return _mockDbTaskItems.FirstOrDefault(task => task.Id == id);
		}

		public void Update(TaskItem item)
		{
			Modify(Operations.Update, item);
		}

		public void Add(TaskItem item)
		{
			TaskItem dbItem = _mockDbTaskItems.FirstOrDefault(existedItem => existedItem.Id == item.Id);
			if (dbItem != null) return;
			_modifiedItems.Add((Operations.Add, item));
			_modifiedState = true;

		}

		public void Delete(TaskItem item)
		{
			Modify(Operations.Delete, item);
		}


		public bool SaveChanges()
		{
			if (!_modifiedState) return false;
			_modifiedItems.ForEach(item => SaveToDb(item.operation, item.taskListItem));
			_modifiedState = false;
			return true;
		}

		public TaskItem Get(int priority)
		{
			return _mockDbTaskItems.FirstOrDefault(item => item.Priority == priority);
		}

		public IEnumerable<TaskItem> Get(Func<TaskItem, bool> predicate)
		{
			return _mockDbTaskItems.Where(predicate);
		}
	}
}