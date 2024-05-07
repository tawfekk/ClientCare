using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace ClientCare.Pages
{
    public partial class Medlemmer : ComponentBase
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
        public CRMService CRMService { get; set; }

        protected IEnumerable<ClientCare.Models.CRM.Medlem> medlems;

        protected RadzenDataGrid<ClientCare.Models.CRM.Medlem> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            medlems = await CRMService.GetMedlemmer(new Query { Filter = $@"i => i.Email.Contains(@0) || i.Name.Contains(@0) || i.Direktør.Contains(@0) || i.AntalAnsatte.Contains(@0) || i.CVR.Contains(@0)", FilterParameters = new object[] { search }, Expand = "RelationsAnsvarlig, Branche" });
        }
        protected override async Task OnInitializedAsync()
        {
            medlems = await CRMService.GetMedlemmer(new Query { Filter = $@"i => i.Email.Contains(@0) || i.Name.Contains(@0) || i.Direktør.Contains(@0) || i.AntalAnsatte.Contains(@0) || i.CVR.Contains(@0)", FilterParameters = new object[] { search }, Expand = "RelationsAnsvarlig, Branche" });
            //medlems = await CRMService.GetMedlemmer();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddMedlem>("Tilføj Medlem", null);
            await grid0.Reload();
        }

        protected async Task EditRow(ClientCare.Models.CRM.Medlem args)
        {
            await DialogService.OpenAsync<EditMedlem>("Rediger Medlem", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, ClientCare.Models.CRM.Medlem medlem)
        {
            try
            {
                if (await DialogService.Confirm("Er du sikker på, at du vil slette denne post?") == true)
                {
                    var deleteResult = await CRMService.DeleteMedlem(medlem.Id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                { 
                    Severity = NotificationSeverity.Error,
                    Summary = $"Fejl", 
                    Detail = $"Kunne ikke slette Medlem" 
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await CRMService.ExportMedlemmerToCSV(new Query
{ 
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}", 
    OrderBy = $"{grid0.Query.OrderBy}", 
    Expand = "", 
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "Medlemmer");
            }

            if (args == null || args.Value == "xlsx")
            {
                await CRMService.ExportMedlemmerToExcel(new Query
{ 
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}", 
    OrderBy = $"{grid0.Query.OrderBy}", 
    Expand = "", 
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "Medlemmer");
            }
        }
    }
}
