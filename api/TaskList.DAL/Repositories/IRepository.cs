namespace TaskList.DAL.Repositories
{
	using System;
	using System.Collections.Generic;
	using TaskList.DAL.Entities;

	public interface IRepository<T>
	{
		List<T> Get();

		T Get(Guid id);

		void Update(T item);

		void Add(T item);

		void Delete(T item);

		bool SaveChanges();
		TaskItem Get(int priority);
		IEnumerable<TaskItem> Get(Func<TaskItem, bool> predicate);
	}
}