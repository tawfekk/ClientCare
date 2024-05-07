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
    public partial class EditMedlem : ComponentBase
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

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            relationsAnsvarligeForRelationsanvarligId = await CRMService.GetRelationsAnsvarlige();
            brancheForBrancheId = await CRMService.GetBrancher();
            medlem = await CRMService.GetMedlemById(Id);
        }
        protected bool errorVisible;
        protected ClientCare.Models.CRM.Medlem medlem;

        protected IEnumerable<ClientCare.Models.CRM.RelationsAnsvarlig> relationsAnsvarligeForRelationsanvarligId;

        protected IEnumerable<ClientCare.Models.CRM.Branche> brancheForBrancheId;

        protected async Task FormSubmit()
        {
            try
            {
                await CRMService.UpdateMedlem(Id, medlem);
                DialogService.Close(medlem);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }


        protected bool hasChanges = false;
        protected bool canEdit = true;

        [Inject]
        protected SecurityService Security { get; set; }


        protected async Task ReloadButtonClick(MouseEventArgs args)
        {
           CRMService.Reset();
            hasChanges = false;
            canEdit = true;

            medlem = await CRMService.GetMedlemById(Id);
        }
    }
}
