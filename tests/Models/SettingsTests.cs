using Xunit;
using openrmf_audit_api.Models;
using System;

namespace tests.Models
{
    public class SettingsTests
    {
        [Fact]
        public void Test_NewSettingsIsValid()
        {
            Settings audset = new Settings();
            Assert.True(audset != null);
        }
    
        [Fact]
        public void Test_SettingsWithDataIsValid()
        {
            Settings audset = new Settings();
            audset.ConnectionString = "myConnection";
            audset.Database = "user=x; database=x; password=x;";

            // test things out
            Assert.True(audset != null);
            Assert.True (!string.IsNullOrEmpty(audset.ConnectionString));
            Assert.True (!string.IsNullOrEmpty(audset.Database));
        }
    }
}
