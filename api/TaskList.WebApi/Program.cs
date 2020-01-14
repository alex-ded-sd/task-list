namespace TaskList.WebApi
{
	using Autofac.Extensions.DependencyInjection;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Hosting;

	public class Program
	{
		public static void Main(string[] args) {
			CreateHostBuilder(args).ConfigureServices(services => services.AddAutofac()).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => {
					webBuilder.UseStartup<Startup>();
				});
	}
}
