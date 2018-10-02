using CMS.MODEL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TEST
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnAreas_ClickAsync(object sender, EventArgs e)
        {
            var areas = new CMS.CORE.Entities.Areas
            {
                Entities = new cmsEntities()
            };

            var area = await areas.Post(new CMS.MODEL.Entities.Area
            {
                AreaId = Guid.NewGuid().ToString(),
                CodigoIdioma = "es-ES",
                //Nombre = "AREA DE PRUEBAS",
                //Tag = "area-de-pruebas-2",
                Descripcion = "Descripción",
                Grupos = "GROUP_DESARROLLO,GROUP_USERS",
                Roles = "All",
                EsMenu = true,
                EstaActivo = true,
                FechaCreacion = DateTime.Now
            });
        }
    }
}
