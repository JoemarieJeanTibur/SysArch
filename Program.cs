using Microsoft.EntityFrameworkCore;
using Tibur_LabAct1.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ ADD SESSION
builder.Services.AddSession();

var app = builder.Build();

// ✅ TEMPORARY FIX - remove this block after running the app once
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.ExecuteSqlRaw(@"
        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE MigrationId = '20260313033847_FixIdNumber')
        INSERT INTO [__EFMigrationsHistory] VALUES ('20260313033847_FixIdNumber', '10.0.5');
        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE MigrationId = '20260313035018_UpdateUserModel')
        INSERT INTO [__EFMigrationsHistory] VALUES ('20260313035018_UpdateUserModel', '10.0.5');
        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE MigrationId = '20260327002018_AddProfilePicture')
        INSERT INTO [__EFMigrationsHistory] VALUES ('20260327002018_AddProfilePicture', '10.0.5');
        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE MigrationId = '20260327013011_AddRemainingSession')
        INSERT INTO [__EFMigrationsHistory] VALUES ('20260327013011_AddRemainingSession', '10.0.5');
        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE MigrationId = '20260327013938_AddSitInTable')
        INSERT INTO [__EFMigrationsHistory] VALUES ('20260327013938_AddSitInTable', '10.0.5');
        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE MigrationId = '20260327014820_RemainingSessionToUser')
        INSERT INTO [__EFMigrationsHistory] VALUES ('20260327014820_RemainingSessionToUser', '10.0.5');
        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE MigrationId = '20260417003244_AddAnnouncementsTable')
        INSERT INTO [__EFMigrationsHistory] VALUES ('20260417003244_AddAnnouncementsTable', '10.0.5');
    ");
}
// ✅ END TEMPORARY FIX

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ IMPORTANT
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();