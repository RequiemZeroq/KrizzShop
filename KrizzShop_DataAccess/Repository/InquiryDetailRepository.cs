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
    public class InquiryDetailRepository : Repository<InquiryDetail>,
                                           IInquiryDetailRepository
    {
        private readonly ApplicationDbContext _db;

        public InquiryDetailRepository(ApplicationDbContext db)
            :base(db)
        {
            _db = db;
        }

        public void Update(InquiryDetail obj)
        {
            _db.InquiryDetails.Update(obj);
        }

    }
}
