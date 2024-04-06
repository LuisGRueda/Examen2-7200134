using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examen2_7200134.Implementacion
{
    public class ClienteLogico
    {
        private readonly Contexto contexto;

        public ClienteLogico(Contexto contexto)
        {
            this.contexto = contexto;
        }

    }
}
