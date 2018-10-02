using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ePassport.MODEL;

namespace TESTING
{
    [TestClass]
    public class Usuarios
    {
        private ePassport.CORE.Entities.Usuarios Entidad { get; set; }

        public Usuarios()
        {
            Entidad = new ePassport.CORE.Entities.Usuarios
            {
                Entities = new hckt_epassportEntities()
            };
        }

        [TestMethod]
        public async Task PostAsync()
        {
            try
            {
                var usuario = await Entidad.Post(new ePassport.MODEL.Entities.Usuario
                {
                    FechaCreacion = DateTime.Now
                });
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
