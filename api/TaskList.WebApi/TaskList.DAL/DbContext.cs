using System;
using System.Collections.Generic;
using TaskList.WebApi.TaskList.DAL.Entities;

namespace TaskList.WebApi.TaskList.DAL
{
	public class DbContext
	{
		private static readonly List<TaskItemEntity> _mockDbTaskItems;
		private static readonly object _lockObject = new object();
		static DbContext()
		{
			_mockDbTaskItems = new List<TaskItemEntity>(GetInitData());
		}

		private static List<TaskItemEntity> GetInitData()
		{
			return new List<TaskItemEntity>() {
				new TaskItemEntity { Id = Guid.NewGuid(), Priority = 1, Name = "Write clean code" },
				new TaskItemEntity { Id = Guid.NewGuid(), Priority = 2, Name = "Read book about Angular" },
				new TaskItemEntity { Id = Guid.NewGuid(), Priority = 3, Name = "Have a rest" }};
		}

		public List<TaskItemEntity> GetItems()
		{
			return _mockDbTaskItems;
		}

		public bool SaveChanges()
		{
			return true;
		}
	}
}