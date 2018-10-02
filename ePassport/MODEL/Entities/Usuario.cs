using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePassport.MODEL.Entities
{
    /// <summary>
    /// Usuarios de ePassport
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador del elemento
        /// </summary>
        public string UsuarioId { get; set; }

        public string ePassport { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
