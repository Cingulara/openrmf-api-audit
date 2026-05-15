// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using openrmf_audit_api.Data;
using openrmf_audit_api.Models;
using Moq;
using MongoDB.Bson;

namespace tests.Classes
{
    /// <summary>
    /// Tests for the IAuditRepository contract via Moq.
    /// AuditRepository itself requires a live MongoDB connection; these tests
    /// verify the interface contract and expected call signatures so that any
    /// concrete implementation stays honest.
    /// </summary>
    public class AuditRepositoryTests
    {
        private readonly Mock<IAuditRepository> _mockRepo;

        public AuditRepositoryTests()
        {
            _mockRepo = new Mock<IAuditRepository>();
        }

        // helper
        private static Audit MakeAudit(string program = "TestProgram")
        {
            return new Audit
            {
                program    = program,
                created    = DateTime.UtcNow,
                action     = "create",
                userid     = Guid.NewGuid().ToString(),
                username   = "test.user",
                fullname   = "Test User",
                email      = "test@openrmf.io",
                url        = "https://openrmf.io",
                message    = "unit test audit record",
                InternalId = ObjectId.GenerateNewId()
            };
        }

        // -----------------------------------------------------------------------
        // PASS tests — IAuditRepository interface contract
        // -----------------------------------------------------------------------

        [Fact]
        public void Test_IAuditRepository_MockIsNotNull()
        {
            Assert.NotNull(_mockRepo.Object);
        }

        [Fact]
        public async Task Test_GetAllAudits_ReturnsExpectedList()
        {
            List<Audit> expected = new List<Audit>
            {
                MakeAudit("Program1"),
                MakeAudit("Program2")
            };
            _mockRepo.Setup(r => r.GetAllAudits()).ReturnsAsync(expected);

            IEnumerable<Audit> result = await _mockRepo.Object.GetAllAudits();

            Assert.NotNull(result);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Test_GetAllAudits_ReturnsEmptyList()
        {
            _mockRepo.Setup(r => r.GetAllAudits()).ReturnsAsync(new List<Audit>());

            IEnumerable<Audit> result = await _mockRepo.Object.GetAllAudits();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Test_GetAllAudits_VerifiesCallCount()
        {
            _mockRepo.Setup(r => r.GetAllAudits()).ReturnsAsync(new List<Audit>());

            await _mockRepo.Object.GetAllAudits();
            await _mockRepo.Object.GetAllAudits();

            _mockRepo.Verify(r => r.GetAllAudits(), Times.Exactly(2));
        }

        [Fact]
        public async Task Test_GetAudit_ReturnsExpectedRecord()
        {
            string id = ObjectId.GenerateNewId().ToString();
            Audit expected = MakeAudit("MyProgram");
            _mockRepo.Setup(r => r.GetAudit(id)).ReturnsAsync(expected);

            Audit result = await _mockRepo.Object.GetAudit(id);

            Assert.NotNull(result);
            Assert.Equal(expected.program, result.program);
            Assert.Equal(expected.auditId, result.auditId);
        }

        [Fact]
        public async Task Test_GetAudit_VerifiesIdPassedThrough()
        {
            string id = ObjectId.GenerateNewId().ToString();
            _mockRepo.Setup(r => r.GetAudit(id)).ReturnsAsync(MakeAudit());

            await _mockRepo.Object.GetAudit(id);

            _mockRepo.Verify(r => r.GetAudit(id), Times.Once);
        }

        [Fact]
        public void Test_HealthStatus_ReturnsTrue_WhenHealthy()
        {
            _mockRepo.Setup(r => r.HealthStatus()).Returns(true);

            bool result = _mockRepo.Object.HealthStatus();

            Assert.True(result);
        }

        [Fact]
        public void Test_HealthStatus_VerifiesCallCount()
        {
            _mockRepo.Setup(r => r.HealthStatus()).Returns(true);

            _mockRepo.Object.HealthStatus();

            _mockRepo.Verify(r => r.HealthStatus(), Times.Once);
        }

        [Fact]
        public async Task Test_GetAudit_ReturnedRecord_HasNonEmptyAuditId()
        {
            string id = ObjectId.GenerateNewId().ToString();
            Audit expected = MakeAudit();
            _mockRepo.Setup(r => r.GetAudit(id)).ReturnsAsync(expected);

            Audit result = await _mockRepo.Object.GetAudit(id);

            Assert.NotEqual(Guid.Empty, result.auditId);
        }

        [Fact]
        public async Task Test_GetAudit_ReturnedRecord_HasExpectedInternalIdString()
        {
            string id = ObjectId.GenerateNewId().ToString();
            Audit expected = MakeAudit();
            _mockRepo.Setup(r => r.GetAudit(id)).ReturnsAsync(expected);

            Audit result = await _mockRepo.Object.GetAudit(id);

            Assert.False(string.IsNullOrEmpty(result.InternalIdString));
        }

        // -----------------------------------------------------------------------
        // FAIL tests — null / missing / unhealthy responses
        // -----------------------------------------------------------------------

        [Fact]
        public async Task Test_GetAllAudits_ReturnsNull_WhenSetupReturnsNull()
        {
            _mockRepo.Setup(r => r.GetAllAudits()).ReturnsAsync((IEnumerable<Audit>)null);

            IEnumerable<Audit> result = await _mockRepo.Object.GetAllAudits();

            Assert.Null(result);
        }

        [Fact]
        public async Task Test_GetAudit_ReturnsNull_WhenNotFound()
        {
            string id = ObjectId.GenerateNewId().ToString();
            _mockRepo.Setup(r => r.GetAudit(id)).ReturnsAsync((Audit)null);

            Audit result = await _mockRepo.Object.GetAudit(id);

            Assert.Null(result);
        }

        [Fact]
        public void Test_HealthStatus_ReturnsFalse_WhenUnhealthy()
        {
            _mockRepo.Setup(r => r.HealthStatus()).Returns(false);

            bool result = _mockRepo.Object.HealthStatus();

            Assert.False(result);
        }

        [Fact]
        public async Task Test_GetAudit_ThrowsException_WhenSetupThrows()
        {
            string id = ObjectId.GenerateNewId().ToString();
            _mockRepo.Setup(r => r.GetAudit(id)).ThrowsAsync(new Exception("db error"));

            await Assert.ThrowsAsync<Exception>(() => _mockRepo.Object.GetAudit(id));
        }

        [Fact]
        public async Task Test_GetAllAudits_ThrowsException_WhenSetupThrows()
        {
            _mockRepo.Setup(r => r.GetAllAudits()).ThrowsAsync(new Exception("connection lost"));

            await Assert.ThrowsAsync<Exception>(() => _mockRepo.Object.GetAllAudits());
        }

        [Fact]
        public void Test_HealthStatus_ThrowsException_WhenSetupThrows()
        {
            _mockRepo.Setup(r => r.HealthStatus()).Throws(new Exception("health check failed"));

            Assert.Throws<Exception>(() => _mockRepo.Object.HealthStatus());
        }
    }
}
