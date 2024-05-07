using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using ClientCare.Data;

namespace ClientCare.Controllers
{
    public partial class ExportCRMController : ExportController
    {
        private readonly CRMContext context;
        private readonly CRMService service;

        public ExportCRMController(CRMContext context, CRMService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/CRM/medlemmer/csv")]
        [HttpGet("/export/CRM/medlemmer/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMedlemmerToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetMedlemmer(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/medlemmer/excel")]
        [HttpGet("/export/CRM/medlemmer/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMedlemmerToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetMedlemmer(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/netværk/csv")]
        [HttpGet("/export/CRM/netværk/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportNetværkToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetNetværk(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/netværk/excel")]
        [HttpGet("/export/CRM/netværk/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportNetværkToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetNetværk(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/brancher/csv")]
        [HttpGet("/export/CRM/brancher/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBrancherToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetBrancher(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/brancher/excel")]
        [HttpGet("/export/CRM/brancher/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBrancherToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetBrancher(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/relationsansvarligs/csv")]
        [HttpGet("/export/CRM/relationsansvarligs/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRelationsAnsvarligeToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRelationsAnsvarlige(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/relationsansvarligs/excel")]
        [HttpGet("/export/CRM/relationsansvarligs/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRelationsAnsvarligeToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRelationsAnsvarlige(), Request.Query), fileName);
        }
    }
}
