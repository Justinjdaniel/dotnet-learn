using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(options => options.UseInMemoryDatabase("items"));
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/todo", () => new { Item = "Water plants", Complete = "false" });
app.MapGet("/api/todos", async (TodoDb db) => await db.Todos.ToListAsync());

app.MapPost("/api/todos", async (TodoDb db, TodoItem todo) =>
{
    await db.Todos.AddAsync(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todo/{todo.Id}", todo);
});
app.MapGet("/api/todos/{id}", async (TodoDb db, int id) => await db.Todos.FindAsync(id));

app.MapPut("/api/todos/{id}", async (TodoDb db, TodoItem updateTodo, int id) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return NotFound();
    todo.Item = updateTodo.Item;
    todo.IsComplete = updateTodo.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/todos/{id}", async (TodoDb db, int id) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
    {
        return NotFound();
    }
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.Ok();
});

object NotFound()
{
    throw new NotImplementedException();
}

app.Run();

class TodoItem
{
    public int Id { get; set; }
    public string? Item { get; set; }
    public bool IsComplete { get; set; }
}

class TodoDb : DbContext
{
    public TodoDb(DbContextOptions options) : base(options) { }
    public DbSet<TodoItem> Todos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("Todos");
    }
}
