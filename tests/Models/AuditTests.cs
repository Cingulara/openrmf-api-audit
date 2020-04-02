using Xunit;
using openrmf_audit_api.Models;
using System;

namespace tests.Models
{
    public class AuditTests
    {
        [Fact]
        public void Test_NewAuditIsValid()
        {
            Audit aud = new Audit();
            Assert.True(aud != null);
            Assert.True(aud.auditId != Guid.Empty);
        }
    
        [Fact]
        public void Test_AuditWithDataIsValid()
        {
            Audit aud = new Audit();
            aud.program = "Save";
            aud.created = DateTime.Now;
            aud.action = "edit";
            aud.userid = Guid.NewGuid().ToString();
            aud.username = "my.username";
            aud.fullname = "My F. Name";
            aud.email = "dale.bingham@cingulara.com";
            aud.url = "https://www.openrmf.io";
            aud.message = "This is a test";

            Assert.True(aud != null);
            Assert.True(aud.auditId != Guid.Empty);
            Assert.True(!string.IsNullOrEmpty(aud.program));
            Assert.True(!string.IsNullOrEmpty(aud.action));
            Assert.True(!string.IsNullOrEmpty(aud.userid));
            Assert.True(!string.IsNullOrEmpty(aud.username));
            Assert.True(!string.IsNullOrEmpty(aud.fullname));
            Assert.True(!string.IsNullOrEmpty(aud.email));
            Assert.True(!string.IsNullOrEmpty(aud.url));
            Assert.True(!string.IsNullOrEmpty(aud.message));
        }
    }
}
