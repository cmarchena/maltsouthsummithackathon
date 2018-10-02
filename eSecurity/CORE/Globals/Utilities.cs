using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;

using SECURITY.CORE.Entities;
using SECURITY.MODEL.Entities;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Text;

namespace SECURITY.CORE.Globals
{    
    public class DbContext : IdentityDbContext<User>
    {
        public DbContext() : base("IdentityConnection", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Crea el contexto
        /// </summary>
        /// <returns></returns>
        public static DbContext Create()
        {
            var context = new DbContext();

            if (context.Database.CreateIfNotExists())
            {
                new DbInitializer().Seed(context);
            }

            return context;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Audience>().ToTable("Audiences").Property(c => c.AudienceId).HasMaxLength(36).IsRequired();
                modelBuilder.Entity<Audience>().ToTable("Audiences").Property(c => c.InternalId).HasMaxLength(36).IsRequired();
                modelBuilder.Entity<Audience>().ToTable("Audiences").Property(c => c.Secret).HasMaxLength(128).IsRequired();

                modelBuilder.Entity<User>().ToTable("Users").Property(p => p.Id).HasMaxLength(128).HasColumnName("UserId");
                modelBuilder.Entity<User>().ToTable("Users").Property(p => p.AudienceId).HasMaxLength(36).IsRequired();
                modelBuilder.Entity<User>().ToTable("Users").Property(c => c.UserName).HasMaxLength(128).IsRequired();

                modelBuilder.Entity<IdentityRole>().ToTable("Roles").Property(p => p.Id).HasMaxLength(128).HasColumnName("RoleId");
                modelBuilder.Entity<IdentityRole>().ToTable("Roles").Property(c => c.Name).HasMaxLength(128).IsRequired();

                modelBuilder.Entity<IdentityUserRole>().ToTable("UsersRoles").Property(p => p.UserId).HasMaxLength(128).IsRequired();
                modelBuilder.Entity<IdentityUserRole>().ToTable("UsersRoles").Property(p => p.RoleId).HasMaxLength(128).IsRequired();

                modelBuilder.Entity<IdentityUserLogin>().ToTable("UsersLogins").Property(p => p.UserId).HasMaxLength(128).IsRequired();

                modelBuilder.Entity<IdentityUserClaim>().ToTable("UsersClaims").Property(p => p.Id).HasColumnName("UserClaimId");
                modelBuilder.Entity<IdentityUserClaim>().ToTable("UsersClaims").Property(p => p.UserId).HasMaxLength(128).IsRequired();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        //protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        //{
        //    var entity = entityEntry;

        //    if (entityEntry != null && entityEntry.State == EntityState.Added)
        //    {
        //        var errors = new List<DbValidationError>();
        //        var user = entityEntry.Entity as User;

        //        if (user != null)
        //        {
        //            if (this.Users.Any(u => string.Equals(u.UserName, user.UserName)
        //              && u.AudienceId == user.AudienceId))
        //            {
        //                errors.Add(new DbValidationError("User",
        //                  string.Format("Username {0} is already taken for AppId {1}",
        //                    user.UserName, user.AudienceId)));
        //            }

        //            if (this.RequireUniqueEmail
        //              && this.Users.Any(u => string.Equals(u.Email, user.Email)
        //              && u.AudienceId == user.AudienceId))
        //            {
        //                errors.Add(new DbValidationError("User",
        //                  string.Format("Email Address {0} is already taken for AppId {1}",
        //                    user.UserName, user.AudienceId)));
        //            }
        //        }
        //        else
        //        {
        //            var role = entityEntry.Entity as IdentityRole;

        //            if (role != null && this.Roles.Any(r => string.Equals(r.Name, role.Name)))
        //            {
        //                errors.Add(new DbValidationError("Role",
        //                  string.Format("Role {0} already exists", role.Name)));
        //            }
        //        }
        //        if (errors.Any())
        //        {
        //            return new DbEntityValidationResult(entityEntry, errors);
        //        }
        //    }

        //    return new DbEntityValidationResult(entityEntry, new List<DbValidationError>());
        //}
    }

    public class DbInitializer : CreateDatabaseIfNotExists<DbContext>
    {
        internal async void Seed(DbContext context)
        {
            var startup = new Startups();

            startup.DbContext = context;
            startup.RoleStore = new RoleManager<Role>(new RoleStore<Role>(new DbContext()));
            startup.UserStore = new UserManager<User>(new UserStore<User>(new DbContext()));

            await startup.Init();
        }
    }

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
                throw new Exception(RESOURCES.Core.ModeloVacio);
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
                sb.Append(string.Format("{0}", item.ErrorMessage));
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
