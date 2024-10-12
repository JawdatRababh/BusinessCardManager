using Domin.Context;
using Repository.Interfaces;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfWork
{
    public class RepositoryUnitOfWork : IRepositoryUnitOfWork
    {
        private BusinessCardDBContext _context;
        public Lazy<IBusinessCardRepositories> BusinessCard { get; set; }
        public Lazy<IAttachmentRepositories> Attachment { get; set; }

        public RepositoryUnitOfWork(BusinessCardDBContext context)
        {
            _context = context;
            BusinessCard = new Lazy<IBusinessCardRepositories>(() => new BusinessCardRepositories(_context));
            Attachment = new Lazy<IAttachmentRepositories>(() => new AttachmentRepositories(_context));

        }
        public void Dispose()
        {
            _context.Dispose();
            BusinessCard = null;
            Attachment = null;
        }
    }
}
