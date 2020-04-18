
using Amazon.DynamoDBv2;
using DataAccess;
using DataAccess.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RandomUserGenerator.Utils;

namespace RandomUserGenerator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Add dynamo service

            var dynamoDbConfig = Configuration.GetSection("DynamoDB");

            if (dynamoDbConfig.GetValue<bool>("LocalMode"))
            {//Add Local DynamoDB Service if running dev mode - from appsettings.Development.json
                var serviceUrl = dynamoDbConfig.GetValue<string>("LocalUrl");
                services.AddSingleton<IAmazonDynamoDB>(sp =>
                {
                    var clientConfig = new AmazonDynamoDBConfig { ServiceURL = serviceUrl };
                    return new AmazonDynamoDBClient(clientConfig);
                });
            }
            else
                //Use real DynamoDB
                services.AddAWSService<IAmazonDynamoDB>();

            #endregion Add dynamo service

            #region our dependencies
            services.AddSingleton<IUserWorker, UserWorker>();
            services.AddSingleton<IRandomIDGenerator, RandomIDGenerator>();
            #endregion ourdependencies

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
