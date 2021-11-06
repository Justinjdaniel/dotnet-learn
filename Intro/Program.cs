var app = WebApplication.Create();

app.MapGet("/", () => "Hello world!");

app.Run();