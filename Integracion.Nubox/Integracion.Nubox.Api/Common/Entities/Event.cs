namespace Integracion.Nubox.Api.Common.Entities
{
    public abstract class Event
    {
        protected Event()
        {
            FechaOcurrido = DateTime.Now;
        }
        public DateTime FechaOcurrido { get; protected set; } = DateTime.Now;
        public abstract string NombreEvento { get; }
        public abstract string Exchange { get; }
    }
}
