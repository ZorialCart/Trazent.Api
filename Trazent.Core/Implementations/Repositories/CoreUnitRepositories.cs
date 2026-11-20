using Npgsql;
using Trazent.Api.Core.Infraestructure.Repositories;
using Trazent.Core.Infraestructure.Repositories;

namespace Trazent.Api.Core.Implementations.Repositories;

public class CoreUnitRepositories : ICoreUnitRepositories
{
    private NpgsqlConnection _connection = default!;
    private NpgsqlTransaction? _transaction;
    public ICompaniesRepository CompaniesRepository { get; }
    public ISitesRepository SitesRepository { get; }

    public CoreUnitRepositories(ICompaniesRepository companiesRepository, ISitesRepository sitesRepository)
    {
        CompaniesRepository = companiesRepository;
        SitesRepository = sitesRepository;
    }

    public void SetConnection(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _transaction = transaction;

        CompaniesRepository.SetConnection(connection, transaction);
        SitesRepository.SetConnection(connection, transaction);

    }
}