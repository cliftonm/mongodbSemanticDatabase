﻿using System;
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

		public string Update(Schema schema, JObject jobjOriginal, JObject jobjNew)
		{
			BsonDocument docOriginal = BsonDocument.Parse(jobjOriginal.ToString());
			BsonDocument docNew = BsonDocument.Parse(jobjNew.ToString());

			return Update(schema, docOriginal, docNew);
		}

		public void Delete(Schema schema, JObject jobj)
		{
			BsonDocument doc = BsonDocument.Parse(jobj.ToString());

			Delete(schema, doc);
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
				InsertRecurseIntoSubtypes(schema, currentObject, subjobj);
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

		/// <summary>
		/// Deleting a semantic type hierarchy involves checking the _ref count.
		/// Only records with a _ref count of 1 can be deleted.  We have to drill
		/// into the lowest type in the hierarchy to determine whether we can 
		/// delete super-types.
		/// </summary>
		protected string Delete(Schema schema, BsonDocument doc)
		{
			string id = null;

			if (schema.IsConcreteType)
			{
				int refCount = GetRefCount(schema.Name, doc, out id);

				if (refCount == 1)
				{
					Delete(schema.Name, id);
				}
				else
				{
					DecrementRefCount(schema.Name, id, refCount);
				}
			}
			else
			{
				BsonDocument currentObject = GetConcreteObjects(schema, doc);
				BsonDocument subjobj = RemoveCurrentConcreteObjects(schema, doc);
				DeleteRecurseIntoSubtypes(schema, currentObject, subjobj);
				int refCount = GetRefCount(schema.Name, currentObject, out id);

				if (refCount == 1)
				{
					Delete(schema.Name, id);
				}
				else
				{
					DecrementRefCount(schema.Name, id, refCount);
				}
			}

			return id;
		}

		/// <summary>
		/// The complete set of values for the original semantic type must be provided as well as the new values -- we can't actually just update a value based on some primary key.
		/// If there are no other references to the semantic type, the concrete types can simply be updated.
		/// If there are other references:
		///		the reference count for the current type must be decremented
		///		a new instance of the type must be inserted
		///		the super-type's "foreign key" reference must be updated
		///		this process needs to recurse upwards through the hierarchy
		/// </summary>
		protected string Update(Schema schema, BsonDocument docOriginal, BsonDocument docNew)
		{
			string id = null;

			if (schema.IsConcreteType)
			{
				int refCount = GetRefCount(schema.Name, docOriginal, out id);

				if (refCount == 1)
				{
					Update(schema.Name, id, docOriginal, docNew);
				}
				else
				{
					DecrementRefCount(schema.Name, id, refCount);
					id = Insert(schema, docNew);
				}
			}
			else
			{
				BsonDocument currentOriginalObject = GetConcreteObjects(schema, docOriginal);
				BsonDocument subOriginalJobj = RemoveCurrentConcreteObjects(schema, docOriginal);
				BsonDocument currentNewObject = GetConcreteObjects(schema, docNew);
				BsonDocument subNewJobj = RemoveCurrentConcreteObjects(schema, docNew);
				UpdateRecurseIntoSubtypes(schema, currentOriginalObject, subOriginalJobj, currentNewObject, subNewJobj);
				int refCount = GetRefCount(schema.Name, currentOriginalObject, out id);

				if (refCount == 1)
				{
					Update(schema.Name, id, currentOriginalObject, currentNewObject);
				}
				else
				{
					DecrementRefCount(schema.Name, id, refCount);
					id = Insert(schema, docNew);
				}
			}

			return id;
		}

		protected void Update(string collectionName, string id, BsonDocument docOriginal, BsonDocument docNew)
		{
			var collection = db.GetCollection<BsonDocument>(collectionName);
			var filter = new BsonDocument("_id", new ObjectId(id));
			Dictionary<string, BsonValue> changes = GetChanges(docOriginal, docNew);

			if (changes.Count > 0)
			{
				// Get started with the first one.
				var update = Builders<BsonDocument>.Update.Set(changes.First().Key, changes.First().Value);

				// Do the rest.
				foreach (KeyValuePair<string, BsonValue> kvp in changes.Skip(1))
				{
					update = update.Set(kvp.Key, kvp.Value);
				}

				collection.UpdateOne(filter, update);
			}
		}

		/// <summary>
		/// Compares to documents, assumed to have elements that do NOT have sub-documents.
		/// </summary>
		protected Dictionary<string, BsonValue> GetChanges(BsonDocument originalDoc, BsonDocument newDoc)
		{
			Dictionary<string, BsonValue> changes = new Dictionary<string, BsonValue>();

			foreach (BsonElement el in newDoc.Elements)
			{
				// Looks like this does a value comparison (not a BsonValue reference comparison), which is good.
				if ( (originalDoc.Contains(el.Name)) && (originalDoc[el.Name] != el.Value) )
				{
					changes[el.Name] = el.Value;
				}
			}

			return changes;
		}

		protected void Delete(string collectionName, string id)
		{
			var collection = db.GetCollection<BsonDocument>(collectionName);
			var filter = new BsonDocument("_id", new ObjectId(id));
			collection.DeleteOne(filter);
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

		protected void DecrementRefCount(string collectionName, string id, int refCount)
		{
			--refCount;
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

		protected int GetRefCount(string collectionName, BsonDocument doc, out string id)
		{
			id = null;
			int refCount = 0;
			List<BsonDocument> docs = db.GetCollection<BsonDocument>(collectionName).Find(doc).ToList();
			// TODO: Assert that docs.Count == 1

			id = docs[0].Elements.Single(el => el.Name == "_id").Value.ToString();
			refCount = docs[0].Elements.Single(el => el.Name == "_ref").Value.ToInt32();

			return refCount;
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

		protected void InsertRecurseIntoSubtypes(Schema schema, BsonDocument currentObject, BsonDocument subdoc)
		{
			foreach (Schema subtype in schema.Subtypes)
			{
				string subtypeId = Insert(subtype, subdoc);
				// TODO: Assert that the subtype name is unique.
				// Insert the object ID's referencing the subtypes
				currentObject.Add(subtype.Name + "Id", new ObjectId(subtypeId));
			}
		}

		protected void DeleteRecurseIntoSubtypes(Schema schema, BsonDocument currentObject, BsonDocument subdoc)
		{
			foreach (Schema subtype in schema.Subtypes)
			{
				string subtypeId = Delete(subtype, subdoc);
				// TODO: Assert that the subtype name is unique.
				// Insert the object ID's referencing the subtypes
				currentObject.Add(subtype.Name + "Id", new ObjectId(subtypeId));
			}
		}

		protected void UpdateRecurseIntoSubtypes(Schema schema,
			BsonDocument currenOriginalObject, BsonDocument currentOriginalSubdoc,
			BsonDocument currentNewObject, BsonDocument newSubdoc)
		{
			foreach (Schema subtype in schema.Subtypes)
			{
				string subtypeId = Update(subtype, currentOriginalSubdoc, newSubdoc);
				// TODO: Assert that the subtype name is unique.
				// Insert the object ID's referencing the subtypes
				currentNewObject.Add(subtype.Name + "Id", new ObjectId(subtypeId));

				// Here we need to get the FK from the original element to be able to compare against the new FK we just set.
			}
		}

		protected void CreateConcreteType(Schema typeDef)
		{
			CreateCollection(typeDef.Name);
		}
	}
}
