using Domin.Context;
using Domin.Models;
using Repository.Common;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class AttachmentRepositories: Repository<Attachment>, IAttachmentRepositories
    {
        private BusinessCardDBContext _context;
        public AttachmentRepositories(BusinessCardDBContext context) : base(context)
        {
            _context = context;
        }

    }
}
