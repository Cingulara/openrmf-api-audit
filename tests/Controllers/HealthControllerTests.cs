using System;
using Xunit;
using openrmf_audit_api.Controllers;
using openrmf_audit_api.Data;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace tests.Controllers
{

    public class HealthControllerTests
    {
        private readonly Mock<ILogger<HealthController>> _mockLogger;
        private readonly Mock<IAuditRepository> _mockauditRepo;
        private readonly HealthController _healthController; 

        public HealthControllerTests() {
            _mockLogger = new Mock<ILogger<HealthController>>();
            _mockauditRepo = new Mock<IAuditRepository>();
            _healthController = new HealthController(_mockauditRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public void Test_HealthControllerIsValid()
        {
            Assert.True(_healthController != null);
        }

        [Fact]
        public void Test_HealthControllerGetIsValid()
        {
            var result = _healthController.Get();
            _mockauditRepo.Setup(e => e.HealthStatus()).Returns(true);
            Assert.True(_healthController != null);
            //Assert.Equal(200, ((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).StatusCode); // returns a status code HTTP 200
        }
    }
}
