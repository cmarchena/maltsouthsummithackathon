using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace ePassport.CORE.Globals
{

    public static class Queries
    {
        /// <summary>
        /// Genera la consulta reemplazando los comodines utilizados en el QueryString["q"]
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string GeneraQuery(string query)
        {
            if (query == null) return "";

            query = query.Replace("ct:", "LIKE ");
            query = query.Replace("*", "%");
            query = query.Replace("eq:", "=");
            query = query.Replace("eq!", "<>");
            query = query.Replace("<:", "<=");
            query = query.Replace(">:", ">=");

            return query;
        }

        /// <summary>
        /// Sincroniza los nombres de campo de los objetos Dummies con la consulta en base de datos
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fieldEquivalents">usuario=username,nombre=name</param>
        /// <returns></returns>
        public static string GeneraEquivalencias(string query, string fieldEquivalents = null, char separator = ',')
        {
            if ((fieldEquivalents != null && fieldEquivalents != string.Empty) && query != null)
            {
                foreach (var item in fieldEquivalents.Split(separator))
                {
                    string[] equivalent = item.Split('=');

                    query = query.ToLower().Replace(equivalent[0].ToLower(), equivalent[1].ToLower());
                }
            }

            return query;
        }

        /// <summary>
        /// Genera la consulta para el count de los elementos devueltos para poder paginar correctamente
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="query"></param>
        /// <param name="fieldEquivalents"></param>
        /// <returns></returns>
        public static string GeneraCountSql(string sql, string query, string fieldEquivalents = null)
        {
            query = GeneraEquivalencias(query, fieldEquivalents);
            query = GeneraQuery(query);

            sql = string.Format(sql, query != "" ? "WHERE " + query : "");

            return sql;
        }

        /// <summary>
        /// Genera la consulta que se enviará a la base de datos después de aplicarle las correcciones necesarias
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="fieldEquivalents"></param>
        /// <returns></returns>
        public static string GeneraSql(string sql, string query, string sort = null, string limit = null, string offset = null, string fieldEquivalents = null)
        {
            sort = GeneraEquivalencias(sort, fieldEquivalents);

            query = GeneraEquivalencias(query, fieldEquivalents);
            query = GeneraQuery(query);

            sql = string.Format(sql, query != "" ? "WHERE " + query : "",
                                     (sort != null) && (sort != string.Empty) ? "ORDER BY " + sort : "",
                                     limit != null ? "LIMIT " + limit : "",
                                     offset != null && limit != null ? "OFFSET " + ((int.Parse(offset) - 1) * int.Parse(limit)) : "");

            return sql;
        }

    }

    public static class Model
    {
        /// <summary>
        /// Verifica que los datos del modelo es válido
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsValid(object model)
        {
            if (model == null)
            {
                throw new Exception("Modelo vacío");
            }

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            if (Validator.TryValidateObject(model, context, results, true))
            {
                return true;
            }

            throw new DbEntityValidationException(ValidationResults(results));

        }

        /// <summary>
        /// Devuelve los errores de validación del modelo
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static string ValidationResults(List<ValidationResult> results)
        {
            var sb = new StringBuilder();

            foreach (var item in results)
            {
                sb.Append(string.Format("{0}",item.ErrorMessage));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Maneja la transacción en curso cuando hay problemas de validación en la entidad
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static DbEntityValidationException ManageException(DbContextTransaction transaction, DbEntityValidationException ex)
        {
            if (transaction != null) transaction.Rollback();

            return new DbEntityValidationException(ex.Message);
        }
    }
}
