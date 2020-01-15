namespace TaskList.WebApi.Controllers
{
	using Microsoft.AspNetCore.Mvc;
	using Core.Services;
	using System.Collections.Generic;
	using DAL.Entities;

	[ApiController]
	[Route("tasklist")]
	public class TaskListController : ControllerBase
	{
		private readonly TaskListService _service;
		public TaskListController(TaskListService service)
		{
			this._service = service;
		}

		[HttpGet]
		public IEnumerable<TaskItem> Get()
		{
			return _service.Get();
		}

		[HttpPost("raisepriority")]

		public IActionResult RaisePriority(TaskItem item)
		{
			_service.RaisePriority(item);
			return Ok();
		}

		[HttpPost("reducepriority")]
		public IActionResult ReducePriority(TaskItem item)
		{
			_service.ReducePriority(item);
			return Ok();
		}

		[HttpPost("setpriority")]
		public IActionResult SetPriority(TaskItem item, int priority)
		{
			_service.SetPriority(item, priority);
			return Ok();
		}
	}
}