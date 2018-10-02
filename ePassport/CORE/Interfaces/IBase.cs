using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Principal;
using ePassport.MODEL;
//using ePassport.MODEL.Entities;

namespace ePassport.CORE.Interfaces
{
    /// <summary>
    /// Define el Interface para la gestión de Areas
    /// </summary>
    public interface IBase
    {
        #region "Miembros"
        hckt_epassportEntities Entities { get; set; }

        string CodigoIdioma { get; set; }

        IPrincipal User { get; set; }

        string CountSql { get; set; }

        string FieldEquivalents { get; set; }

        /// <summary>
        /// Devuelve el recuento de la consulta para realizar las paginaciones
        /// </summary>
        /// <returns></returns>
        int Count();
        #endregion
    }
}
