using System;

namespace TaskList.WebApi.TaskList.DAL.Entities
{
	public class TaskItemEntity
	{
		public Guid Id { get; set; }

		public int Priority { get; set; }

		public string Name { get; set; }
	}
}