// Copyright (c) Cingulara LLC 2020 and Tutela LLC 2020. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using openrmf_audit_api.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;

namespace openrmf_audit_api.Data {
    public class AuditRepository : IAuditRepository
    {
        private readonly AuditContext _context = null;

        public AuditRepository(IOptions<Settings> settings)
        {
            _context = new AuditContext(settings);
        }

        public async Task<IEnumerable<Audit>> GetAllAudits()
        {
                return await _context.Audits
                        .Find(_ => true).ToListAsync();
        }

        // query after Id or InternalId (BSonId value)
        public async Task<Audit> GetAudit(string id)
        {
                ObjectId internalId = GetInternalId(id);
                return await _context.Audits.Find(Audit => Audit.InternalId == GetInternalId(id)).FirstOrDefaultAsync();
        }

        private ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }
        
        // check that the database is responding and it returns at least one collection name
        public bool HealthStatus(){
            var result = _context.Audits.Database.ListCollectionNamesAsync().GetAwaiter().GetResult().FirstOrDefault();
            if (!string.IsNullOrEmpty(result)) // we are good to go
                return true;
            return false;
        }
    }
}