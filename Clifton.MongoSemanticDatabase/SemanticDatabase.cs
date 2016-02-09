using System;
using System.Collections.Generic;
using System.Linq;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Clifton.MongoSemanticDatabase
{
    public class SemanticDatabase
    {
		protected IMongoClient client;
		protected IMongoDatabase db;

		public SemanticDatabase()
		{
			client = new MongoClient();
		}

		public void DropDatabase(string dbName)
		{
			client.DropDatabase(dbName);
		}

		public void Open(string dbName)
		{
			db = client.GetDatabase(dbName);
		}

		public void CreateCollection(string collectionName)
		{
			// This throws a NullReferenceException!
			// See here: https://jira.mongodb.org/browse/CSHARP-1524
			// This is apparently fixed in version 2.2.1
			// db.CreateCollection(collectionName);
			
			// For now, we use this workaround:

			// As per the documentation: MongoDB creates collections automatically when they are first used, so you only need to call this method if you want to provide non-default options.
			// What we do here is create a collection with a single entry, then delete that item, thus resulting in an empty collection.  
			// While I get the "don't create it until you need it" concept, there are reasons (like my unit tests) for why I want the collection actually physically created.
			var data = new BsonDocument(collectionName, "{Value : 0}");
			var collection = db.GetCollection<BsonDocument>(collectionName);
			collection.InsertOne(data);
			var result = collection.DeleteOne(new BsonDocument("_id", data.Elements.Single(el => el.Name == "_id").Value));
		}

		public List<string> GetCollections()
		{
			List<string> items = db.ListCollections().ToList().Select(doc => doc.Elements.Single(el => el.Name == "name").Value.AsString).ToList();

			return items;
		}

		public void InstantiateSchema(Schema typeDef)
		{
			if (typeDef.IsConcreteType)
			{
				CreateConcreteType(typeDef);
			}
			else
			{
				CreateCollection(typeDef.Name);

				foreach (Schema subtype in typeDef.Subtypes)
				{
					InstantiateSchema(subtype);
				}
			}
		}

		public string Insert(Schema schema, JObject jobj)
		{
			string id = null;

			if (schema.IsConcreteType)
			{
				id = Insert(schema.Name, jobj);
			}
			else
			{
				JObject currentObject = GetConcreteObjects(schema, jobj);
				JObject subjobj = RemoveCurrentConcreteObjects(schema, jobj);
				RecurseIntoSubtypes(schema, currentObject, subjobj);
				id = Insert(schema.Name, currentObject);
			}

			return id;
		}

		protected string Insert(string collectionName, JObject jobj)
		{
			BsonDocument doc = BsonDocument.Parse(jobj.ToString());
			db.GetCollection<BsonDocument>(collectionName).InsertOne(doc);
			BsonValue bid = doc.Elements.Single(el => el.Name == "_id").Value;
			string id = bid.ToString();

			return id;
		}

		protected JObject GetConcreteObjects(Schema schema, JObject jobj)
		{
			JObject newjobj = new JObject();

			// Add the current schema's concrete types to the new JSON object.
			foreach (string concreteType in schema.ConcreteTypes.Keys)
			{
				newjobj.Add(concreteType, jobj[concreteType]);
			}

			return newjobj;
		}

		protected JObject RemoveCurrentConcreteObjects(Schema schema, JObject jobj)
		{
			JObject subjobj = new JObject(jobj);

			foreach (string concreteType in schema.ConcreteTypes.Keys)
			{
				subjobj.Remove(concreteType);
			}

			return subjobj;
		}

		protected void RecurseIntoSubtypes(Schema schema, JObject currentObject, JObject subjobj)
		{
			foreach (Schema subtype in schema.Subtypes)
			{
				string subtypeId = Insert(subtype, subjobj);
				// TODO: Assert that the subtype name is unique.
				// Insert the object ID's referencing the subtypes
				currentObject.Add(subtype.Name + "Id", subtypeId);
			}
		}


		public List<string> GetAll(string collectionName)
		{
			List<string> ret = new List<string>();
			// Empty filter, and remove the _id from the set if returned fields.
			List<BsonDocument> docs = db.GetCollection<BsonDocument>(collectionName).Find(new BsonDocument()).Project("{_id:0}").ToList();

			foreach (BsonDocument doc in docs)
			{
				string json = doc.ToJson();
				ret.Add(json);
			}

			return ret;
		}

		protected void CreateConcreteType(Schema typeDef)
		{
			CreateCollection(typeDef.Name);
		}
    }
}
