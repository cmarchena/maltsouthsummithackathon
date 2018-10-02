using System.Security.Principal;

namespace SECURITY.MODEL.Interfaces
{
    /// <summary>
    /// Define el Interface para la gestión de Areas
    /// </summary>
    public interface IBase
    {
        #region "Miembros"
        securityEntities Entities { get; set; }

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
