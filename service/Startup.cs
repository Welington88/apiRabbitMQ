using apiRabbitMQ.App;
using apiRabbitMQ.Consumers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace service
{
    public class Startup
    {
        private ConfigurationManager configuration;

        public Startup(ConfigurationManager configuration)
        {
            this.configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<ProcessMessageConsumer>();
            services.Configure<RabbitMqConfiguration>(Configuration.GetSection("RabbitMaConfig"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {


        }
    }
}
