// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using IdentityServer4.Models;
using SpringComp.IdentityServer.TableStorage.Stores;
using SpringComp.IdentityServer4.Azure.Tables.Options;
using Xunit;

namespace SpringComp.IdentityServer4.Azure.Tables.Stores
{
    public class PersistedGrantStoreTests : StoreTests
    {
        private static readonly OperationalTestDatabaseOptions StoreOptions = new OperationalTestDatabaseOptions();

        private static PersistedGrant CreateTestObject()
        {
            return new PersistedGrant
            {
                Key = "cixjnH510x/aq25gCeVvI0h400000000JYC5jCeHroo=",
                Type = "authorization_code",
                ClientId = Guid.NewGuid().ToString(),
                SubjectId = "ccfe87807159f8db181000000000f8a761d9885180b10eb562105c300c079690",
                CreationTime = new DateTime(2016, 08, 01),
                Expiration = new DateTime(2016, 08, 31),
                Data = Guid.NewGuid().ToString()
            };
        }

        [Fact]
        public void GetAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            var store = new PersistedGrantStore(StoreOptions, new FakeLogger<PersistedGrantStore>());

            var persistedGrant = CreateTestObject();
            store.StoreAsync(persistedGrant).Wait();

            var foundPersistedGrants = store.GetAllAsync(persistedGrant.SubjectId).Result.ToList();
            Assert.NotNull(foundPersistedGrants);
            Assert.NotEmpty(foundPersistedGrants);
            Assert.Single(foundPersistedGrants);
            Assert.Equal(persistedGrant.SubjectId, foundPersistedGrants[0].SubjectId);
        }

        [Fact]
        public void RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted()
        {
            var store = new PersistedGrantStore(StoreOptions, new FakeLogger<PersistedGrantStore>());

            var persistedGrant = CreateTestObject();
            store.StoreAsync(persistedGrant).Wait();

            store.RemoveAsync(persistedGrant.Key).Wait();

            var foundGrant = store.GetAsync(persistedGrant.Key).Result;
            Assert.Null(foundGrant);
        }

        [Fact]
        public void RemoveAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted()
        {
            var store = new PersistedGrantStore(StoreOptions, new FakeLogger<PersistedGrantStore>());

            var persistedGrant = CreateTestObject();
            store.StoreAsync(persistedGrant).Wait();

            store.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId).Wait();

            var foundGrant = store.GetAsync(persistedGrant.Key).Result;
            Assert.Null(foundGrant);
        }

        [Fact]
        public void RemoveAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted()
        {
            var store = new PersistedGrantStore(StoreOptions, new FakeLogger<PersistedGrantStore>());

            var persistedGrant = CreateTestObject();
            store.StoreAsync(persistedGrant).Wait();

            store.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId, persistedGrant.Type).Wait();

            var foundGrant = store.GetAsync(persistedGrant.Key).Result;
            Assert.Null(foundGrant);
        }

        [Fact]
        public void Store_should_create_new_record_if_key_does_not_exist()
        {
            var store = new PersistedGrantStore(StoreOptions, new FakeLogger<PersistedGrantStore>());
            var persistedGrant = CreateTestObject();

            store.RemoveAsync(persistedGrant.Key).Wait();
            var missingGrant = store.GetAsync(persistedGrant.Key).Result;
            Assert.Null(missingGrant);

            store.StoreAsync(persistedGrant).Wait();

            var foundGrant = store.GetAsync(persistedGrant.Key).Result;
            Assert.NotNull(foundGrant);
        }

        [Fact]
        public void Store_should_update_record_if_key_already_exists()
        {
            var store = new PersistedGrantStore(StoreOptions, new FakeLogger<PersistedGrantStore>());

            var persistedGrant = CreateTestObject();
            store.StoreAsync(persistedGrant).Wait();

            Assert.NotNull(persistedGrant.Expiration);
            var newDate = persistedGrant.Expiration.Value.AddHours(1);
            persistedGrant.Expiration = newDate;
            store.StoreAsync(persistedGrant).Wait();

            var foundGrant = store.GetAsync(persistedGrant.Key).Result;
            Assert.NotNull(foundGrant);
            Assert.Equal(newDate, persistedGrant.Expiration);
        }
    }
}