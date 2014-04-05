using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ThinkCrmPlugin.Core.Helpers
{
    public class EntityCollection<T> : IExtensibleDataObject
    {
        private string _entityName;
        private IList<T> _entities;
        private bool _moreRecords;
        private string _pagingCookie;
        private string _minActiveRowVersion;
        private int _totalRecordCount;
        private bool _totalRecordCountLimitExceeded;
        private ExtensionDataObject _extensionDataObject;

        [DataMember]
        public IList<T> Entities
        {
            get
            {
                if (this._entities == null)
                    return this._entities = new List<T>();
                return this._entities;
            }
            private set
            {
                this._entities = value;
            }
        }

        [DataMember]
        public bool MoreRecords
        {
            get
            {
                return this._moreRecords;
            }
            set
            {
                this._moreRecords = value;
            }
        }

        [DataMember]
        public string PagingCookie
        {
            get
            {
                return this._pagingCookie;
            }
            set
            {
                this._pagingCookie = value;
            }
        }

        [DataMember]
        public string MinActiveRowVersion
        {
            get
            {
                return this._minActiveRowVersion;
            }
            set
            {
                this._minActiveRowVersion = value;
            }
        }

        [DataMember]
        public int TotalRecordCount
        {
            get
            {
                return this._totalRecordCount;
            }
            set
            {
                this._totalRecordCount = value;
            }
        }

        [DataMember]
        public bool TotalRecordCountLimitExceeded
        {
            get
            {
                return this._totalRecordCountLimitExceeded;
            }
            set
            {
                this._totalRecordCountLimitExceeded = value;
            }
        }

        public T this[int index]
        {
            get
            {
                return this.Entities[index];
            }
            set
            {
                this.Entities[index] = value;
            }
        }

        [DataMember]
        public string EntityName
        {
            get
            {
                return this._entityName;
            }
            set
            {
                this._entityName = value;
            }
        }

        public ExtensionDataObject ExtensionData
        {
            get
            {
                return this._extensionDataObject;
            }
            set
            {
                this._extensionDataObject = value;
            }
        }

        public EntityCollection()
        {
        }

        public EntityCollection(IEnumerable<T> list)
        {
            this._entities = new List<T>(list);
        }

    }
}
