using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Baselib.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(
            "Server=localhost;Database=mugla;User=root;Password=;Port=3306;Pooling=true;MinPoolSize=10;MaxPoolSize=200;ConnectionTimeout=60;DefaultCommandTimeout=300;AllowUserVariables=true;UseAffectedRows=false;ConvertZeroDateTime=true;AllowZeroDateTime=true;",
            ServerVersion.AutoDetect("Server=localhost;Database=mugla;User=root;Password=;Port=3306;Pooling=true;MinPoolSize=10;MaxPoolSize=200;ConnectionTimeout=60;DefaultCommandTimeout=300;AllowUserVariables=true;UseAffectedRows=false;ConvertZeroDateTime=true;AllowZeroDateTime=true;"));
        return new AppDbContext(optionsBuilder.Options);
    }
}