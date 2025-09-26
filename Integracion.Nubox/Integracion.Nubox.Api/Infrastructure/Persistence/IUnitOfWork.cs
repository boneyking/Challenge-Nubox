using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Integracion.Nubox.Api.Infrastructure.Persistence
{
    public interface IUnitOfWork
    {
        ICompaniaRepository Companias { get; }
        IBitacoraSincronizacionRepository BitacorasSincronizacion { get; }
        ITrabajadorRepository Trabajadores { get; }
        IRegistroAsistenciaRepository RegistrosAsistencia { get; }
        ITransaccionSincronizacionRepository TransaccionesSincronizacion { get; }
        IConfiguracionPartnerRepository ConfiguracionesPartner { get; }
        IResumenAsistenciaRepository ResumenesAsistencia { get; }
        Task<bool> SaveChangesAsync();
        Task<int> SaveChangesAndReturnCountAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        public ICompaniaRepository Companias { get; }
        public IBitacoraSincronizacionRepository BitacorasSincronizacion { get; }
        public ITrabajadorRepository Trabajadores { get; }
        public IRegistroAsistenciaRepository RegistrosAsistencia { get; }
        public ITransaccionSincronizacionRepository TransaccionesSincronizacion { get; }
        public IConfiguracionPartnerRepository ConfiguracionesPartner { get; }
        public IResumenAsistenciaRepository ResumenesAsistencia { get; }


        private readonly IntegracionNuboxContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(IntegracionNuboxContext context,
            ICompaniaRepository companiaRepository,
            IBitacoraSincronizacionRepository bitacoraSincronizacionRepository,
            ITrabajadorRepository trabajadorRepository,
            IRegistroAsistenciaRepository registroAsistenciaRepository,
            ITransaccionSincronizacionRepository transaccionSincronizacionRepository,
            IConfiguracionPartnerRepository configuracionPartnerRepository,
            IResumenAsistenciaRepository resumenAsistenciaRepository)
        {
            _context = context;
            Companias = companiaRepository;
            BitacorasSincronizacion = bitacoraSincronizacionRepository;
            Trabajadores = trabajadorRepository;
            RegistrosAsistencia = registroAsistenciaRepository;
            TransaccionesSincronizacion = transaccionSincronizacionRepository;
            ConfiguracionesPartner = configuracionPartnerRepository;
            ResumenesAsistencia = resumenAsistenciaRepository;
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<int> SaveChangesAndReturnCountAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
