using Microsoft.EntityFrameworkCore;

using SciMaterials.LinkSearch.WebAPI.Data;
using SciMaterials.LinkSearch.WebAPI.Data.Interfaces;
using SciMaterials.LinkSearch.WebAPI.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LinkSearchDbContextApp>(options =>
{
    options.UseSqlServer(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);
});

builder.Services.AddControllers();

builder.Services.AddScoped<ILinkSearch, LinkSearchRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();