using SECURITY.MODEL;
using System.Web;
using System.Web.Http;



namespace SECURITY.API.Controllers
{
    public class BaseController : ApiController
    {
        #region "Miembros"
        protected readonly securityEntities Entities;

        #region "Propiedades para las consultas"
        protected string Query
        {
            get
            {
                return HttpContext.Current.Request.QueryString["q"];
            }
        }

        protected string Sort
        {
            get
            {
                return HttpContext.Current.Request.QueryString["sort"];
            }
        }

        protected string Limit
        {
            get
            {
                var v = HttpContext.Current.Request.QueryString["limit"];
                return v ?? "1";
            }
        }

        protected string Offset
        {
            get
            {
                var v = HttpContext.Current.Request.QueryString["offset"];
                return v ?? "1";
            }
        }

        protected string Filter
        {
            get
            {
                return HttpContext.Current.Request.QueryString["filter"];
            }
        }
        #endregion

        #endregion

        public BaseController()
        {
            Entities = new securityEntities();
        }
    }
}
