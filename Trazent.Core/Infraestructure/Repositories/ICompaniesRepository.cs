using System;
using System.Collections.Generic;
using System.Text;
using Trazent.DataAccess;
using Trazent.Infraestructure.Common.Entities;

namespace Trazent.Api.Core.Infraestructure.Repositories
{
    public interface ICompaniesRepository : IRepositoryBase
    {
        Task<List<Companies>> GetAlltCompaniesAsync();
        Task UpdateCompaniesAsync(Companies company);
        Task<Companies> GetCompanyById(Guid id, bool? isActive = null);
        Task<bool> ChangeIsActiveByIdAsync(Guid id);
        Task<string> CreateCompanyAsync(Companies company);

    }
}
