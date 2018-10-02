using SECURITY.MODEL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SECURITY.MODEL.Interfaces
{
    /// <summary>
    /// Define el Interface para la gestión de Categorias
    /// </summary>
    public interface IAudiences : IBase
    {
        #region "Get"
        /// <summary>
        /// Devuelve <see cref="List{Audience}"/> que coincidan con la query.
        /// </summary>
        /// <param name="query">campo eq:'string' or campo eq:numero and campo ct:'*contiene*' or campo ct:'comienza*' or campo eq!'diferente'</param>
        /// <param name="sort">campo1 asc, campo2 desc, campo3 asc</param>
        /// <param name="limit">Limita el número de filas devueltas</param>
        /// <param name="offset">Página</param>
        /// <param name="filter">Filtro predeterminado de la consulta</param>
        /// <returns><see cref="List{Audience}"/></returns>
        /// <remarks>Está preparada para paginación</remarks>
        Task<List<Audience>> Get(string query, string sort = null, string limit = null, string offset = null, string filter = null);
        #endregion

        #region "GetById"
        /// <summary>
        /// Devuelve <see cref="Audience"/> cuyo internalId coincida
        /// </summary>
        /// <param name="internalId">Key</param>
        /// <returns><see cref="Audience"/></returns>
        Task<Audience> Get(Guid internalId);
        #endregion

        #region "Create"
        /// <summary>
        /// Crea un <see cref="Audience"/>
        /// </summary>
        /// <param name="model"><see cref="List{Audience}"/></param>
        /// <returns><see cref="Audience"/></returns>
        Task<Audience> Post(Audience model);
        #endregion

        #region "Update"
        /// <summary>
        /// Actualiza un <see cref="Audience"/>
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="List{Audience}"/></returns>
        Task<Audience> Put(Audience model);
        #endregion

        #region "Delete"
        /// <summary>
        /// Elimina una <see cref="Audience"/>
        /// </summary>
        /// <param name="internalId"></param>
        /// <returns><see cref="List{Audience}"/></returns>
        Task<Audience> Delete(Guid internalId);
        #endregion
    }
}
