using KrizzShop_DataAccess.Repository.IRepository;
using KrizzShop_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrizzShop_DataAccess.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>,
                                             IApplicationTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationTypeRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationType obj)
        {
            var objFromDb = base
                .FirstOrDefault(a => a.Id == obj.Id);

            if (objFromDb is not null) 
            {
                obj.Name = objFromDb.Name;
            }
        }
    }
}
