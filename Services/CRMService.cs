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

namespace ClientCare
{
    public partial class CRMService
    {
        CRMContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly CRMContext context;
        private readonly NavigationManager navigationManager;

        public CRMService(CRMContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportMedlemmerToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/medlemmer/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/medlemmer/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportMedlemmerToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/medlemmer/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/medlemmer/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnMedlemmerRead(ref IQueryable<ClientCare.Models.CRM.Medlem> items);

        public async Task<IQueryable<ClientCare.Models.CRM.Medlem>> GetMedlemmer(Query query = null)
        {
            var items = Context.Medlemmer.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnMedlemmerRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnMedlemGet(ClientCare.Models.CRM.Medlem item);
        partial void OnGetMedlemById(ref IQueryable<ClientCare.Models.CRM.Medlem> items);


        public async Task<ClientCare.Models.CRM.Medlem> GetMedlemById(int id)
        {
            var items = Context.Medlemmer
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetMedlemById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnMedlemGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnMedlemCreated(ClientCare.Models.CRM.Medlem item);
        partial void OnAfterMedlemCreated(ClientCare.Models.CRM.Medlem item);

        public async Task<ClientCare.Models.CRM.Medlem> CreateMedlem(ClientCare.Models.CRM.Medlem medlem)
        {
            OnMedlemCreated(medlem);

            var existingItem = Context.Medlemmer
                              .Where(i => i.Id == medlem.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Medlemmer.Add(medlem);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(medlem).State = EntityState.Detached;
                throw;
            }

            OnAfterMedlemCreated(medlem);

            return medlem;
        }

        public async Task<ClientCare.Models.CRM.Medlem> CancelMedlemChanges(ClientCare.Models.CRM.Medlem item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnMedlemUpdated(ClientCare.Models.CRM.Medlem item);
        partial void OnAfterMedlemUpdated(ClientCare.Models.CRM.Medlem item);

        public async Task<ClientCare.Models.CRM.Medlem> UpdateMedlem(int id, ClientCare.Models.CRM.Medlem medlem)
        {
            OnMedlemUpdated(medlem);

            var itemToUpdate = Context.Medlemmer
                              .Where(i => i.Id == medlem.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(medlem);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterMedlemUpdated(medlem);

            return medlem;
        }

        partial void OnMedlemDeleted(ClientCare.Models.CRM.Medlem item);
        partial void OnAfterMedlemDeleted(ClientCare.Models.CRM.Medlem item);

        public async Task<ClientCare.Models.CRM.Medlem> DeleteMedlem(int id)
        {
            var itemToDelete = Context.Medlemmer
                              .Where(i => i.Id == id)
                              .Include(i => i.Netværk)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnMedlemDeleted(itemToDelete);


            Context.Medlemmer.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterMedlemDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportNetværkToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/netværk/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/netværk/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportNetværkToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/netværk/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/netværk/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnNetværkRead(ref IQueryable<ClientCare.Models.CRM.Netværk> items);

        public async Task<IQueryable<ClientCare.Models.CRM.Netværk>> GetNetværk(Query query = null)
        {
            var items = Context.Netværk.AsQueryable();

            items = items.Include(i => i.Medlem);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnNetværkRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnNetværkGet(ClientCare.Models.CRM.Netværk item);
        partial void OnGetNetværkById(ref IQueryable<ClientCare.Models.CRM.Netværk> items);


        public async Task<ClientCare.Models.CRM.Netværk> GetNetværkById(int id)
        {
            var items = Context.Netværk
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Medlem);
 
            OnGetNetværkById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnNetværkGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnNetværkCreated(ClientCare.Models.CRM.Netværk item);
        partial void OnAfterNetværkCreated(ClientCare.Models.CRM.Netværk item);

        public async Task<ClientCare.Models.CRM.Netværk> CreateNetværk(ClientCare.Models.CRM.Netværk netværk)
        {
            OnNetværkCreated(netværk);

            var existingItem = Context.Netværk
                              .Where(i => i.Id == netværk.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Netværk.Add(netværk);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(netværk).State = EntityState.Detached;
                throw;
            }

            OnAfterNetværkCreated(netværk);

            return netværk;
        }

        public async Task<ClientCare.Models.CRM.Netværk> CancelNetværkChanges(ClientCare.Models.CRM.Netværk item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnNetværkUpdated(ClientCare.Models.CRM.Netværk item);
        partial void OnAfterNetværkUpdated(ClientCare.Models.CRM.Netværk item);

        public async Task<ClientCare.Models.CRM.Netværk> UpdateNetværk(int id, ClientCare.Models.CRM.Netværk netværk)
        {
            OnNetværkUpdated(netværk);

            var itemToUpdate = Context.Netværk
                              .Where(i => i.Id == netværk.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(netværk);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterNetværkUpdated(netværk);

            return netværk;
        }

        partial void OnNetværkDeleted(ClientCare.Models.CRM.Netværk item);
        partial void OnAfterNetværkDeleted(ClientCare.Models.CRM.Netværk item);

        public async Task<ClientCare.Models.CRM.Netværk> DeleteNetværk(int id)
        {
            var itemToDelete = Context.Netværk
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnNetværkDeleted(itemToDelete);


            Context.Netværk.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterNetværkDeleted(itemToDelete);

            return itemToDelete;
        }

    
        public async Task ExportBrancherToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/brancher/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/brancher/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportBrancherToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/brancher/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/brancher/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnBrancherRead(ref IQueryable<ClientCare.Models.CRM.Branche> items);

        public async Task<IQueryable<ClientCare.Models.CRM.Branche>> GetBrancher(Query query = null)

        {
            var items = Context.Brancher.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnBrancherRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnBrancheGet(ClientCare.Models.CRM.Branche item);
        partial void OnGetBrancheById(ref IQueryable<ClientCare.Models.CRM.Branche> items);


        public async Task<ClientCare.Models.CRM.Branche> GetBrancheById(int id)
        {
            var items = Context.Brancher
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetBrancheById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnBrancheGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnBrancheCreated(ClientCare.Models.CRM.Branche item);
        partial void OnAfterBrancheCreated(ClientCare.Models.CRM.Branche item);

        public async Task<ClientCare.Models.CRM.Branche> CreateBranche(ClientCare.Models.CRM.Branche branche)
        {
            OnBrancheCreated(branche);

            var existingItem = Context.Brancher
                              .Where(i => i.Id == branche.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Brancher.Add(branche);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(branche).State = EntityState.Detached;
                throw;
            }

            OnAfterBrancheCreated(branche);

            return branche;
        }

        public async Task<ClientCare.Models.CRM.Branche> CancelBrancheChanges(ClientCare.Models.CRM.Branche item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnBrancheUpdated(ClientCare.Models.CRM.Branche item);
        partial void OnAfterBrancheUpdated(ClientCare.Models.CRM.Branche item);

        public async Task<ClientCare.Models.CRM.Branche> UpdateBranche(int id, ClientCare.Models.CRM.Branche branche)
        {
            OnBrancheUpdated(branche);

            var itemToUpdate = Context.Brancher
                              .Where(i => i.Id == branche.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(branche);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterBrancheUpdated(branche);

            return branche;
        }

        partial void OnBrancheDeleted(ClientCare.Models.CRM.Branche item);
        partial void OnAfterBrancheDeleted(ClientCare.Models.CRM.Branche item);

        public async Task<ClientCare.Models.CRM.Branche> DeleteBranche(int id)
        {
            var itemToDelete = Context.Brancher
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnBrancheDeleted(itemToDelete);


            Context.Brancher.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterBrancheDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportRelationsAnsvarligeToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/relationsansvarligs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/relationsansvarligs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRelationsAnsvarligeToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/relationsansvarligs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/relationsansvarligs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRelationsAnsvarligeRead(ref IQueryable<ClientCare.Models.CRM.RelationsAnsvarlig> items);

        public async Task<IQueryable<ClientCare.Models.CRM.RelationsAnsvarlig>> GetRelationsAnsvarlige(Query query = null)

        {
            var items = Context.RelationsAnsvarlige.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnRelationsAnsvarligeRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRelationsAnsvarligGet(ClientCare.Models.CRM.RelationsAnsvarlig item);
        partial void OnGetRelationsAnsvarligById(ref IQueryable<ClientCare.Models.CRM.RelationsAnsvarlig> items);


        public async Task<ClientCare.Models.CRM.RelationsAnsvarlig> GetRelationsAnsvarligById(int id)
        {
            var items = Context.RelationsAnsvarlige
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetRelationsAnsvarligById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRelationsAnsvarligGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRelationsAnsvarligCreated(ClientCare.Models.CRM.RelationsAnsvarlig item);
        partial void OnAfterRelationsAnsvarligCreated(ClientCare.Models.CRM.RelationsAnsvarlig item);

        public async Task<ClientCare.Models.CRM.RelationsAnsvarlig> CreateRelationsAnsvarlig(ClientCare.Models.CRM.RelationsAnsvarlig relationsansvarlig)
        {
            OnRelationsAnsvarligCreated(relationsansvarlig);

            var existingItem = Context.RelationsAnsvarlige
                              .Where(i => i.Id == relationsansvarlig.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.RelationsAnsvarlige.Add(relationsansvarlig);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(relationsansvarlig).State = EntityState.Detached;
                throw;
            }

            OnAfterRelationsAnsvarligCreated(relationsansvarlig);

            return relationsansvarlig;
        }

        public async Task<ClientCare.Models.CRM.RelationsAnsvarlig> CancelRelationsAnsvarligChanges(ClientCare.Models.CRM.RelationsAnsvarlig item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRelationsAnsvarligUpdated(ClientCare.Models.CRM.RelationsAnsvarlig item);
        partial void OnAfterRelationsAnsvarligUpdated(ClientCare.Models.CRM.RelationsAnsvarlig item);

        public async Task<ClientCare.Models.CRM.RelationsAnsvarlig> UpdateRelationsAnsvarlig(int id, ClientCare.Models.CRM.RelationsAnsvarlig relationsansvarlig)
        {
            OnRelationsAnsvarligUpdated(relationsansvarlig);

            var itemToUpdate = Context.RelationsAnsvarlige
                              .Where(i => i.Id == relationsansvarlig.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(relationsansvarlig);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRelationsAnsvarligUpdated(relationsansvarlig);

            return relationsansvarlig;
        }

        partial void OnRelationsAnsvarligDeleted(ClientCare.Models.CRM.RelationsAnsvarlig item);
        partial void OnAfterRelationsAnsvarligDeleted(ClientCare.Models.CRM.RelationsAnsvarlig item);

        public async Task<ClientCare.Models.CRM.RelationsAnsvarlig> DeleteRelationsAnsvarlig(int id)
        {
            var itemToDelete = Context.RelationsAnsvarlige
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnRelationsAnsvarligDeleted(itemToDelete);


            Context.RelationsAnsvarlige.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRelationsAnsvarligDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}
