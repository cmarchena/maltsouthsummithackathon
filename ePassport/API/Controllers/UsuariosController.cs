using ePassport.API.Globals;
using ePassport.CORE.Interfaces;
using ePassport.MODEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ePassport.API.Controllers
{

    /// <summary>
    /// Usuarios ePassport
    /// </summary>
    [RoutePrefix("{lang}/api/v1.0")]
    public class UsuariosController : BaseController
    {
        #region "Constructores"
        protected readonly IUsuarios Store;

        public UsuariosController(IUsuarios store)
        {
            Store = store;
            Store.Entities = Entities;
            Store.User = User;
        }
        #endregion

        #region "Get"
        /// <summary>
        /// Devuelve los usuarios que cumplan con la consulta
        /// </summary>
        /// <remarks>
        /// <p>Devuelve un Json con los usuarios en <b>results</b> y la gestión de la paginación en <b>metadata</b>.</p>
        /// <br/>
        /// <p><b>GET </b> /es-ES/api/v1.0/usuarios?q=ePassport+ct:'*ePassport*'&amp;sort=nombre+asc,direccion+desc&amp;limit=25&amp;offset=1</p>
        /// <br/>
        /// <p><b>Query String Params</b></p>
        /// <ul>
        /// <li><b><u>q (Query):</u> </b> En este parámetro podemos definir con algunos metas que filas queremos que nos devuelva el servicio definiendo patrones de búsqueda o comparación.
        ///     <ul>
        ///         <li><b>ct (Contains):</b> q=Campo1 ct:'comienza por\*' AND Campo2 ct:'\*finaliza por' OR Campo3 ct:'\*contiene\*'</li>
        ///         <li><b>eq: (Equals):</b> q=Campo1 eq:'texto' AND Campo2 eq:0</li>
        ///         <li><b>eq! (Not Equals):</b> q=Campo1 eq!'texto' AND Campo2 eq!0</li>
        ///         <li><b>&lt;: (Lower than):</b> q=Campo1 &lt;: 1</li>
        ///         <li><b>&gt;: (Greater than):</b> q=Campo1 &gt;: 1</li>
        ///     </ul>
        /// </li>
        /// <li><b><u>sort (Ordenación):</u> </b> Definimos como queremos ordenar los resultados. <br/> sort=Campo1 asc,Campo2 desc</li>
        /// <li><b><u>limit (Límite filas devueltas):</u> </b> Límite para la paginación. Estableciendo este límite, la paginación se realiza de forma automática. El servicio nos devolverá los enlaces de las páginas <b>self</b>, <b>previous</b>, <b>next</b><br/>limit=25</li>
        /// <li><b><u>offset (Página):</u> </b> Página actual.<br/>offset=1</li>
        /// </ul>
        /// </remarks> 
        /// <response code="200">Response Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        /// <returns>Json con </returns>
        [Globals.Security.Authorization(AppSettingRoles = "roles:administracion")]
        [Route("usuarios")]
        [HttpGet]
        [ResponseType(typeof(List<Usuario>))]
        async public Task<IHttpActionResult> Get()
        {
            try
            {
                //Establece el idioma
                Store.CodigoIdioma = RequestContext.RouteData.Values["lang"].ToString();

                var results = await Store.Get(Query, Sort, Limit, Offset, Filter);

                var response = new Response(true, HttpStatusCode.OK);
                var metadata = new Metadata(Store.Count(), int.Parse(Limit), int.Parse(Offset), results.Count());

                return Json(new
                {
                    results,
                    success = response.Success,
                    metadata
                });
            }
            catch (Exception e)
            {
                return Json(new Response(false, HttpStatusCode.InternalServerError, e.Message));
            }
        }
        #endregion

        #region "GetById"
        /// <summary>
        /// Devuelve un área por su identificador
        /// </summary>
        /// <param name="ePassport">ePassport</param>
        /// <response code="200">Response Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        /// <returns></returns>
        [Route("usuarios/{ePassport:Guid}")]
        [HttpGet]
        [ResponseType(typeof(List<Usuario>))]
        async public Task<IHttpActionResult> Get(Guid ePassport)
        {
            try
            {
                //Establece el idioma
                Store.CodigoIdioma = RequestContext.RouteData.Values["lang"].ToString();

                var results = await Store.Get(ePassport);
                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                return Json(new Response(false, HttpStatusCode.InternalServerError, e.Message));
            }
        }
        #endregion

        #region "Create"
        /// <summary>
        /// Crea un nuevo usuario de ePassport
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Response Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        /// <returns></returns>
        [Globals.Security.Authorization(AppSettingRoles = "roles:registrador")]
        [Route("usuarios")]
        [HttpPost]
        [ResponseType(typeof(List<Usuario>))]
        async public Task<IHttpActionResult> Post(Usuario model)
        {
            try
            {
                //Establece el idioma
                Store.CodigoIdioma = RequestContext.RouteData.Values["lang"].ToString();

                var results = await Store.Post(model);
                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                return Json(new Response(false, HttpStatusCode.InternalServerError, e.Message));
            }
        }
        #endregion

        #region "Update"
        /// <summary>
        /// Actualiza el Usuario
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Response Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        /// <returns></returns>
        [Globals.Security.Authorization(AppSettingRoles = "roles:administracion")]
        [Route("usuarios")]
        [HttpPut]
        [ResponseType(typeof(List<Usuario>))]
        async public Task<IHttpActionResult> Put(Usuario model)
        {
            try
            {
                //Establece el idioma
                Store.CodigoIdioma = RequestContext.RouteData.Values["lang"].ToString();

                var results = await Store.Put(model);
                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                return Json(new Response(false, HttpStatusCode.InternalServerError, e.Message));
            }
        }
        #endregion

        #region "Delete"
        /// <summary>
        /// Elimina el Usuario por su ePassport
        /// </summary>
        /// <param name="ePassport"></param>
        /// <returns></returns>
        [Globals.Security.Authorization(AppSettingRoles = "roles:administracion")]
        [Route("usuarios")]
        [HttpDelete]
        [ResponseType(typeof(List<Usuario>))]
        async public Task<IHttpActionResult> Delete(Guid ePassport)
        {
            try
            {
                //Establece el idioma
                Store.CodigoIdioma = RequestContext.RouteData.Values["lang"].ToString();

                var results = await Store.Delete(ePassport);
                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                return Json(new Response(false, HttpStatusCode.InternalServerError, e.Message));
            }
        }
        #endregion
    }
}
