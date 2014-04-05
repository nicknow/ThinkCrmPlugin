using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using ThinkCrmPlugin.Core.Interfaces;
using ThinkCrmPlugin.Core.Logging;

namespace ThinkCrmPlugin.Core.Helpers
{
    public class CrmService : IOrganizationService, ICrmService
    {

        private const string LogPrefix = "CrmService:";

        private readonly IOrganizationService _service;
        private readonly ILogging _logging;
        private readonly bool _throwErrors;
        private readonly ICrmResourceStrings _crmResourceStrings;
        private readonly Dictionary<StringId, Tuple<string, string>> _dictionary;

        public CrmService(IOrganizationService service, ILogging logging, ICrmResourceStrings crmResourceStrings)
            : this(service, logging, true, crmResourceStrings)
        {
        }

        public CrmService(IOrganizationService service, ILogging logging, bool throwErrors, ICrmResourceStrings crmResourceStrings)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (logging == null) throw new ArgumentNullException("logging");
            if (crmResourceStrings == null) throw new ArgumentNullException("crmResourceStrings");
            _service = service;
            _logging = logging;
            _throwErrors = throwErrors;
            _dictionary = BuildDictionary();
            _crmResourceStrings = crmResourceStrings;
            _logging.Write("{0}: {1} {2}: {3}", _crmResourceStrings.GetString("LoggingClass"),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);
        }

