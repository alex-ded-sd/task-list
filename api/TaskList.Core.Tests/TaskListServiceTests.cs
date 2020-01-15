namespace TaskList.Core.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Autofac.Extras.FakeItEasy;
	using Autofac.Extras.Moq;
	using DAL.Entities;
	using DAL.Repositories;
	using FakeItEasy;
	using Moq;
	using NUnit.Framework;
	using Services;

	public class TaskListServiceTests
	{

		[SetUp]
		public void Setup() {
		}

		private TaskItem GenerateTaskItem(Guid id, string name, int priority) =>
			new TaskItem {
				Id = id,
				Name = name,
				Priority = priority
			};

		[Test]
		public void RaisePriority_ShouldCorrectlyChangePriority() {
			Guid target = Guid.NewGuid();
			Guid higher = Guid.NewGuid();
			TaskItem targetItem = GenerateTaskItem(target, "target", 7);
			TaskItem higherItem = GenerateTaskItem(higher, "higher", 6);
			int expectedTargetPriority = higherItem.Priority;
			int expectedHigherPriority = targetItem.Priority;
			using var mock = AutoMock.GetLoose();
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(6)).Returns(higherItem);
			var service = mock.Create<TaskListService>();
			service.RaisePriority(targetItem);
			mock.Mock<IRepository<TaskItem>>().Verify(x => x.Update(It.IsAny<TaskItem>()), () => Times.Exactly(2));
			Assert.AreEqual(targetItem.Priority, expectedTargetPriority);
			Assert.AreEqual(higherItem.Priority, expectedHigherPriority);
		}

		[Test]
		public void RaisePriority_ShouldCorrectlyReducePriority() {
			TaskItem targetItem = GenerateTaskItem(Guid.NewGuid(), "higher", 6);
			TaskItem lowerPriority = GenerateTaskItem(Guid.NewGuid(), "target", 7);
			int expectedTargetPriority = lowerPriority.Priority;
			int expectedHigherPriority = targetItem.Priority;
			using var mock = AutoMock.GetLoose();
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(7)).Returns(lowerPriority);
			var service = mock.Create<TaskListService>();
			service.ReducePriority(targetItem);
			mock.Mock<IRepository<TaskItem>>().Verify(x => x.Update(It.IsAny<TaskItem>()), () => Times.Exactly(2));
			Assert.AreEqual(targetItem.Priority, expectedTargetPriority);
			Assert.AreEqual(lowerPriority.Priority, expectedHigherPriority);
		}

		[Test]
		public void SetPriority_ShouldCorrectlySetLowerPriority() {
			TaskItem sixPrior = GenerateTaskItem(Guid.NewGuid(), "task", 6);
			TaskItem sevenPrior = GenerateTaskItem(Guid.NewGuid(), "task", 7);
			TaskItem eightPrior = GenerateTaskItem(Guid.NewGuid(), "task", 8);
			TaskItem ninePrior = GenerateTaskItem(Guid.NewGuid(), "task", 9);
			TaskItem tenPrior = GenerateTaskItem(Guid.NewGuid(), "task", 10);
			using var mock = AutoMock.GetLoose();
			mock.Mock<IRepository<TaskItem>>()
				.Setup(x => x.Get(It.IsAny<Func<TaskItem, bool>>()))
				.Returns((Func<TaskItem, bool> filter) =>
					new List<TaskItem>(){ sixPrior, sevenPrior, eightPrior, ninePrior, tenPrior }.Where(filter).ToList());
			var service = mock.Create<TaskListService>();
			service.SetPriority(sevenPrior, 9);
			mock.Mock<IRepository<TaskItem>>().Verify(x => x.Update(It.IsAny<TaskItem>()), () => Times.Exactly(3));
			Assert.AreEqual(sevenPrior.Priority, 9);
			Assert.AreEqual(eightPrior.Priority, 7);
			Assert.AreEqual(ninePrior.Priority, 8);
		}

		[Test]
		public void SetPriority_ShouldCorrectlySetHigherLowerPriority() {
			TaskItem sixPrior = GenerateTaskItem(Guid.NewGuid(), "task", 6);
			TaskItem sevenPrior = GenerateTaskItem(Guid.NewGuid(), "task", 7);
			TaskItem eightPrior = GenerateTaskItem(Guid.NewGuid(), "task", 8);
			TaskItem ninePrior = GenerateTaskItem(Guid.NewGuid(), "task", 9);
			TaskItem tenPrior = GenerateTaskItem(Guid.NewGuid(), "task", 10);
			using var mock = AutoMock.GetLoose();
			mock.Mock<IRepository<TaskItem>>()
				.Setup(x => x.Get(It.IsAny<Func<TaskItem, bool>>()))
				.Returns((Func<TaskItem, bool> filter) =>
					new List<TaskItem>() { sixPrior, sevenPrior, eightPrior, ninePrior, tenPrior }.Where(filter).ToList());
			var service = mock.Create<TaskListService>();
			service.SetPriority(tenPrior, 6);
			mock.Mock<IRepository<TaskItem>>().Verify(x => x.Update(It.IsAny<TaskItem>()), () => Times.Exactly(5));
			Assert.AreEqual(sixPrior.Priority, 7);
			Assert.AreEqual(sevenPrior.Priority, 8);
			Assert.AreEqual(eightPrior.Priority, 9);
			Assert.AreEqual(ninePrior.Priority, 10);
			Assert.AreEqual(tenPrior.Priority, 6);
		}

		[Test]
		public void UpdateTask_ShouldCorrectlyUpdateAndChangePriority() {
			TaskItem sixPrior = GenerateTaskItem(Guid.NewGuid(), "task", 6);
			TaskItem sevenPrior = GenerateTaskItem(Guid.NewGuid(), "task", 7);
			TaskItem eightPrior = GenerateTaskItem(Guid.NewGuid(), "task", 8);
			using var mock = AutoMock.GetLoose();
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(sevenPrior.Priority)).Returns(sevenPrior);
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(sixPrior.Id)).Returns(sixPrior);
			mock.Mock<IRepository<TaskItem>>()
				.Setup(x => x.Get(It.IsAny<Func<TaskItem, bool>>()))
				.Returns((Func<TaskItem, bool> filter) =>
					new List<TaskItem>() { sixPrior, sevenPrior, eightPrior }.Where(filter).ToList());
			var service = mock.Create<TaskListService>();
			TaskItem updatingItem = GenerateTaskItem(sixPrior.Id, sixPrior.Name, sevenPrior.Priority);
			service.UpdateTask(updatingItem);
			mock.Mock<IRepository<TaskItem>>().Verify(x => x.Update(It.IsAny<TaskItem>()), () => Times.Exactly(2));
			Assert.AreEqual(sixPrior.Priority, 7);
			Assert.AreEqual(sevenPrior.Priority, 6);
		}

		[Test]
		public void UpdateTask_ShouldCorrectlyUpdateAndDontChangePriority() {
			TaskItem sixPrior = GenerateTaskItem(Guid.NewGuid(), "task", 6);
			TaskItem sevenPrior = GenerateTaskItem(Guid.NewGuid(), "task", 7);
			TaskItem eightPrior = GenerateTaskItem(Guid.NewGuid(), "task", 8);
			using var mock = AutoMock.GetLoose();
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(sixPrior.Priority)).Returns(sixPrior);
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(sixPrior.Id)).Returns(sixPrior);
			mock.Mock<IRepository<TaskItem>>()
				.Setup(x => x.Get(It.IsAny<Func<TaskItem, bool>>()))
				.Returns((Func<TaskItem, bool> filter) =>
					new List<TaskItem>() { sixPrior, sevenPrior, eightPrior }.Where(filter).ToList());
			var service = mock.Create<TaskListService>();
			sixPrior.Name = "Changed";
			var equal = service.UpdateTask(sixPrior);
			mock.Mock<IRepository<TaskItem>>().Verify(x => x.Update(It.IsAny<TaskItem>()), () => Times.Exactly(1));
			Assert.AreEqual(sixPrior.Priority, 6);
			Assert.AreEqual(sevenPrior.Priority, 7);
			Assert.AreEqual(eightPrior.Priority, 8);
			Assert.AreEqual(equal.Name, sixPrior.Name);
		}

		[Test]
		public void AddTask_ShouldCorrectlyAddTaskWithLowestPriority() {
			TaskItem sixPrior = GenerateTaskItem(Guid.NewGuid(), "task", 6);
			TaskItem sevenPrior = GenerateTaskItem(Guid.NewGuid(), "task", 7);
			TaskItem eightPrior = GenerateTaskItem(Guid.NewGuid(), "task", 8);
			using var mock = AutoMock.GetLoose();
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(sixPrior.Priority)).Returns(sixPrior);
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(sixPrior.Id)).Returns(sixPrior);
			mock.Mock<IRepository<TaskItem>>()
				.Setup(x => x.Get()).Returns(new List<TaskItem>() { sixPrior, sevenPrior, eightPrior });
			TaskItem newTask = GenerateTaskItem(Guid.NewGuid(), "new task", 1);
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(newTask.Id)).Returns(newTask);
			var service = mock.Create<TaskListService>();
			var equal = service.AddTask(newTask);
			mock.Mock<IRepository<TaskItem>>().Verify(x => x.Add(It.IsAny<TaskItem>()), () => Times.Exactly(1));
			Assert.AreEqual(newTask.Id, equal.Id);
			Assert.AreEqual(equal.Priority, new[] {
				sixPrior.Priority, sevenPrior.Priority, eightPrior.Priority
			}.Max() + 1);
		}

		[Test]
		public void DeleteTask_ShouldCorrectlyDeleteTask() {
			TaskItem sixPrior = GenerateTaskItem(Guid.NewGuid(), "task", 6);
			TaskItem sevenPrior = GenerateTaskItem(Guid.NewGuid(), "task", 7);
			TaskItem eightPrior = GenerateTaskItem(Guid.NewGuid(), "task", 8);
			using var mock = AutoMock.GetLoose();
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(sixPrior.Id)).Returns(sixPrior);
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.SaveChanges()).Returns(true);
			var service = mock.Create<TaskListService>();
			var result = service.DeleteTask(sixPrior);
			mock.Mock<IRepository<TaskItem>>().Verify(x => x.Delete(It.IsAny<TaskItem>()), () => Times.Exactly(1));
			Assert.IsTrue(result);
		}

		[Test]
		public void DeleteTask_ShouldDeleteIfElementDoesntExist() {
			TaskItem sixPrior = GenerateTaskItem(Guid.NewGuid(), "task", 6);
			TaskItem sevenPrior = GenerateTaskItem(Guid.NewGuid(), "task", 7);
			TaskItem eightPrior = GenerateTaskItem(Guid.NewGuid(), "task", 8);
			using var mock = AutoMock.GetLoose();
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.Get(sixPrior.Id)).Returns(default(TaskItem));
			mock.Mock<IRepository<TaskItem>>().Setup(x => x.SaveChanges()).Returns(true);
			var service = mock.Create<TaskListService>();
			var result = service.DeleteTask(sixPrior);
			mock.Mock<IRepository<TaskItem>>().Verify(x => x.Delete(It.IsAny<TaskItem>()), () => Times.Exactly(0));
			Assert.IsFalse(result);
		}
	}
}