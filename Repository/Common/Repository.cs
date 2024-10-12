using Domin.Common;
using Domin.Context;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Common
{
    public class Repository<IEntity> : IRepository<IEntity> where IEntity : BaseModel, new()
    {
        protected BusinessCardDBContext Context;
        public Repository(BusinessCardDBContext context)
        {
            Context = context;
        }
    }
}
