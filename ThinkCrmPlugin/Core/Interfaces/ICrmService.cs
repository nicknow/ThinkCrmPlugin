using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using ThinkCrmPlugin.Core.Helpers;

namespace ThinkCrmPlugin.Core.Interfaces
{
    public interface ICrmService
    {
        Guid Create(Entity entity);
        Entity Retrieve(string entityName, Guid id, ColumnSet columnSet);
        T Retrieve<T>(string entityName, Guid id, ColumnSet columnSet) where T : Entity;
        Entity Retrieve(string entityName, Guid id);
        T Retrieve<T>(string entityName, Guid id) where T : Entity;
        Entity Retrieve(string entityName, Guid id, params string[] columns);
        T Retrieve<T>(string entityName, Guid id, params string[] columns) where T : Entity;
        EntityCollection RetrieveMultiple(QueryBase query);
        EntityCollection<T> RetrieveMultiple<T>(QueryBase query) where T : Entity;
        void Update(Entity entity);
        void Delete(string entityName, Guid id);
        OrganizationResponse Execute(OrganizationRequest request);
        Resp Execute<Resp>(OrganizationRequest request) where Resp : OrganizationResponse;
        void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities);
        void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities);
    }
}
