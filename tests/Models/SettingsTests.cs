// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using Xunit;
using openrmf_audit_api.Models;

namespace tests.Models
{
    public class SettingsTests
    {
        // -----------------------------------------------------------------------
        // PASS tests — valid construction and property assignment
        // -----------------------------------------------------------------------

        [Fact]
        public void Test_NewSettings_IsNotNull()
        {
            Settings settings = new Settings();
            Assert.NotNull(settings);
        }

        [Fact]
        public void Test_Settings_ConnectionStringIsAssigned()
        {
            Settings settings = new Settings
            {
                ConnectionString = "mongodb://localhost:27017"
            };
            Assert.Equal("mongodb://localhost:27017", settings.ConnectionString);
        }

        [Fact]
        public void Test_Settings_DatabaseIsAssigned()
        {
            Settings settings = new Settings
            {
                Database = "openrmf"
            };
            Assert.Equal("openrmf", settings.Database);
        }

        [Fact]
        public void Test_Settings_BothFieldsAssigned()
        {
            Settings settings = new Settings
            {
                ConnectionString = "mongodb://localhost:27017",
                Database = "openrmf"
            };

            Assert.NotNull(settings);
            Assert.False(string.IsNullOrEmpty(settings.ConnectionString));
            Assert.False(string.IsNullOrEmpty(settings.Database));
        }

        [Fact]
        public void Test_Settings_ConnectionString_MatchesExpectedValue()
        {
            const string expected = "mongodb://user:pass@host:27017/admin";
            Settings settings = new Settings { ConnectionString = expected };
            Assert.Equal(expected, settings.ConnectionString);
        }

        [Fact]
        public void Test_Settings_Database_MatchesExpectedValue()
        {
            const string expected = "auditdb";
            Settings settings = new Settings { Database = expected };
            Assert.Equal(expected, settings.Database);
        }

        // -----------------------------------------------------------------------
        // FAIL tests — missing / empty / null data scenarios
        // -----------------------------------------------------------------------

        [Fact]
        public void Test_Settings_ConnectionString_IsNullByDefault()
        {
            Settings settings = new Settings();
            Assert.Null(settings.ConnectionString);
        }

        [Fact]
        public void Test_Settings_Database_IsNullByDefault()
        {
            Settings settings = new Settings();
            Assert.Null(settings.Database);
        }

        [Fact]
        public void Test_Settings_EmptyConnectionString_IsNullOrEmpty()
        {
            Settings settings = new Settings { ConnectionString = string.Empty };
            Assert.True(string.IsNullOrEmpty(settings.ConnectionString));
        }

        [Fact]
        public void Test_Settings_EmptyDatabase_IsNullOrEmpty()
        {
            Settings settings = new Settings { Database = string.Empty };
            Assert.True(string.IsNullOrEmpty(settings.Database));
        }
    }
}
