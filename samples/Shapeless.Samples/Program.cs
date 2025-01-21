var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => { })
    .AddClayOptions(options => { options.KeyValueJsonToObject = true; });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/clay",
    async (HttpContext context, Clay clay) => { await context.Response.WriteAsJsonAsync(clay.ToJsonString()); });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();