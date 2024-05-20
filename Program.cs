using BECamp_T13_HW2_Aspnet_AI.Data;
using BECamp_T13_HW2_Aspnet_AI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LoginContext>(
    options => options.UseMySQL(builder.Configuration["MySQL:BECampT13HW2"])
);
builder.Services.AddDbContext<RegisterContext>(
    options => options.UseMySQL(builder.Configuration["MySQL:BECampT13HW2"])
);

// Use Interface to achieve dependency injection.
builder.Services.AddScoped<IAIServices, OpenAIServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();   // Transform the defalut route from single route(MVC) to attribute route(REST API).

app.Run();
