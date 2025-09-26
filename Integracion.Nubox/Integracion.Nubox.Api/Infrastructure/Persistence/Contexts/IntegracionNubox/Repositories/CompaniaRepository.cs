using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories
{
    public class CompaniaRepository : Repository<Compania, Guid>, ICompaniaRepository
    {
        public CompaniaRepository(IntegracionNuboxContext context) : base(context)
        {
        }
    }

}
