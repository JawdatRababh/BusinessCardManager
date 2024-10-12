using Repository.Interfaces;
using Service.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AttachmentService
    {
        private IRepositoryUnitOfWork _repositoryUnitOfWork;
        private ITPServiceUnitOfWork _tPServiceUnitOfWork;

        public AttachmentService(IRepositoryUnitOfWork repositoryUnitOfWork, ITPServiceUnitOfWork tPServiceUnitOfWork)
        {
            _repositoryUnitOfWork = repositoryUnitOfWork;
            _tPServiceUnitOfWork = tPServiceUnitOfWork;
        }
    }
}
