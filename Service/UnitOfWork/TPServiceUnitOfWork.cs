using Service.Interfaces;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.UnitOfWork
{
    public class TPServiceUnitOfWork : ITPServiceUnitOfWork
    {
        private HttpClient _client;

        public Lazy<ITPIntegrationService> TPIntegrationService { get; set; }

        public TPServiceUnitOfWork(HttpClient client)
        {
            TPIntegrationService = new Lazy<ITPIntegrationService>(() => new TPIntegrationService(_client));
        }

        public void Dispose()
        {
            //_client.Dispose();
        }
    }
}
