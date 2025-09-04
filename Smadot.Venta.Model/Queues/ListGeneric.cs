namespace Smadot.Venta.Model.Queues
{
    public class ListGeneric : IListGeneric
    {
        private List<long> colaImpresion = new();
        private List<long> colaFolios = new();
        private List<long> colaVerificacion = new();

        public List<long> ColaImpresion { get => colaImpresion; set => colaImpresion = value; }
        public List<long> ColaFolios { get => colaFolios; set => colaFolios = value; }
        public List<long> ColaVerificacion { get => colaVerificacion; set => colaVerificacion = value; }

        public ListGeneric()
        {
        }

        public void AddColaImpresion(long id)
        {
            lock (ColaImpresion)
            {
                if (!ColaImpresion.Any(x => x == id))
                    ColaImpresion.Add(id);
            }
        }
        public void AddColaVerificacion(long id)
        {
            lock (ColaVerificacion)
            {
                if (!ColaVerificacion.Any(x => x == id))
                    ColaVerificacion.Add(id);
            }
        }

        public void AddColaFolios(long id)
        {
            lock (ColaFolios)
            {
                if (!ColaFolios.Any(x => x == id))
                    ColaFolios.Add(id);
            }
        }

        public void RemoveColaImpresion(long id)
        {
            lock (ColaImpresion)
            {
                if (ColaImpresion.Any(x => x == id))
                    ColaImpresion.Remove(id);
            }
        }

        public void RemoveColaVerificacion(long id)
        {
            lock (ColaVerificacion)
            {
                if (ColaVerificacion.Any(x => x == id))
                    ColaVerificacion.Remove(id);
            }
        }

        public void RemoveColaFolios(long id)
        {
            lock (ColaFolios)
            {
                if (ColaFolios.Any(x => x == id))
                    ColaFolios.Remove(id);
            }
        }
        public List<long> GetColaFolio()
        {
            return ColaFolios;
        }
        public List<long> GetColaVerifcacion() => ColaVerificacion;

        public List<long> GetColaImpresion() => ColaImpresion;

    }

    public interface IListGeneric
    {
        void AddColaImpresion(long id);
        void AddColaVerificacion(long id);
        void AddColaFolios(long id);
        void RemoveColaImpresion(long id);
        void RemoveColaVerificacion(long id);
        void RemoveColaFolios(long id);
        List<long> GetColaFolio();
        List<long> GetColaVerifcacion();
        List<long> GetColaImpresion();
    }
}
