using Domin.Context;
using Repository.Interfaces;
using Repository.UnitOfWork;
using Service.Interfaces;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.UnitOfWork
{
    public class ServiceUnitOfWork : IServiceUnitOfWork
    {
        private IRepositoryUnitOfWork _repositoryUnitOfWork;
        public ITPServiceUnitOfWork _tPServiceUnitOfWork { get; set; }
        public Lazy<IBusinessCardService> BusinessCard { get; set; }
        public Lazy<IAttachmentService> Attachment { get; set; }

        public ServiceUnitOfWork(BusinessCardDBContext context)
        {
            _repositoryUnitOfWork = new RepositoryUnitOfWork(context);

            BusinessCard = new Lazy<IBusinessCardService>(() => new BusinessCardService(_repositoryUnitOfWork, _tPServiceUnitOfWork));
         //   Attachment = new Lazy<IAttachmentService>(() => new AttachmentService(_repositoryUnitOfWork, _tPServiceUnitOfWork));

        }
        
        public void Dispose()
        {
        }
    }
}
