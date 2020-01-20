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
            try
            {
                return await _context.Audits
                        .Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        // query after Id or InternalId (BSonId value)
        public async Task<Audit> GetAudit(string id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);
                return await _context.Audits.Find(Audit => Audit.InternalId == GetInternalId(id)).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        private ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }
    }
}