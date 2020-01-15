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
			lock (_lockObject) {
				return _repository.Get();
			}
		}

		public TaskItem Get(Guid id)
		{
			lock (_lockObject) {
				return _repository.Get(id);
			}
		}

		public bool RaisePriority(TaskItem item)
		{
			lock (_lockObject)
			{
				int higherPriority = item.Priority - 1;
				TaskItem higherPriorityItem = _repository.Get(higherPriority);
				if (higherPriorityItem == null) return false;
				higherPriorityItem.Priority = item.Priority;
				item.Priority--;
				_repository.Update(item);
				_repository.Update(higherPriorityItem);
				return _repository.SaveChanges();
			}
		}

		public bool ReducePriority(TaskItem item)
		{
			lock (_lockObject)
			{
				int lowerPriority = item.Priority + 1;
				TaskItem lowerPriorityItem = _repository.Get(lowerPriority);
				if (lowerPriorityItem == null) return false;
				lowerPriorityItem.Priority = item.Priority;
				item.Priority++;
				_repository.Update(item);
				_repository.Update(lowerPriorityItem);
				return _repository.SaveChanges();
			}
		}

		public bool SetPriority(TaskItem item, int priority)
		{
			lock (_lockObject)
			{
				List<TaskItem> forReducing = _repository
					.Get(dbItem => dbItem.Priority <= priority && dbItem.Priority > item.Priority)
					.ToList();
				List<TaskItem> forIncreasing = _repository
					.Get(dbItem => dbItem.Priority >= priority && dbItem.Priority < item.Priority)
					.ToList();
				item.Priority = priority;
				_repository.Update(item);
				forReducing.ForEach(task => {
					task.Priority--;
					_repository.Update(task);
				});
				forIncreasing.ForEach(task => {
					task.Priority++;
					_repository.Update(task);
				});
				return _repository.SaveChanges();
			}
		}

		public TaskItem UpdateTask(TaskItem item) {
			lock (_lockObject) {
				TaskItem dbItem = _repository.Get(item.Priority);
				if (dbItem == null) return null;
				if (dbItem.Id == item.Id) {
					_repository.Update(item);
					_repository.SaveChanges();
				}
				else {
					TaskItem targetDbItem = _repository.Get(item.Id);
					SetPriority(targetDbItem, item.Priority);
				}
				return _repository.Get(item.Id);
			}
		}

		public TaskItem AddTask(TaskItem newTask) {
			lock (_lockObject) {
				int maxPrior = _repository.Get().Max(dbItem => dbItem.Priority);
				newTask.Priority = maxPrior+1;
				_repository.Add(newTask);
				_repository.SaveChanges();
				return _repository.Get(newTask.Id);
			}
		}

		public bool DeleteTask(TaskItem deletingTask) {
			lock (_lockObject) {
				TaskItem dbItem = _repository.Get(deletingTask.Id);
				if (dbItem != null) {
					_repository.Delete(deletingTask);
					return _repository.SaveChanges();
				}
				return false;
			}
		}
	}
}