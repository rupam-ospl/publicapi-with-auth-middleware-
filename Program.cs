using OpenAPI3.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();
app.UseSwagger();
//app.UseSwaggerUI(c => c.RoutePrefix = string.Empty);
app.UseSwaggerUI();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();
