using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Baselib.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(
            "Server=localhost;Port=3306;Database=baselib;User=root;Password=;",
            new MySqlServerVersion(new Version(8, 0, 0)));
        return new AppDbContext(optionsBuilder.Options);
    }
}