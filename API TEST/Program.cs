var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ cho Razor Pages và API Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Đăng ký đường dẫn cho Razor Pages và API
app.MapRazorPages();
app.MapControllers();

app.Run();