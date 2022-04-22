using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Encryption;

    public class ExplicitEncryptionAndAutoDecryptionExamples
    {
        private const string LocalMasterKey = "Mng0NCt4ZHVUYUJCa1kxNkVyNUR1QURhZ2h2UzR2d2RrZzh0cFBwM3R6NmdWMDFBMUN3YkQ5aXRRMkhGRGdQV09wOGVNYUMxT2k3NjZKelhaQmRCZGJkTXVyZG9uSjFk";

        //	private const string  ConnectionString = "mongodb+srv://user:password@cluster0.xxx.mongodb.net/test?retryWrites=true&w=majority";
        
        public static string? ConnectionString = Environment.GetEnvironmentVariable("MONGODB_URI");

        public static void Main(string[] args)
        {
            var localMasterKey = Convert.FromBase64String(LocalMasterKey);
            var kmsProviders = new Dictionary<string, IReadOnlyDictionary<string, object>>();
            var localKey = new Dictionary<string, object>
            {
                { "key", localMasterKey }
            };
            kmsProviders.Add("local", localKey);

            var keyVaultNamespace = CollectionNamespace.FromFullName("keyvault.datakeys");
            var collectionNamespace = CollectionNamespace.FromFullName("csfle_db.test_coll");
            var autoEncryptionOptions = new AutoEncryptionOptions(
                keyVaultNamespace,
                kmsProviders,
                bypassAutoEncryption: true);
            var clientSettings = MongoClientSettings.FromConnectionString( ConnectionString );
            clientSettings.AutoEncryptionOptions = autoEncryptionOptions;
            var mongoClient = new MongoClient(clientSettings);
            var database = mongoClient.GetDatabase(collectionNamespace.DatabaseNamespace.DatabaseName);
            database.DropCollection(collectionNamespace.CollectionName);
            var collection = database.GetCollection<BsonDocument>(collectionNamespace.CollectionName);

            var keyVaultClient = new MongoClient( ConnectionString );
            var keyVaultDatabase = keyVaultClient.GetDatabase(keyVaultNamespace.DatabaseNamespace.DatabaseName);
            keyVaultDatabase.DropCollection(keyVaultNamespace.CollectionName);

            // Create the ClientEncryption instance
            var clientEncryptionSettings = new ClientEncryptionOptions(
                keyVaultClient,
                keyVaultNamespace,
                kmsProviders);
            using (var clientEncryption = new ClientEncryption(clientEncryptionSettings))
            {
                var dataKeyId = clientEncryption.CreateDataKey(
                    "local",
                    new DataKeyOptions(),
                    CancellationToken.None);

                var originalString = "123456789";
                Console.WriteLine($"Original string {originalString}.");

                // Explicitly encrypt a field
                var encryptOptions = new EncryptOptions(
                    EncryptionAlgorithm.AEAD_AES_256_CBC_HMAC_SHA_512_Deterministic.ToString(),
                    keyId: dataKeyId);
                var encryptedFieldValue = clientEncryption.Encrypt(
                    originalString,
                    encryptOptions,
                    CancellationToken.None);
                Console.WriteLine($"Encrypted value {encryptedFieldValue}.");

                collection.InsertOne(new BsonDocument("encryptedField", encryptedFieldValue));

                // Automatically decrypts the encrypted field.
                var decryptedValue = collection.Find(FilterDefinition<BsonDocument>.Empty).First();
                Console.WriteLine($"Decrypted document {decryptedValue.ToJson()}.");
            }
        }
    }
