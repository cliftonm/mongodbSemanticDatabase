using System;
using System.Collections.Generic;
using System.Linq;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Clifton.Core.ExtensionMethods;

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
			BsonDocument doc = BsonDocument.Parse(jobj.ToString());

			return Insert(schema, doc);
		}

		protected string Insert(Schema schema, BsonDocument doc)
		{
			string id = null;

			if (schema.IsConcreteType)
			{
				int refCount;

				if (IsDuplicate(schema.Name, doc, out id, out refCount))
				{
					IncrementRefCount(schema.Name, id, refCount);
				}
				else
				{
					BsonDocument withRef = AddRef1(doc);
					id = Insert(schema.Name, withRef);
				}
			}
			else
			{
				BsonDocument currentObject = GetConcreteObjects(schema, doc);
				BsonDocument subjobj = RemoveCurrentConcreteObjects(schema, doc);
				RecurseIntoSubtypes(schema, currentObject, subjobj);
				int refCount;

				if (IsDuplicate(schema.Name, currentObject, out id, out refCount))
				{
					IncrementRefCount(schema.Name, id, refCount);
				}
				else
				{
					BsonDocument withRef = AddRef1(currentObject);
					id = Insert(schema.Name, withRef);
				}
			}

			return id;
		}

		public List<BsonDocument> Query(Schema schema, string id = null)
		{
			List<BsonDocument> records = new List<BsonDocument>();

			records = GetAll(schema.Name, id);

			foreach (BsonDocument record in records)
			{
				record.Remove("_ref");

				foreach (Schema subtype in schema.Subtypes)
				{
					string childIdName = subtype.Name + "Id";
					// Remove the FK ID, as we don't want it in the final recordset
					string childId = record[childIdName].ToString();
					record.Remove(childIdName);
					List<BsonDocument> childRecords = Query(subtype, childId);

					// TODO: Assert that childRecords <= 1, and we know only one child record exists because we don't allow duplicates.
					if (childRecords.Count == 1)
					{
						childRecords[0].Elements.ForEach(p => record.Add(p.Name, childRecords[0][p.Name]));
					}
				}
			}

			return records;
		}

		public List<BsonDocument> QueryServerSide(Schema schema, string id = null)
		{
			var collection = db.GetCollection<BsonDocument>(schema.Name);
			List<string> projections = new List<string>();
			List<string> pipeline = BuildQueryPipeline(schema, String.Empty, projections);
			pipeline.Add(String.Format("{{$project: {{{0}, '_id':0}} }}", String.Join(",", projections)));
			var aggr = collection.Aggregate();
			pipeline.ForEach(s => aggr = aggr.AppendStage<BsonDocument>(s));
			List<BsonDocument> records = aggr.ToList();

			return records;
		}

		/// <summary>
		/// Returns all but the _id field of a MongoDB collection.
		/// </summary>
		public List<BsonDocument> GetAll(string collectionName, string id = null)
		{
			BsonDocument filter = GetIdFilterDocument(id);
			// Empty filter, and remove the _id from the set if returned fields.
			List<BsonDocument> docs = db.GetCollection<BsonDocument>(collectionName).Find(filter).Project("{_id:0}").ToList();

			return docs;
		}

		protected List<string> BuildQueryPipeline(Schema schema, string parentName, List<string> projections)
		{
			List<string> pipeline = new List<string>();

			schema.ConcreteTypes.ForEach(kvp => projections.Add(String.Format("'{0}':'${1}'", kvp.Key, parentName + kvp.Key)));

			foreach (Schema subtype in schema.Subtypes)
			{
				pipeline.Add(String.Format("{{$lookup: {{from: '{0}', localField:'{2}{1}', foreignField: '_id', as: '{0}'}} }},", subtype.Name, subtype.Name + "Id", parentName));
				pipeline.Add(String.Format("{{$unwind: '${0}'}}", subtype.Name));
				List<string> subpipeline = BuildQueryPipeline(subtype, subtype.Name + ".", projections);

				if (subpipeline.Count > 0)
				{
					pipeline[pipeline.Count - 1] = pipeline.Last() + ",";
					pipeline.AddRange(subpipeline);
				}
			}

			return pipeline;
		}

		protected BsonDocument GetIdFilterDocument(string id)
		{
			BsonDocument ret;

			if (id == null)
			{
				ret = new BsonDocument();			// empty filter.
			}
			else
			{
				ret = new BsonDocument("_id", new ObjectId(id));
			}

			return ret;
		}

		protected void IncrementRefCount(string collectionName, string id, int refCount)
		{
			++refCount;
			var collection = db.GetCollection<BsonDocument>(collectionName);
			var filter = new BsonDocument("_id", new ObjectId(id));
			var update = Builders<BsonDocument>.Update.Set("_ref", refCount);
			collection.UpdateOne(filter, update);
		}

		protected BsonDocument AddRef1(BsonDocument jobj)
		{
			BsonDocument withRef = new BsonDocument(jobj);
			withRef.Add("_ref", 1);

			return withRef;
		}

		protected bool IsDuplicate(string collectionName, BsonDocument doc, out string id, out int refCount)
		{
			bool exists = false;
			id = null;
			refCount = 0;
			List<BsonDocument> docs = db.GetCollection<BsonDocument>(collectionName).Find(doc).ToList();
			exists = docs.Count == 1;
			// TODO: Assert that docs.Count is never > 1

			if (exists)
			{
				id = docs[0].Elements.Single(el => el.Name == "_id").Value.ToString();
				refCount = docs[0].Elements.Single(el => el.Name == "_ref").Value.ToInt32();
			}

			return exists;
		}

		protected string Insert(string collectionName, BsonDocument doc)
		{
			db.GetCollection<BsonDocument>(collectionName).InsertOne(doc);
			BsonValue bid = doc.Elements.Single(el => el.Name == "_id").Value;
			string id = bid.ToString();

			return id;
		}

		protected BsonDocument GetConcreteObjects(Schema schema, BsonDocument doc)
		{
			BsonDocument newDoc = new BsonDocument();

			// Add the current schema's concrete types to the new JSON object.
			foreach (string concreteType in schema.ConcreteTypes.Keys)
			{
				newDoc.Add(concreteType, doc[concreteType]);
			}

			return newDoc;
		}

		protected BsonDocument RemoveCurrentConcreteObjects(Schema schema, BsonDocument doc)
		{
			BsonDocument subdoc = new BsonDocument(doc);

			foreach (string concreteType in schema.ConcreteTypes.Keys)
			{
				subdoc.Remove(concreteType);
			}

			return subdoc;
		}

		protected void RecurseIntoSubtypes(Schema schema, BsonDocument currentObject, BsonDocument subdoc)
		{
			foreach (Schema subtype in schema.Subtypes)
			{
				string subtypeId = Insert(subtype, subdoc);
				// TODO: Assert that the subtype name is unique.
				// Insert the object ID's referencing the subtypes
				currentObject.Add(subtype.Name + "Id", new ObjectId(subtypeId));
			}
		}

		protected void CreateConcreteType(Schema typeDef)
		{
			CreateCollection(typeDef.Name);
		}
	}
}
