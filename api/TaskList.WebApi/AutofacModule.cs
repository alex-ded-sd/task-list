namespace TaskList.WebApi
{
	using Autofac;
	using DAL.Repositories;

	public class AutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder) {
			builder.RegisterGeneric(typeof(TaskListRepository))
				.As(typeof(IRepository<>))
				.InstancePerLifetimeScope();
		}
	}
}