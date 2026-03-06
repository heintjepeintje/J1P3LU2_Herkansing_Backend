using LU2_API_Herkansing.Authentication;
using LU2_API_Herkansing.Interfaces;
using LU2_API_Herkansing.Repositories;
using Microsoft.AspNetCore.Identity;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Configuration.AddUserSecrets<Program>(true);

string? sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString") ?? builder.Configuration.GetValue<string>("SqlConnectionString");
if (sqlConnectionString == null)
{
	throw new Exception("SQL connection string cannot be null");
}

builder.Services.AddAuthorization();

IdentityBuilder identityBuilder = builder.Services.AddIdentityApiEndpoints<IdentityUser>(
	options =>
	{
		options.User.RequireUniqueEmail = true;
		options.Password.RequiredLength = 10;
		options.Password.RequireNonAlphanumeric = true;
		options.Password.RequireDigit = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireUppercase = true;
	});
identityBuilder.AddRoles<IdentityRole>();
identityBuilder.AddDapperStores(options =>
{
	options.ConnectionString = sqlConnectionString;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetAuthenticationService>();
builder.Services.AddScoped<AspNetAuthenticationService>();
builder.Services.AddScoped<IEnvironmentRepository>(provider => new EnvironmentDatabaseRepository(sqlConnectionString));
builder.Services.AddScoped<IObjectRepository>(provider => new ObjectDatabaseRepository(sqlConnectionString));
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Map("/", () => $"The API is up and running. ({sqlConnectionString})");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapGroup("/account").MapIdentityApi<IdentityUser>();
app.MapControllers().RequireAuthorization();

app.Run();
