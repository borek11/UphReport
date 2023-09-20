using UphReport.Data;
using UphReport.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UphReport.Seeder
{
    public class UphSeeder
    {
        private readonly MyDbContext _dbContext;

        public UphSeeder(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {

                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "Admin"
                },
                new Role()
                {
                    Name = "User"
                },
                new Role()
                {
                    Name = "Blocked"
                }
            };
            return roles;
        }
    }
}
