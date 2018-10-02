using System.Net;
using System.Web;

namespace ePassport.API.Globals
{

    public class Status
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Devuelve el estado de la respuesta HTTP
    /// </summary>
    public class Response
    {
        public bool Success { get; set; }

        public Status Status { get; set; }

        public Response(bool success, HttpStatusCode code, string message = "")
        {
            Success = success;
            Status = new Status
            {
                Code = code,
                Message = message
            };
        }
    }

    /// <summary>
    /// Gestiona la paginación de los elementos de la consulta
    /// </summary>
    public class Metadata
    {

        public string Url { get; set; }

        public int Total { get; }
        public int Filtered { get; }
        public int Limit { get; }
        public int Offset { get; }

        public int Offsets { get; }

        public string Self
        {
            get { return Url + "&limit=" + Limit + "&offset=" + Offset; }
        }

        public string Next
        {
            get
            {
                if (Offset < (Offsets))
                {
                    int offset = Offset + 1;
                    return Url + "&limit=" + Limit + "&offset=" + offset;
                }
                return "";
            }
        }

        public string Previous
        {
            get
            {
                if (Offset > 1)
                {
                    int offset = Offset - 1;
                    return Url + "&limit=" + Limit + "&offset=" + offset;
                }

                return "";
            }
        }

        public Metadata(int total, int limit, int offset, int filtered)
        {
            Url = HttpContext.Current.Request.Url.Scheme + "://" +
                  HttpContext.Current.Request.Url.Authority +
                  HttpContext.Current.Request.CurrentExecutionFilePath + "?q=" +
                  HttpContext.Current.Request.QueryString["q"];

            Total = total;
            Limit = limit;
            Offset = offset;

            Filtered = filtered;

            if (total > limit)
            {
                Offsets = total / limit;

                if (total % limit != 0)
                {
                    Offsets++;
                }
            }
            else
            {
                Offsets = 1;
            }

        }

    }
}