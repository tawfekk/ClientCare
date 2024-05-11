using System.Net.Http;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Microsoft.EntityFrameworkCore;
using ClientCare.Models.Classes;

namespace ClientCare.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        public Data.CRMContext Context {get; set;}

        [Inject]
        public CRMService CRMService { get; set;}

        Stats monthlyStats;
        public IEnumerable<RevenueByMonth> revenueByMonth { get; set; }
        public IEnumerable<RevenueByMedlem> revenueByMedlem { get; set; }

        protected System.Linq.IQueryable<ClientCare.Models.CRM.Medlem> medlems;

        protected System.Linq.IQueryable<ClientCare.Models.CRM.Netværk> netværk;

     
        protected override async Task OnInitializedAsync()
        {
            monthlyStats = MonthlyStats();
            revenueByMonth = RevenueByMonth();
            revenueByMedlem = RevenueByMedlem();
        }


        public Stats MonthlyStats()
        {
            return Context.Medlemmer
                        .ToList()
                        .GroupBy(medlemmer => new DateTime(medlemmer.Indmeldelsesdato.Year, medlemmer.Indmeldelsesdato.Month, 1))
                        .Select(group => new Stats()
                        {
                            Month = group.Key,
                            Revenue = group.Sum(medlemmer => medlemmer.Kontigent),
                            Medlemmer = group.Count(),
                        })
                        .LastOrDefault();
        }


        public IEnumerable<RevenueByMedlem> RevenueByMedlem()
        {
            return Context.Medlemmer
                .ToList()
                .GroupBy(medlemmer => $"{medlemmer.Name}")
                .Select(group => new RevenueByMedlem()
                {
                    Medlem = group.Key,
                    Revenue = group.Sum(medlemmer => medlemmer.Kontigent)
                })
                .OrderByDescending(revenueByMedlem => revenueByMedlem.Revenue)
                .Take(15);
        }

        public IEnumerable<RevenueByMonth> RevenueByMonth()
        {
            return Context.Medlemmer
                .ToList()
                .GroupBy(medlemmer => new DateTime(medlemmer.Indmeldelsesdato.Year, medlemmer.Indmeldelsesdato.Month, 1))
                .Select(group => new RevenueByMonth()
                {
                    Revenue = group.Sum(medlemmer => medlemmer.Kontigent) / 1000, // Divide revenue by 1000
                    Month = group.Key
                })
                .OrderBy(deals => deals.Month);
        }       
        
    }
    

}
