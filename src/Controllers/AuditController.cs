// Copyright (c) Cingulara LLC 2020 and Tutela LLC 2020. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using openrmf_audit_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using openrmf_audit_api.Data;

namespace openrmf_audit_api.Controllers
{
    [Route("/")]
    public class AuditController : Controller
    {
	    private readonly IAuditRepository _auditRepo;
       private readonly ILogger<AuditController> _logger;

        public AuditController(IAuditRepository auditRepo, ILogger<AuditController> logger)
        {
            _logger = logger;
            _auditRepo = auditRepo;
        }

        /// <summary>
        /// GET an Audit record by the Audit ID
        /// </summary>
        /// <param name="id">The audit ID</param>
        /// <returns>
        /// HTTP Status showing it was generated and the Audit record showing action, person, date, etc.
        /// </returns>
        /// <response code="200">Returns the Audit record generated for the id data passed in</response>
        /// <response code="400">If the item did not generate correctly</response>
        /// <response code="404">If the Audit ID was invalid</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAudit(string id)
        {
            try {
                _logger.LogInformation("Calling GetScore({0})", id);
                Audit auditRecord = new Audit();
                auditRecord = await _auditRepo.GetAudit(id);
                if (auditRecord == null) {                    
                    _logger.LogWarning("Calling GetAudit({0}) returned an invalid Audit record", id);
                    return NotFound();
                }
                _logger.LogInformation("Called GetAudit({0}) successfully", id);
                return Ok(auditRecord);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "GetAudit() Error Retrieving Audit for id {0}", id);
                return BadRequest();
            }
        }

        /// <summary>
        /// GET all Audit records
        /// </summary>
        /// <returns>
        /// HTTP Status showing it was generated and the list of audit records.
        /// </returns>
        /// <response code="200">Returns the list of audits</response>
        /// <response code="400">If the item did not generate correctly</response>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAllAudits()
        {
            try {
                _logger.LogInformation("Calling GetAllAudits()");
                IEnumerable<Audit> auditListing;
                auditListing = await _auditRepo.GetAllAudits();
                if (auditListing == null) {    
                    _logger.LogWarning("Calling GetAllAudits() returned an invalid list of Audit records");
                    return NotFound();
                }
                _logger.LogInformation("Called GetAllAudits() successfully");
                return Ok(auditListing);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "GetAllAudits() Error Retrieving list of Audits");
                return BadRequest();
            }
        }

    }
}
