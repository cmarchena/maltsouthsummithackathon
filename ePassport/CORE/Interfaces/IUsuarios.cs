using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Principal;
using ePassport.MODEL.Entities;

namespace ePassport.CORE.Interfaces
{
    /// <summary>
    /// Define el Interface para la gestión de Usuarios
    /// </summary>
    public interface IUsuarios : IBase
    {
        #region "Get"
        /// <summary>
        /// Devuelve <see cref="List{Usuario}"/> que coincidan con la query.
        /// </summary>
        /// <param name="query">campo eq:'string' or campo eq:numero and campo ct:'*contiene*' or campo ct:'comienza*' or campo eq!'diferente'</param>
        /// <param name="sort">campo1 asc, campo2 desc, campo3 asc</param>
        /// <param name="limit">Limita el número de filas devueltas</param>
        /// <param name="offset">Página</param>
        /// <param name="filter">Filtro predeterminado de la consulta</param>
        /// <returns><see cref="List{Usuario}"/></returns>
        /// <remarks>Está preparada para paginación</remarks>
        Task<List<Usuario>> Get(string query, string sort = null, string limit = null, string offset = null, string filter = null);
        #endregion

        #region "GetById"
        /// <summary>
        /// Devuelve <see cref="Usuario"/> cuyo internalId coincida
        /// </summary>
        /// <param name="internalId">Key</param>
        /// <returns><see cref="Usuario"/></returns>
        Task<Usuario> Get(Guid internalId);
        #endregion

        #region "Create"
        /// <summary>
        /// Crea un <see cref="Usuario"/>
        /// </summary>
        /// <param name="model"><see cref="List{Usuario}"/></param>
        /// <returns><see cref="Usuario"/></returns>
        Task<Usuario> Post(Usuario model);
        #endregion

        #region "Update"
        /// <summary>
        /// Actualiza un <see cref="Usuario"/>
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="List{Usuario}"/></returns>
        Task<Usuario> Put(Usuario model);
        #endregion

        #region "Delete"
        /// <summary>
        /// Elimina una <see cref="Usuario"/>
        /// </summary>
        /// <param name="internalId"></param>
        /// <returns><see cref="List{Usuario}"/></returns>
        Task<Usuario> Delete(Guid internalId);
        #endregion
    }
}
