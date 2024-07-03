using KrizzShop_Models;
using KrizzShop_DataAccess.Repository.IRepository;

namespace KrizzShop_DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>,
                                      ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db)
            : base(db) 
        {
            _db = db;
        }

        public void Update(Category obj)
        {
            var objFromDb = base
                .FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb is not null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.DisplayOrder = obj.DisplayOrder;
            }
        }
    }
}
