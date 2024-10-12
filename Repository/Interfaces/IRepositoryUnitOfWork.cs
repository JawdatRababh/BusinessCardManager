using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IRepositoryUnitOfWork : IDisposable
    {
        Lazy<IBusinessCardRepositories> BusinessCard { get; set; }

        Lazy<IAttachmentRepositories> Attachment { get; set; }

    }
}
