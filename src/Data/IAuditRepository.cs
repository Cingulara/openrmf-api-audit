using openrmf_audit_api.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace openrmf_audit_api.Data {
    public interface IAuditRepository
    {
        Task<IEnumerable<Audit>> GetAllAudits();
        Task<Audit> GetAudit(string id);
        bool HealthStatus();
    }
}