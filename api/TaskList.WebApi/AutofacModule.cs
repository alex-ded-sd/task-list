namespace TaskList.WebApi
{
	using Autofac;
	using DAL.Repositories;
	using Core.Services;
	using TaskList.DAL.Entities;

	public class AutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<TaskListRepository>()
				.As<IRepository<TaskItem>>()
				.InstancePerLifetimeScope();
			builder.RegisterType<TaskListService>().InstancePerLifetimeScope();
		}
	}
}