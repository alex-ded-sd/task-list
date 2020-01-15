using System.Linq;
using System;
using System.Collections.Generic;
using TaskList.DAL.Entities;
using TaskList.DAL.Repositories;

namespace TaskList.Core.Services
{
	public class TaskListService
	{
		private readonly IRepository<TaskItem> _repository;
		private static readonly object _lockObject = new object();

		public TaskListService(IRepository<TaskItem> repository)
		{
			_repository = repository;
		}

		public List<TaskItem> Get()
		{
			return _repository.Get();
		}

		public TaskItem Get(Guid id)
		{
			return _repository.Get(id);
		}

		public bool RaisePriority(TaskItem item)
		{
			lock (_lockObject)
			{
				int higerPriority = item.Priority + 1;
				TaskItem higherPriorityItem = _repository.Get(higerPriority);
				if (higherPriorityItem == null) return false;
				higherPriorityItem.Priority = item.Priority;
				item.Priority++;
				_repository.Update(item);
				_repository.Update(higherPriorityItem);
				return _repository.SaveChanges();
			}
		}

		public bool ReducePriority(TaskItem item)
		{
			lock (_lockObject)
			{
				int lowerPriority = item.Priority - 1;
				TaskItem lowerPriorityItem = _repository.Get(lowerPriority);
				if (lowerPriorityItem == null) return false;
				lowerPriorityItem.Priority = item.Priority;
				item.Priority--;
				_repository.Update(item);
				_repository.Update(lowerPriorityItem);
				return _repository.SaveChanges();
			}
		}

		public bool SetPriority(TaskItem item, int priority)
		{
			lock (_lockObject)
			{
				List<TaskItem> items = _repository.Get(dbItem => dbItem.Priority >= priority)
					.OrderBy(filteredItem => filteredItem.Priority).ToList();
				if (items.Count == 0) return false;
				item.Priority = priority;
				_repository.Update(item);
				foreach (TaskItem dbItem in items)
				{
					dbItem.Priority++;
					_repository.Update(dbItem);
				}
				return _repository.SaveChanges();
			}
		}
	}
}