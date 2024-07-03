using KrizzShop_DataAccess.Repository.IRepository;
using KrizzShop_Models;
using Org.BouncyCastle.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrizzShop_DataAccess.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>,
                                           IInquiryHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public InquiryHeaderRepository(ApplicationDbContext db)
            :base(db)
        {
            _db = db;
        }

        public void Update(InquiryHeader obj)
        {
            _db.InquiryHeaders.Update(obj);
        }
    }
}
