// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using openrmf_audit_api.Controllers;
using openrmf_audit_api.Data;
using openrmf_audit_api.Models;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace tests.Controllers
{
    public class AuditControllerTests
    {
        private readonly Mock<ILogger<AuditController>> _mockLogger;
        private readonly Mock<IAuditRepository> _mockAuditRepo;
        private readonly AuditController _auditController;

        public AuditControllerTests()
        {
            _mockLogger    = new Mock<ILogger<AuditController>>();
            _mockAuditRepo = new Mock<IAuditRepository>();
            _auditController = new AuditController(_mockAuditRepo.Object, _mockLogger.Object);
        }

        // helper to build a populated Audit record
        private static Audit MakeAudit(string program = "TestProgram", string action = "create")
        {
            return new Audit
            {
                program  = program,
                created  = DateTime.UtcNow,
                action   = action,
                userid   = Guid.NewGuid().ToString(),
                username = "test.user",
                fullname = "Test User",
                email    = "test@openrmf.io",
                url      = "https://openrmf.io/api/audit",
                message  = "unit test record",
                InternalId = ObjectId.GenerateNewId()
            };
        }

        // -----------------------------------------------------------------------
        // PASS tests — GetAudit
        // -----------------------------------------------------------------------

        [Fact]
        public void Test_AuditController_IsNotNull()
        {
            Assert.NotNull(_auditController);
        }

        [Fact]
        public async Task Test_GetAudit_ReturnsOk_WhenRecordFound()
        {
            string id = ObjectId.GenerateNewId().ToString();
            Audit record = MakeAudit();
            _mockAuditRepo.Setup(r => r.GetAudit(id)).ReturnsAsync(record);

            IActionResult result = await _auditController.GetAudit(id);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            Audit returned = Assert.IsType<Audit>(ok.Value);
            Assert.Equal(record.program, returned.program);
            Assert.Equal(record.action, returned.action);
        }

        [Fact]
        public async Task Test_GetAudit_VerifiesRepoCalledOnce()
        {
            string id = ObjectId.GenerateNewId().ToString();
            _mockAuditRepo.Setup(r => r.GetAudit(id)).ReturnsAsync(MakeAudit());

            await _auditController.GetAudit(id);

            _mockAuditRepo.Verify(r => r.GetAudit(id), Times.Once);
        }

        [Fact]
        public async Task Test_GetAudit_ReturnedAudit_HasNonEmptyAuditId()
        {
            string id = ObjectId.GenerateNewId().ToString();
            Audit record = MakeAudit();
            _mockAuditRepo.Setup(r => r.GetAudit(id)).ReturnsAsync(record);

            IActionResult result = await _auditController.GetAudit(id);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
            Audit returned = Assert.IsType<Audit>(ok.Value);
            Assert.NotEqual(Guid.Empty, returned.auditId);
        }

        // -----------------------------------------------------------------------
        // FAIL tests — GetAudit
        // -----------------------------------------------------------------------

        [Fact]
        public async Task Test_GetAudit_ReturnsNotFound_WhenRecordIsNull()
        {
            string id = ObjectId.GenerateNewId().ToString();
            _mockAuditRepo.Setup(r => r.GetAudit(id)).ReturnsAsync((Audit)null);

            IActionResult result = await _auditController.GetAudit(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test_GetAudit_ReturnsBadRequest_OnException()
        {
            string id = ObjectId.GenerateNewId().ToString();
            _mockAuditRepo.Setup(r => r.GetAudit(id)).ThrowsAsync(new Exception("db error"));

            IActionResult result = await _auditController.GetAudit(id);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Test_GetAudit_ReturnsNotFound_ForEmptyId()
        {
            _mockAuditRepo.Setup(r => r.GetAudit(string.Empty)).ReturnsAsync((Audit)null);

            IActionResult result = await _auditController.GetAudit(string.Empty);

            Assert.IsType<NotFoundResult>(result);
        }

        // -----------------------------------------------------------------------
        // PASS tests — GetAllAudits
        // -----------------------------------------------------------------------

        [Fact]
        public async Task Test_GetAllAudits_ReturnsOk_WithList()
        {
            List<Audit> audits = new List<Audit>
            {
                MakeAudit("Program1"),
                MakeAudit("Program2"),
                MakeAudit("Program3")
            };
            _mockAuditRepo.Setup(r => r.GetAllAudits()).ReturnsAsync(audits);

            IActionResult result = await _auditController.GetAllAudits();

            OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            List<Audit> returned = Assert.IsType<List<Audit>>(ok.Value);
            Assert.Equal(3, returned.Count);
        }

        [Fact]
        public async Task Test_GetAllAudits_ResultIsOrderedByCreatedDescending()
        {
            DateTime oldest = DateTime.UtcNow.AddDays(-2);
            DateTime middle = DateTime.UtcNow.AddDays(-1);
            DateTime newest = DateTime.UtcNow;

            List<Audit> audits = new List<Audit>
            {
                new Audit { program = "Oldest", created = oldest, InternalId = ObjectId.GenerateNewId() },
                new Audit { program = "Newest", created = newest, InternalId = ObjectId.GenerateNewId() },
                new Audit { program = "Middle", created = middle, InternalId = ObjectId.GenerateNewId() }
            };
            _mockAuditRepo.Setup(r => r.GetAllAudits()).ReturnsAsync(audits);

            IActionResult result = await _auditController.GetAllAudits();

            OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
            List<Audit> returned = Assert.IsType<List<Audit>>(ok.Value);
            Assert.Equal("Newest", returned[0].program);
            Assert.Equal("Middle", returned[1].program);
            Assert.Equal("Oldest", returned[2].program);
        }

        [Fact]
        public async Task Test_GetAllAudits_ReturnsOk_WithEmptyList()
        {
            _mockAuditRepo.Setup(r => r.GetAllAudits()).ReturnsAsync(new List<Audit>());

            IActionResult result = await _auditController.GetAllAudits();

            OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
            List<Audit> returned = Assert.IsType<List<Audit>>(ok.Value);
            Assert.Empty(returned);
        }

        [Fact]
        public async Task Test_GetAllAudits_VerifiesRepoCalledOnce()
        {
            _mockAuditRepo.Setup(r => r.GetAllAudits()).ReturnsAsync(new List<Audit>());

            await _auditController.GetAllAudits();

            _mockAuditRepo.Verify(r => r.GetAllAudits(), Times.Once);
        }

        // -----------------------------------------------------------------------
        // FAIL tests — GetAllAudits
        // -----------------------------------------------------------------------

        [Fact]
        public async Task Test_GetAllAudits_ReturnsNotFound_WhenNull()
        {
            _mockAuditRepo.Setup(r => r.GetAllAudits()).ReturnsAsync((IEnumerable<Audit>)null);

            IActionResult result = await _auditController.GetAllAudits();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test_GetAllAudits_ReturnsBadRequest_OnException()
        {
            _mockAuditRepo.Setup(r => r.GetAllAudits()).ThrowsAsync(new Exception("connection lost"));

            IActionResult result = await _auditController.GetAllAudits();

            Assert.IsType<BadRequestResult>(result);
        }
    }
}
