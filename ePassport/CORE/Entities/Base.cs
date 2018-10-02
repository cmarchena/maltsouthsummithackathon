using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;

using ePassport.MODEL;

namespace ePassport.CORE.Entities
{
    /// <summary>
    /// Clase Base para las entidades
    /// </summary>
    public class Base
    {

        #region "Miembros"

        //dbContext sobre el que realizaremos la petición
        public hckt_epassportEntities  Entities { get; set; }

        //Código de Idioma que genera la petición
        public string CodigoIdioma { get; set; }

        //Usuario que levanta la instancia
        public IPrincipal User { get; set; }

        //Equivalencias de nombres de campos en base de datos y su definición en
        //ePassport.MODEL.Entities, .... si se define un buen modelo y sistema de vistas
        //esta característica se podría deshechar en futuras versiones, pero de momento 
        //la dejamos porque podría ser util.
        public string FieldEquivalents { get; set; }

        //Consulta de recuento para la consulta que se peticiona
        //Con esto realizaremos la paginación en consultas complejas
        public string CountSql { get; set; }

        //Devuelve el recuento
        public int Count()
        {
            try
            {
                return Entities.Database.SqlQuery<int>(CountSql).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

    }
}
