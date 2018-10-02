using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using ePassport.CORE.Interfaces;
using ePassport.MODEL.Entities;
using ePassport.MODEL;
using ePassport.CORE.Globals;

namespace ePassport.CORE.Entities
{
    /// <summary>
    /// Gestión de la entidad Usuarios ePassport
    /// </summary>
    public class Usuarios : Base, IUsuarios
    {

        #region "Constructor"
        public Usuarios()
        {
            FieldEquivalents = "";
        }
        #endregion

        public Task<Usuario> Delete(Guid internalId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> Get(string query, string sort = null, string limit = null, string offset = null, string filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> Get(Guid internalId)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> Post(Usuario model)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> Put(Usuario model)
        {
            throw new NotImplementedException();
        }
    }

}