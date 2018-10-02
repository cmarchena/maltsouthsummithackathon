using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

using SECURITY.CORE.Globals;
using SECURITY.MODEL;
using SECURITY.MODEL.Entities;
using SECURITY.MODEL.Interfaces;

namespace SECURITY.CORE.Entities
{
    /// <summary>
    /// Implementación del Interface IIdiomas
    /// </summary>
    public class Audiences : Base, IAudiences
    {
        #region "Constructor"
        public Audiences()
        {
            FieldEquivalents = "";
        }
        #endregion

        #region "Funciones privadas"

        #region "Actualiza entidad"
        /// <summary>
        /// Actualiza los datos anejos del objeto o añade uno nuevo si no existiese
        /// </summary>
        /// <param name="dbModel">pais</param>
        /// <param name="model">modelo</param>
        private void Update(audience dbModel, Audience model)
        {
            var modificando = false;

            //Si la entidad ha sido modificada, le actualizamos la fecha y hora de modificación
            if (Entities.ChangeTracker.Entries().FirstOrDefault(x => x.Entity == dbModel).State == EntityState.Modified)
            {
                modificando = true;
            }

            if (modificando)
            {
                dbModel.ModificationDate = DateTime.Now;
            }
        }
        #endregion

        #region "Listado"
        /// <summary>
        /// Genera la lista de objetos maniqui para devolver la estructura que queramos,
        /// en este caso es una entidad simple pero si este objeto tuviese propiedades complejas las rellenaríamos desde aquí.
        /// Ej: el objeto que tratamos aquí es de tipo Audience, pues podemos rellenar el listado de imagenes por ejemplo
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private List<Audience> Listado(List<audience> result)
        {
            try
            {
                var lst = new List<Audience>();

                foreach (var item in result)
                {
                    #region "Audience"
                    var audience = new Audience
                    {
                        AudienceId = item.InternalId,
                        Name = item.Name,
                        Secret = item.Secret,
                        DaysToExpire = item.DaysToExpire,
                        ExpirationDate = item.ExpirationDate,
                        CreationDate = item.CreationDate,
                        ModificationDate = item.ModificationDate,
                        IsActive = item.IsActive,
                        IsInternal = item.IsInternal
                    };
                    #endregion

                    lst.Add(audience);
                }

                return lst;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        #endregion

        #endregion

        #region "Get"
        async public Task<List<Audience>> Get(string query, string sort = null, string limit = null, string offset = null, string filter = null)
        {
            try
            {
                if (filter != null && filter != string.Empty)
                {
                    if (query != string.Empty)
                    {
                        query = string.Format("({0}) AND ({1}) ", filter, query);
                    }
                    else
                    {
                        query = string.Format("{0}", filter);
                    }
                }

                var sql = "Select * From getaudiences {0} {1} {2} {3}";
                var sqlCount = "Select Count(AudienceId) From getaudiences {0}";

                sql = Queries.GeneraSql(sql, query, sort, limit, offset, FieldEquivalents);
                CountSql = Queries.GeneraCountSql(sqlCount, query, FieldEquivalents);

                var result = await Entities.audiences.SqlQuery(sql).ToListAsync();

                return Listado(result);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        #endregion

        #region "GetById"
        async public Task<Audience> Get(Guid internalId)
        {
            try
            {
                var sql = string.Format("Select * From getaudiences Where InternalId = '{0}'", internalId.ToString());

                var result = await Entities.audiences.SqlQuery(sql).ToListAsync();

                return Listado(result).FirstOrDefault();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        #endregion

        #region "Create"
        async public Task<Audience> Post(Audience model)
        {
            var dbContextTransaction = Entities.Database.BeginTransaction();

            try
            {

                if (Model.IsValid(model))
                {
                    #region "Audience"
                    model.CreationDate = DateTime.Now;

                    var audience = new audience
                    {
                        AudienceId = Guid.NewGuid().ToString(),
                        InternalId = model.InternalId,
                        Name = model.Name,
                        Secret = model.Secret,
                        DaysToExpire = model.DaysToExpire,
                        IsActive = model.IsActive,
                        IsInternal = model.IsInternal,
                        CreationDate = model.CreationDate
                    };

                    if (model.DaysToExpire > 0) {
                        audience.ExpirationDate = DateTime.Now.AddDays(model.DaysToExpire);
                    } 

                    Entities.audiences.Add(audience);
                    #endregion

                    await Entities.SaveChangesAsync();

                    Update(audience, model);

                    await Entities.SaveChangesAsync();

                }

                dbContextTransaction.Commit();

                return model;

            }
            catch (DbEntityValidationException ex)
            {
                throw Model.ManageException(Entities.Database.CurrentTransaction, ex);
            }
            catch (Exception e)
            {
                if (Entities.Database.CurrentTransaction != null) dbContextTransaction.Rollback();
                throw new Exception(string.Format(RESOURCES.Core.ProblemasActualizandoEntradas, e.InnerException.InnerException.Message));
            }

        }
        #endregion

        #region "Update"
        async public Task<Audience> Put(Audience model)
        {
            var dbContextTransaction = Entities.Database.BeginTransaction();

            try
            {

                if (Model.IsValid(model))
                {

                    #region "audience"
                    audience audience = Entities.audiences.FirstOrDefault(x => x.InternalId == model.AudienceId);

                    model.CreationDate = audience.CreationDate;
                    model.ModificationDate = DateTime.Now;

                    audience.Name = model.Name;
                    audience.Secret = model.Secret;
                    audience.DaysToExpire = model.DaysToExpire;
                    audience.IsInternal = model.IsInternal;
                    audience.IsActive = model.IsActive;

                    if (model.DaysToExpire > 0)
                    {
                        audience.ExpirationDate = DateTime.Now.AddDays(model.DaysToExpire);
                    }
                    #endregion

                    Update(audience, model);

                    await Entities.SaveChangesAsync();

                }

                dbContextTransaction.Commit();

                return model;

            }
            catch (DbEntityValidationException ex)
            {
                throw Model.ManageException(Entities.Database.CurrentTransaction, ex);
            }
            catch (Exception e)
            {
                if (Entities.Database.CurrentTransaction != null) dbContextTransaction.Rollback();
                throw new Exception(e.Message);
            }

        }
        #endregion

        #region "Delete"
        async public Task<Audience> Delete(Guid internalId)
        {
            var dbContextTransaction = Entities.Database.BeginTransaction();

            try
            {
                var sql = string.Format("Select * From getaudiences Where InternalId = '{0}'", internalId.ToString());

                var result = await Entities.audiences.SqlQuery(sql).ToListAsync();

                if (result.Count <= 0)
                {
                    throw new Exception(RESOURCES.Core.ElementoNoExistente);
                }

                var audience = Listado(result).FirstOrDefault();

                Entities.audiences.Remove(result.FirstOrDefault());

                await Entities.SaveChangesAsync();

                dbContextTransaction.Commit();

                return audience;

            }
            catch (Exception e)
            {
                if (Entities.Database.CurrentTransaction != null) dbContextTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }
        #endregion

    }
}
