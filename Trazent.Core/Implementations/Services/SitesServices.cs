using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using Trazent.Api.Core.Implementations.Repositories;
using Trazent.Api.DataAccess;
using Trazent.Core.Infraestructure.Services;

namespace Trazent.Core.Implementations.Services
{
    public class SitesServices : ISitesServices
    {
        private readonly ICoreUnitRepositories _repository;
        private readonly IAppDbContext _dbContext;

        public SitesServices(ICoreUnitRepositories repository, IAppDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        
       


    }
}
