// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using Xunit;
using openrmf_audit_api.Models;
using System;
using MongoDB.Bson;

namespace tests.Models
{
    public class AuditTests
    {
        // -----------------------------------------------------------------------
        // PASS tests — valid construction and property assignment
        // -----------------------------------------------------------------------

        [Fact]
        public void Test_NewAudit_IsNotNull()
        {
            Audit aud = new Audit();
            Assert.NotNull(aud);
        }

        [Fact]
        public void Test_NewAudit_HasNonEmptyAuditId()
        {
            Audit aud = new Audit();
            Assert.NotEqual(Guid.Empty, aud.auditId);
        }

        [Fact]
        public void Test_NewAudit_EachInstanceHasUniqueAuditId()
        {
            Audit aud1 = new Audit();
            Audit aud2 = new Audit();
            Assert.NotEqual(aud1.auditId, aud2.auditId);
        }

        [Fact]
        public void Test_AuditWithData_AllPropertiesSet()
        {
            Audit aud = new Audit
            {
                program   = "Save",
                created   = DateTime.UtcNow,
                action    = "edit",
                userid    = Guid.NewGuid().ToString(),
                username  = "my.username",
                fullname  = "My F. Name",
                email     = "test@openrmf.io",
                url       = "https://www.openrmf.io",
                message   = "This is a test message"
            };

            Assert.NotNull(aud);
            Assert.NotEqual(Guid.Empty, aud.auditId);
            Assert.False(string.IsNullOrEmpty(aud.program));
            Assert.False(string.IsNullOrEmpty(aud.action));
            Assert.False(string.IsNullOrEmpty(aud.userid));
            Assert.False(string.IsNullOrEmpty(aud.username));
            Assert.False(string.IsNullOrEmpty(aud.fullname));
            Assert.False(string.IsNullOrEmpty(aud.email));
            Assert.False(string.IsNullOrEmpty(aud.url));
            Assert.False(string.IsNullOrEmpty(aud.message));
        }

        [Fact]
        public void Test_AuditProgram_MatchesAssignedValue()
        {
            Audit aud = new Audit { program = "ChecklistSave" };
            Assert.Equal("ChecklistSave", aud.program);
        }

        [Fact]
        public void Test_AuditAction_MatchesAssignedValue()
        {
            Audit aud = new Audit { action = "delete" };
            Assert.Equal("delete", aud.action);
        }

        [Fact]
        public void Test_AuditCreated_MatchesAssignedDateTime()
        {
            DateTime now = DateTime.UtcNow;
            Audit aud = new Audit { created = now };
            Assert.Equal(now, aud.created);
        }

        [Fact]
        public void Test_AuditInternalId_CanBeAssigned()
        {
            ObjectId id = ObjectId.GenerateNewId();
            Audit aud = new Audit { InternalId = id };
            Assert.Equal(id, aud.InternalId);
        }

        [Fact]
        public void Test_AuditInternalIdString_ReturnsNonEmptyString()
        {
            ObjectId id = ObjectId.GenerateNewId();
            Audit aud = new Audit { InternalId = id };
            Assert.False(string.IsNullOrEmpty(aud.InternalIdString));
            Assert.Equal(id.ToString(), aud.InternalIdString);
        }

        [Fact]
        public void Test_AuditAuditId_CanBeOverridden()
        {
            Guid fixedId = Guid.NewGuid();
            Audit aud = new Audit { auditId = fixedId };
            Assert.Equal(fixedId, aud.auditId);
        }

        // -----------------------------------------------------------------------
        // FAIL tests — missing / empty / null data scenarios
        // -----------------------------------------------------------------------

        [Fact]
        public void Test_AuditProgram_IsNullByDefault()
        {
            Audit aud = new Audit();
            Assert.Null(aud.program);
        }

        [Fact]
        public void Test_AuditAction_IsNullByDefault()
        {
            Audit aud = new Audit();
            Assert.Null(aud.action);
        }

        [Fact]
        public void Test_AuditUserid_IsNullByDefault()
        {
            Audit aud = new Audit();
            Assert.Null(aud.userid);
        }

        [Fact]
        public void Test_AuditUsername_IsNullByDefault()
        {
            Audit aud = new Audit();
            Assert.Null(aud.username);
        }

        [Fact]
        public void Test_AuditFullname_IsNullByDefault()
        {
            Audit aud = new Audit();
            Assert.Null(aud.fullname);
        }

        [Fact]
        public void Test_AuditEmail_IsNullByDefault()
        {
            Audit aud = new Audit();
            Assert.Null(aud.email);
        }

        [Fact]
        public void Test_AuditUrl_IsNullByDefault()
        {
            Audit aud = new Audit();
            Assert.Null(aud.url);
        }

        [Fact]
        public void Test_AuditMessage_IsNullByDefault()
        {
            Audit aud = new Audit();
            Assert.Null(aud.message);
        }

        [Fact]
        public void Test_AuditInternalIdString_IsEmptyForDefaultObjectId()
        {
            Audit aud = new Audit();
            // Default InternalId is ObjectId.Empty; its string is 24 zeros
            Assert.Equal(ObjectId.Empty.ToString(), aud.InternalIdString);
        }

        [Fact]
        public void Test_AuditProgram_EmptyStringIsNotNull()
        {
            Audit aud = new Audit { program = string.Empty };
            // program is set but logically empty — not null, but IsNullOrEmpty is true
            Assert.True(string.IsNullOrEmpty(aud.program));
            Assert.NotNull(aud.program); // empty string is not null
        }
    }
}
