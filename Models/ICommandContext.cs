using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CmdApi.Models
{
    public interface ICommandContext : IDisposable
    {
        DbSet<CommandContext> CommandItems { get; set; }
        int SaveChanges();
        EntityEntry Entry(object entity);
    }
}
