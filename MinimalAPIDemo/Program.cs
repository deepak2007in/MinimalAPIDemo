using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(
    opt => opt.UseInMemoryDatabase("Todos"));

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

var todos = app.MapGroup("/todos");

todos.MapGet("/", (TodoDbContext dbContext) => 
       dbContext.Todos.ToListAsync());

todos.MapPost("/", async (Todo todo, TodoDbContext dbContext) => 
{
    dbContext.Todos.Add(entity: todo);
    await dbContext.SaveChangesAsync();
    return TypedResults.Created($"/todos/{todo.Id}", todo);
});

app.Run();

class Todo
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool IsComplete { get; set; }
}

class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Todo> Todos => Set<Todo>();
}