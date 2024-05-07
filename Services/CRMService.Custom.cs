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

        partial void OnNetværkCreated(Netværk item)
        {
             var userId = security.User.Id;

            // Set the UserId property of the netværk to the current user's id
             item.UserId = userId;
        }

        partial void OnNetværkRead(ref IQueryable<Netværk> items)
        {
            if (!security.IsInRole("Drift"))
            {
                var userId = security.User.Id;

                // Filter the netværk by the current user's id
                items = items.Where(item => item.UserId == userId);
            }
            // Include the user
            items = items.Include(item => item.User);
        }

    }
}
