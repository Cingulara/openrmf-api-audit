// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;
using Xunit;
using openrmf_audit_api.Controllers;
using openrmf_audit_api.Data;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly Mock<ILogger<HealthController>> _mockLogger;
        private readonly Mock<IAuditRepository> _mockAuditRepo;
        private readonly HealthController _healthController;

        public HealthControllerTests()
        {
            _mockLogger    = new Mock<ILogger<HealthController>>();
            _mockAuditRepo = new Mock<IAuditRepository>();
            _healthController = new HealthController(_mockAuditRepo.Object, _mockLogger.Object);
        }

        // -----------------------------------------------------------------------
        // PASS tests — controller constructs and returns healthy status
        // -----------------------------------------------------------------------

        [Fact]
        public void Test_HealthController_IsNotNull()
        {
            Assert.NotNull(_healthController);
        }

        [Fact]
        public void Test_HealthController_Get_ReturnsOk_WhenHealthy()
        {
            _mockAuditRepo.Setup(r => r.HealthStatus()).Returns(true);

            ActionResult<string> result = _healthController.Get();

            OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Equal("ok", ok.Value);
        }

        [Fact]
        public void Test_HealthController_Get_VerifiesHealthStatusCalledOnce_WhenHealthy()
        {
            _mockAuditRepo.Setup(r => r.HealthStatus()).Returns(true);

            _healthController.Get();

            _mockAuditRepo.Verify(r => r.HealthStatus(), Times.Once);
        }

        [Fact]
        public void Test_HealthController_Get_ResultIsActionResultOfString()
        {
            _mockAuditRepo.Setup(r => r.HealthStatus()).Returns(true);

            ActionResult<string> result = _healthController.Get();

            Assert.NotNull(result);
        }

        // -----------------------------------------------------------------------
        // FAIL tests — controller returns BadRequest on unhealthy or exception
        // -----------------------------------------------------------------------

        [Fact]
        public void Test_HealthController_Get_ReturnsBadRequest_WhenUnhealthy()
        {
            _mockAuditRepo.Setup(r => r.HealthStatus()).Returns(false);

            ActionResult<string> result = _healthController.Get();

            BadRequestObjectResult bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, bad.StatusCode);
            Assert.Equal("database error", bad.Value);
        }

        [Fact]
        public void Test_HealthController_Get_ReturnsBadRequest_OnException()
        {
            _mockAuditRepo.Setup(r => r.HealthStatus()).Throws(new Exception("connection refused"));

            ActionResult<string> result = _healthController.Get();

            BadRequestObjectResult bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, bad.StatusCode);
            Assert.Equal("Improper API configuration", bad.Value);
        }

        [Fact]
        public void Test_HealthController_Get_VerifiesHealthStatusCalledOnce_WhenUnhealthy()
        {
            _mockAuditRepo.Setup(r => r.HealthStatus()).Returns(false);

            _healthController.Get();

            _mockAuditRepo.Verify(r => r.HealthStatus(), Times.Once);
        }
    }
}