        public Guid Create(Entity entity)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            try
            {
                var response = _service.Create(entity);
                return _logging.WriteAndReturn(response, GetString(StringId.LoggingCreatedEntity), entity.LogicalName, response.ToString());
            }
            catch (Exception ex)
            {
                _logging.Write(ex);

                if (_throwErrors) throw;
                
                return  _logging.WriteAndReturn(Guid.Empty, GetString(StringId.LoggingErrorNoThrow));
            }
        }

        #region Retrieve

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            try
            {                
                var response = _service.Retrieve(entityName, id, columnSet);
                return _logging.WriteAndReturn(response, GetString(StringId.LoggingRetrievedEntity),
                    response.LogicalName, response.Id);
            }
            catch (Exception ex)
            {
                _logging.Write(GetString(StringId.LoggingExceptionOnRetrieve), entityName, id,
                    ColumnSetToStringLog(columnSet));
                
                _logging.Write(ex);
                
                if (_throwErrors) throw;

                return _logging.WriteAndReturn<Entity>(null, GetString(StringId.LoggingErrorNoThrow));                
            }
        }

        private string ColumnSetToStringLog(ColumnSet columnSet)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            if (columnSet.AllColumns == true) return GetString(StringId.LoggingColumnsAllTrue);
            else if (!columnSet.Columns.Any()) return GetString(StringId.LoggingNoColumnsDefined);
            else return columnSet.Columns.Aggregate((working, next) => working + ',' + next);

        }

        public T Retrieve<T>(string entityName, Guid id, ColumnSet columnSet) where T : Entity
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            _logging.Write(GetString(StringId.LoggingRequestedType), typeof(T).Name);

            var result = Retrieve(entityName, id, columnSet).ToEntity<T>();

            return _logging.WriteAndReturn(result, GetString(StringId.LoggingRetrievedEntity), result.LogicalName,result.Id.ToString());
            
        }

        public Entity Retrieve(string entityName, Guid id)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            return Retrieve(entityName, id, new ColumnSet(true));
        }

        public T Retrieve<T>(string entityName, Guid id) where T : Entity
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);            

            return Retrieve<T>(entityName, id, new ColumnSet(true));
        }

        public Entity Retrieve(string entityName, Guid id, params string[] columns)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            if (columns != null && columns.Any())
            {
                _logging.Write(GetString(StringId.LoggingBuildingColumnSet));
                var columnSet = new ColumnSet(columns);
                return Retrieve(entityName, id, columnSet);
            }
            else
            {
                return Retrieve(entityName, id);
            }
        }

        public T Retrieve<T>(string entityName, Guid id, params string[] columns) where T : Entity
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            if (columns != null && columns.Any())
            {
                _logging.Write(GetString(StringId.LoggingBuildingColumnSet));
                var columnSet = new ColumnSet(columns);
                return Retrieve<T>(entityName, id, columnSet);
            }
            else
            {
                return Retrieve<T>(entityName, id);
            }
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);            

            try
            {
                var response = _service.RetrieveMultiple(query);
                return _logging.WriteAndReturn(response, GetString(StringId.LoggingRetrieveMultipleSuccess),
                    "RetrieveMultiple Response: IsNull={0} / Count={1} / EntityName={2}",
                    response == null ? true.ToString() : false.ToString(),
                    response != null ? response.Entities.Count.ToString() : " --",
                    response != null ? response.EntityName : " --");
            }
            catch (Exception ex)
            {
                _logging.Write(GetString(StringId.LoggingExeceptionRetrieveMultiple));
                
                if (query is FetchExpression)
                {
                    var q = query as FetchExpression;
                    _logging.Write(GetString(StringId.LoggingFetchXml));
                    _logging.Write(q.Query);

                }
                else if (query is QueryExpression)
                {
                    var q = query as QueryExpression;
                    _logging.Write(GetString(StringId.LoggingQueryExpressionEntity), q.EntityName);
                }

                _logging.Write(ex);
                if (_throwErrors) throw;

                return _logging.WriteAndReturn<EntityCollection>(null, GetString(StringId.LoggingErrorNoThrow));
                
            }
        }

        public EntityCollection<T> RetrieveMultiple<T>(QueryBase query) where T : Entity
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            _logging.Write(GetString(StringId.LoggingRequestedType), typeof(T).Name);

            var entCol = RetrieveMultiple(query);

            if (entCol == null || !entCol.Entities.Any()) return null;


            var response = new EntityCollection<T>()
            {
                EntityName = entCol.EntityName,
                ExtensionData = entCol.ExtensionData,
                MinActiveRowVersion = entCol.MinActiveRowVersion,
                MoreRecords = entCol.MoreRecords,
                PagingCookie = entCol.PagingCookie,
                TotalRecordCount = entCol.TotalRecordCount,
                TotalRecordCountLimitExceeded = entCol.TotalRecordCountLimitExceeded
            };

            entCol.Entities.ToList().ForEach(x => response.Entities.Add(x.ToEntity<T>()));

            return response;

        }

        #endregion

        public void Update(Entity entity)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);            

            try
            {
                _service.Update(entity);
                _logging.Write("Update Success on Type: {0} / Id: {1}", entity.LogicalName, entity.Id);
            }
            catch (Exception ex)
            {
                //TODO: Update Exception Message
                _logging.Write(ex);

                if (_throwErrors) throw;

                _logging.Write(GetString(StringId.LoggingErrorNoThrow));               
            }            

        }

        public void Delete(string entityName, Guid id)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            try
            {
                _service.Delete(entityName, id);
                _logging.Write("Delete Success on Type: {0} / Id: {1}", entityName, id);
            }
            catch (Exception ex)
            {
                _logging.Write("Exception on Delete of Type: {0} / Id: {1}", entityName, id);
                _logging.Write(ex);

                if (_throwErrors) throw;

                _logging.Write(GetString(StringId.LoggingErrorNoThrow));
            }

        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            try
            {
                var response = _service.Execute(request);
                return _logging.WriteAndReturn(response, "Execute Success Request: {0} / Response: {1}", request.RequestName, response.ResponseName);
            }
            catch (Exception ex)
            {
                _logging.Write("Exception on Execute");
                CrmDetailsLogger.LogOrganizationRequest(_logging, request);
                _logging.Write(ex);

                if (_throwErrors) throw;

                return _logging.WriteAndReturn<OrganizationResponse>(null, GetString(StringId.LoggingErrorNoThrow));
            }
        }

        public TResp Execute<TResp>(OrganizationRequest request) where TResp : OrganizationResponse
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            _logging.Write("Request Type: {0}  Response Type: {1}", request.RequestName, typeof(TResp).Name);

            return (TResp)Execute(request);
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            try
            {
                _service.Associate(entityName, entityId, relationship, relatedEntities);
            }
            catch (Exception ex)
            {

                _logging.Write("Exception on Associate");
                _logging.Write("Associate EntityName={0} / EntityId={1} / Relationship={2} / RelatedEntities.Count={3}",
                    entityName, entityId.ToString(), relationship.SchemaName,
                    relatedEntities != null ? relatedEntities.Count.ToString() : "(null)");
                if (relatedEntities != null) relatedEntities.ToList().ForEach(x => _logging.Write(CrmDetailsLogger.LogEntityReference(x)));
                _logging.Write(ex);

                if (_throwErrors) throw;

                _logging.Write(GetString(StringId.LoggingErrorNoThrow));
            }
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _logging.Write("{0}: {1} {2}: {3}", GetString(StringId.LoggingClass),
                MethodBase.GetCurrentMethod().DeclaringType,
                GetString(StringId.LoggingMethod),
                MethodBase.GetCurrentMethod().Name);

            try
            {
                _service.Disassociate(entityName, entityId, relationship, relatedEntities);
            }
            catch (Exception ex)
            {

                _logging.Write("Exception on Disassociate");
                _logging.Write("Disassociate EntityName={0} / EntityId={1} / Relationship={2} / RelatedEntities.Count={3}",
                    entityName, entityId.ToString(), relationship.SchemaName,
                    relatedEntities != null ? relatedEntities.Count.ToString() : "(null)");
                if (relatedEntities != null) relatedEntities.ToList().ForEach(x => _logging.Write(CrmDetailsLogger.LogEntityReference(x)));
                _logging.Write(ex);

                if (_throwErrors) throw;

                _logging.Write(GetString(StringId.LoggingErrorNoThrow));
            }
        }

        #region ResourceStringSupport
        private string GetString(StringId stringId)
        {
            return _dictionary.ContainsKey(stringId)
                ? (_crmResourceStrings != null
                    ? _crmResourceStrings.GetString(_dictionary[stringId].Item1, _dictionary[stringId].Item2)
                    : _dictionary[stringId].Item2)
                : string.Empty;
        }

        private Dictionary<StringId, Tuple<string,string>> BuildDictionary()
        {
            var dict = new Dictionary<StringId, Tuple<string, string>>
            {
                {StringId.LoggingClassMethod, new Tuple<string, string>("LoggingClassMethod", "Class: {0} Method: {1}")},
                {
                    StringId.LoggingErrorNoThrow,
                    new Tuple<string, string>("LoggingErrorNoThrow",
                        "Throw Errors is False - No exception will be thrown.")
                },
                {StringId.LoggingClass, new Tuple<string, string>("LoggingClass", "Class")},
                {StringId.LoggingMethod, new Tuple<string, string>("LoggingMethod", "Method")},
                {
                    StringId.LoggingExceptionOnRetrieve, new Tuple<string, string>("LoggingExceptionOnRetrieve",
                        "Exception on Retrieve: EntityName={0},Id={1},ColumnSet={2}")
                },
                {
                    StringId.LoggingColumnsAllTrue,
                    new Tuple<string, string>("LoggingColumnsAllTrue", "ColumnSet.AllColumns=True")
                },
                {
                    StringId.LoggingNoColumnsDefined,
                    new Tuple<string, string>("LoggingNoColumnsDefined", "(No Columns Defined)")
                },
                {
                    StringId.LoggingRetrievedEntity,
                    new Tuple<string, string>("LoggingRetrievedEntity", "Retrieved Type: {0} Id: {1}")
                },
                {
                    StringId.LoggingRetrievedEntityNull,
                    new Tuple<string, string>("LoggingRetrievedEntityNull", "(CrmService: No Record - Null Response)")
                },
                {
                    StringId.LoggingRequestedType, new Tuple<string, string>("LoggingRequestedType", "Requested Type: {0}")
                },
                {
                    StringId.LoggingBuildingColumnSet,
                    new Tuple<string, string>("LoggingBuildingColumnSet", "Building ColumnSet")
                },
                {StringId.LoggingFetchXml, new Tuple<string, string>("LoggingFetchXml", "FetchXML Query")},
                {
                    StringId.LoggingQueryExpressionEntity,
                    new Tuple<string, string>("LoggingQueryExpressionEntity", "QueryExpression Query: {0}")
                },
                {
                    StringId.LoggingExeceptionRetrieveMultiple,
                    new Tuple<string, string>("LoggingExeceptionRetrieveMultiple", "Exception on RetrieveMultiple")
                },
                {
                    StringId.ErrorRetrievingUsername,
                    new Tuple<string, string>("ErrorRetrievingUsername", "Error Retrieving User Name.")
                },
                {
                    StringId.LoggingCreatedEntity, new Tuple<string, string>("LoggingCreatedEntity", "Created Entity of Type: {0} / Id: {1}")
                },
                {
                    StringId.LoggingRetrieveMultipleSuccess, new Tuple<string, string>("LoggingRetrieveMultipleSuccess","RetrieveMultiple Response: IsNull={0} / Count={1} / EntityName={2}")
                }
            };
            return dict;
        }

        public enum StringId
        {
            LoggingClassMethod,
            LoggingErrorNoThrow,
            LoggingClass,
            LoggingMethod,
            LoggingExceptionOnRetrieve,
            LoggingColumnsAllTrue,
            LoggingNoColumnsDefined,
            LoggingRetrievedEntity,
            LoggingRetrievedEntityNull,
            LoggingRequestedType,
            LoggingBuildingColumnSet,
            LoggingFetchXml,
            LoggingQueryExpressionEntity,
            LoggingExeceptionRetrieveMultiple,
            ErrorRetrievingUsername,
            LoggingCreatedEntity,
            LoggingRetrieveMultipleSuccess
        } 
        #endregion

    }
}
