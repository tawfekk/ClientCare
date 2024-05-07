using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;
using ClientCare.Data;
using ClientCare.Models.CRM;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace ClientCare
{
    public partial class CRMService
    {
        private readonly SecurityService security;

        public CRMService(CRMContext context, NavigationManager navigationManager, SecurityService security)
          : this(context, navigationManager)
        {
            this.security = security;
        }


    }
}
