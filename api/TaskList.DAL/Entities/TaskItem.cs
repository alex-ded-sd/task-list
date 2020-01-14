namespace TaskList.DAL.Entities
{
	using System;

	public class TaskItem
	{
		public Guid Id { get; set; }

		public int Priority { get; set; }

		public string Name { get; set; }
	}
}