using Microsoft.EntityFrameworkCore;

namespace LogicParser.Api.Data;

public class LogicParserContext(DbContextOptions<LogicParserContext> options) : DbContext(options)
{

    //public DbSet<User> Users { get; set; }
}
