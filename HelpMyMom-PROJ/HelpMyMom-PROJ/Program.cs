
using HelpMyMom_PROJ.models;

namespace HelpMyMom_PROJ
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<SoftwareBirbContext>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            /*
             * {
                "id": 0,
                "fName": "string",
                "lName": "string",
                "username": "string",
                "email": "string",
                "relationships": [],
                 "reports": [],
                "tickets": []
                }
             */
        }
    }
}
