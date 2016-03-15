using System;
using System.Collections.Generic;
using System.Linq;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Clifton.Core.Assertions;
using Clifton.Core.ExtensionMethods;

namespace Clifton.MongoSemanticDatabase
{
	public class SemanticDatabaseException : ApplicationException
	{
		public SemanticDatabaseException(string msg) : base(msg) { }
	}

	public class CommonType
	{
		public Schema Schema1 { get; set; }
		public Schema Schema2 { get; set; }
	}

	public class SemanticDatabase
	{
		protected IMongoClient client;
		protected IMongoDatabase db;

		/// <summary>
		/// Constructor initializes the MongoDB client.
		/// </summary>
		public SemanticDatabase()
		{
			client = new MongoClient();
		}

		/// <summary>
		/// Remove a database.
		/// </summary>
		public void DropDatabase(string dbName)
		{
			client.DropDatabase(dbName);
		}

		/// <summary>
		/// Opens an existing or new database.
		/// </summary>
		public void Open(string dbName)
		{
			db = client.GetDatabase(dbName);
		}

		/// <summary>
		/// Creates a collection.  Collections will also be auto-created by MongoDB on their first use, so calling this
		/// method really is never necessary.
		/// </summary>
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

		/// <summary>
		/// Return the names of all collections in the database.
		/// </summary>
		public List<string> GetCollections()
		{
			List<string> items = db.ListCollections().ToList().Select(doc => doc.Elements.Single(el => el.Name == "name").Value.AsString).ToList();
			// For some reason, this collection appeared on my Gateway laptop!
			items.Remove("system.indexes");

			return items;
		}

		/// <summary>
		/// Returns a MongoDB collection object.
		/// </summary>
		public IMongoCollection<BsonDocument> GetCollection(string collectionName)
		{
			return db.GetCollection<BsonDocument>(collectionName);
		}

		/// <summary>
		/// Creates the collections for the specified schema.
		/// </summary>
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

		public Schema GetAssociationSchema(Schema schema1, Schema schema2)
		{
			Schema associationSchema = CreateAssociationSchema(schema1, schema2);

			return associationSchema;
		}

		public Schema Associate(Schema schema1, Schema schema2)
		{
			Schema associationSchema = GetAssociationSchema(schema1, schema2);
			InstantiateSchema(associationSchema);

			return associationSchema;
		}

		public string Insert(Schema schema, BsonDocument doc)
		{
			return InternalInsert(schema, doc);
		}


		/// <summary>
		/// Updates the semantic type.  If the root type has concrete fields, then the root ID is not necessary.
		/// If the root type is abstract, then the root ID must be supplied.
		/// The updated record _id is returned.
		/// </summary>
		public string Update(Schema schema, BsonDocument docOriginal, BsonDocument docNew, string rootId = null)
		{
			return InternalUpdate(schema, docOriginal, docNew, rootId);
		}

		public void Delete(Schema schema, BsonDocument doc)
		{
			InternalDelete(schema, doc);
		}

		/// <summary>
		/// Delete an entry in the association schema where the doc contains the _id of the association record to delete.
		/// </summary>
		public void DeleteAssociation(Schema schema, BsonDocument doc)
		{
			// For example, person_phoneNumber
			List<BsonDocument> recs = GetAll(schema.Name, null, true, doc);
			Assert.That(recs.Count == 1, "Association record does not exist.");
			BsonDocument rootRec = recs[0];
			Assert.That(rootRec["_ref"].ToString().to_i() == 1, "Deleting an association record that is itself associated is not supported.");
			
			// For example, person_phoneNumber_Association
			string rootAssociation = schema.Name + "_Association";
			string rootAssociationId = rootRec[rootAssociation + "Id"].ToString();
			List<BsonDocument> rootAssociationRecs = GetAll(rootAssociation, rootAssociationId, true);
			Assert.That(rootAssociationRecs.Count == 1, "Expected one and only one entry for " + rootAssociation + " where _id = " + rootAssociationId);
			BsonDocument rootAssociationRec = rootAssociationRecs[0];
			string associationId = rootAssociationRec["associationId"].ToString();

			// For example, person_phoneNumber_Assocation references the "assocation" collection.
			List<BsonDocument> associationRecs = GetAll("association", associationId, true);
			Assert.That(associationRecs.Count == 1, "Expected one and only one entry for 'association' where _id = " + associationId);
			BsonDocument associationRec = associationRecs[0];
			string fwdAssocNameId = associationRec["forwardAssociationNameId"].ToString();
			string revAssocNameId = associationRec["reverseAssociationNameId"].ToString();

			// Drill into the forward/reverse AssociationName collections
			List<BsonDocument> fwdRefNamesRecs = GetAll("forwardAssociationName", fwdAssocNameId, true);
			Assert.That(fwdRefNamesRecs.Count == 1, "Expected one and only one name record with _id = " + fwdAssocNameId);
			BsonDocument fwdRefNameRec = fwdRefNamesRecs[0];
			int fwdRefNameCount = fwdRefNameRec["_ref"].ToString().to_i();
			string fwdNameId = fwdRefNameRec["nameId"].ToString();
			List<BsonDocument> revRefNamesRecs = GetAll("reverseAssociationName", revAssocNameId, true);
			Assert.That(revRefNamesRecs.Count == 1, "Expected one and only one name record with _id = " + revAssocNameId);
			BsonDocument revRefNameRec = revRefNamesRecs[0];
			int revRefNameCount = revRefNameRec["_ref"].ToString().to_i();
			string revNameId = revRefNameRec["nameId"].ToString();

			// Drill into the name collection.
			List<BsonDocument> fwdNameRecs = GetAll("name", fwdNameId, true);
			Assert.That(fwdNameRecs.Count == 1, "Expected one and only one name record with _id = " + fwdNameId);
			BsonDocument fwdNameRec = fwdNameRecs[0];
			string fwdName = fwdNameRec["name"].ToString();
			List<BsonDocument> revNameRecs = GetAll("name", revNameId, true);
			Assert.That(revNameRecs.Count == 1, "Expected one and only one name record with _id = " + revNameId);
			BsonDocument revNameRec = revNameRecs[0];
			string revName = revNameRec["name"].ToString();

			// We now have all the pieces needed to traverse back up the hierarchy.
			Schema nameSchema = GetNameSchema();
			InternalDelete(nameSchema, BsonDocument.Parse("{name: '" + fwdName + "'}"));
			InternalDelete(nameSchema, BsonDocument.Parse("{name: '" + revName + "'}"));

			// The rest have no concrete types, so we can't use InternalDelete because we haven't really implemented Delete correctly to handle cascading (downward) deletes.  A big TODO to understand this correctly.
			DecrementRefCountOrDelete("forwardAssociationName", fwdRefNameRec, fwdRefNameCount);
			DecrementRefCountOrDelete("reverseAssociationName", revRefNameRec, revRefNameCount);
			DecrementRefCountOrDelete("association", associationRec, associationRec["_ref"].ToString().to_i());
			DecrementRefCountOrDelete(rootAssociation, rootAssociationRec, rootAssociationRec["_ref"].ToString().to_i());
			DecrementRefCountOrDelete(schema.Name, rootRec, rootRec["_ref"].ToString().to_i());
		}

		protected void DecrementRefCountOrDelete(string collectionName, BsonDocument doc, int count)
		{
			if (count == 1)
			{
				Delete(collectionName, doc["_id"].ToString());
			}
			else
			{
				DecrementRefCount(collectionName, doc["_id"].ToString(), count);
			}
		}

		/// <summary>
		/// Returns a flattened view of the schema hierarchy.  The _id field is the ID of the root type.
		/// </summary>
		public List<BsonDocument> Query(Schema schema, string id = null)
		{
			return Query(schema, id, true);
		}

		public List<BsonDocument> Query(Schema schema, BsonDocument filter)
		{
			return Query(schema, null, true, filter);
		}

		protected List<BsonDocument> Query(Schema schema, string id, bool withId, BsonDocument filter = null)
		{
			List<BsonDocument> records = new List<BsonDocument>();

			records = GetAll(schema.Name, id, withId, filter);

			foreach (BsonDocument record in records)
			{
				record.Remove("_ref");

				foreach (Schema subtype in schema.Subtypes)
				{
					string childIdName = subtype.Name + "Id";
					// Remove the FK ID, as we don't want it in the final recordset

					// A semantic instance may be a partial graph.
					if (record.Contains(childIdName))
					{
						string childId = record[childIdName].ToString();
						record.Remove(childIdName);
						List<BsonDocument> childRecords = Query(subtype, childId, false);

						// TODO: Assert that childRecords <= 1, and we know only one child record exists because we don't allow duplicates.
						if (childRecords.Count == 1)
						{
							childRecords[0].Elements.ForEach(p =>
								{
									if (subtype.ConcreteTypes.Exists(ct => ct.Name == p.Name))
									{
										record.Add(subtype.GetAlias(p.Name), childRecords[0][p.Name]);
									}
									else
									{
										record.Add(p.Name, childRecords[0][p.Name]);
									}
								});
						}
					}
				}
			}

			return records;
		}

		public List<BsonDocument> QueryServerSide(Schema schema)
		{
			var collection = db.GetCollection<BsonDocument>(schema.Name);
			List<string> plan = GetPlan(schema);
			var aggr = collection.Aggregate();
			plan.ForEach(s => aggr = aggr.AppendStage<BsonDocument>(s));
			List<BsonDocument> records = aggr.ToList();

			return records;
		}

		public List<BsonDocument> QueryAssociationServerSide(Schema schema1, Schema schema2, BsonDocument filter = null)
		{
			string plan;
			
			return QueryAssociationServerSide(schema1, schema2, out plan, filter);
		}

		public List<BsonDocument> QueryAssociationServerSide(Schema schema1, Schema schema2, out string plan, BsonDocument filter = null)
		{
			List<string> fullPipeline = new List<string>();
			List<string> pipeline1;
			List<string> projections1;
			List<string> pipeline2;
			List<string> projections2;
			Dictionary<string, int> asList = new Dictionary<string, int>();
			string schema1Num = "";
			string schema2Num = "";

			if (schema1.Name == schema2.Name)
			{
				schema1Num = "1";
				schema2Num = "2";
			}

			GetPlan(schema1, out pipeline1, out projections1, asList, "", schema1Num);
			GetPlan(schema2, out pipeline2, out projections2, asList, schema2.Name + schema2Num + ".", schema2Num);
	
			// schema association join:

			List<string> associationJoin = new List<string>();
			string schema1Name = schema1.Name;
			string schema2Name = schema2.Name;
			string assocCollectionName = schema1.Name + "_" + schema2.Name;
			associationJoin.Add(String.Format("{{$lookup: {{from: '{0}', localField: '_id', foreignField: '{1}{2}Id', as: '{0}'}} }},",
				assocCollectionName, 
				schema1.Name,
				schema1Num));
			associationJoin.Add(String.Format("{{$unwind: '${0}'}},", assocCollectionName));
			associationJoin.Add(String.Format("{{$lookup: {{from: '{1}', localField: '{0}.{1}{2}Id', foreignField: '_id', as: '{1}{2}'}} }},", assocCollectionName, schema2Name, schema2Num));
			associationJoin.Add(String.Format("{{$unwind: '${0}{1}'}},", schema2Name, schema2Num));

			List<string> assocPipeline = new List<string>();
			// specific association to common assocation piece:
			assocPipeline.Add(String.Format("{{$lookup: {{from: '{0}_Association', localField: '{0}.{0}_AssociationId', foreignField: '_id', as: 'specAssoc'}} }},", assocCollectionName));
			assocPipeline.Add("{$unwind: '$specAssoc'},");

			// Common part:
			assocPipeline.Add("{$lookup: {from: 'association', localField: 'specAssoc.associationId', foreignField: '_id', as: 'assoc'} },");
			assocPipeline.Add("{$unwind: '$assoc'},");
			assocPipeline.Add("{$lookup: {from: 'forwardAssociationName', localField: 'assoc.forwardAssociationNameId', foreignField: '_id', as: 'fan'} },");
			assocPipeline.Add("{$unwind: '$fan'},");
			assocPipeline.Add("{$lookup: {from: 'reverseAssociationName', localField: 'assoc.reverseAssociationNameId', foreignField: '_id', as: 'ran'} },");
			assocPipeline.Add("{$unwind: '$ran'},");
			assocPipeline.Add("{$lookup: {from: 'name', localField: 'fan.nameId', foreignField: '_id', as: 'fanName'} },");
			assocPipeline.Add("{$unwind: '$fanName'},");
			assocPipeline.Add("{$lookup: {from: 'name', localField: 'ran.nameId', foreignField: '_id', as: 'ranName'} },");
			assocPipeline.Add("{$unwind: '$ranName'},");

			fullPipeline.AddRange(pipeline1);
			fullPipeline[fullPipeline.Count - 1] = fullPipeline.Last() + ",";
			fullPipeline.AddRange(associationJoin);
			fullPipeline.AddRange(assocPipeline);
			fullPipeline.AddRange(pipeline2);
			fullPipeline[fullPipeline.Count - 1] = fullPipeline.Last() + ",";

			List<string> allProjections = new List<string>();
			allProjections.AddRange(projections1);
			allProjections.AddRange(projections2);
			allProjections.Add("'fwdAssocName':'$fanName.name'");
			allProjections.Add("'revAssocName':'$ranName.name'");
			allProjections.Add("'" + schema1.Name + schema1Num + "Id' : '$" + assocCollectionName + "." + schema1.Name + schema1Num + "Id'");
			allProjections.Add("'" + schema2.Name + schema2Num + "Id' : '$" + assocCollectionName + "." + schema2.Name + schema2Num + "Id'");

			// fullPipeline.Add(String.Format("{{$project: {{{0}, '_id':0}} }}", String.Join(",", allProjections)));
			// We want the id of the association record.
			allProjections.Insert(0, "'_id': '$" + assocCollectionName + "._id'");
			fullPipeline.Add(String.Format("{{$project: {{{0}}} }}", String.Join(",", allProjections)));

			if (filter != null)
			{
				// TODO: The plan is missing a trailing comma after the projection.
				// TODO: Remove all the dreck where we're appending ',' to the pipeline and add it in as part of the join.  I don't think AppendStage cares about the ',' anyways.
				fullPipeline.Add("{$match: " + filter.ToString() + "}");
			}

			plan = "db." + schema1.Name + ".aggregate(" + String.Join("\r\n", fullPipeline) + ")";

			var collection = db.GetCollection<BsonDocument>(schema1.Name);
			var aggr = collection.Aggregate();
			fullPipeline.ForEach(s => aggr = aggr.AppendStage<BsonDocument>(s));

			//if (filter != null)
			//{
			//	aggr = aggr.AppendStage<BsonDocument>("{$match: "+filter.ToString()+"}");
			//}

			List<BsonDocument> records = aggr.ToList();

			return records;
		}

		public string ShowPlan(Schema schema)
		{
			string ret;
			List<string> plan = GetPlan(schema);

			// If there are no subtypes, then we don't have an aggregation, we just have a basic collection query.
			if (plan.Count == 0)
			{
				ret = "db." + schema.Name + ".find()";			// TODO: Add projection!
			}
			else
			{
				ret = "db." + schema.Name + ".aggregate(" + String.Join("\r\n", plan) + ")";
			}

			return ret;
		}

		/// <summary>
		/// Returns all data in a MongoDB collection.  By default, the _id field is not returned.  
		/// An optional _id filter can be passed in with a string.  To filter on other fields, leave id null and specify a BsonDocument filter.
		/// </summary>
		public List<BsonDocument> GetAll(string collectionName, string id = null, bool withId = false, BsonDocument filter = null)
		{
			// Filter by ID or a filter specified by the caller.
			if (id != null)
			{
				filter = GetIdFilterDocument(id);
			}
			else if (filter == null)
			{
				filter = new BsonDocument();
			}

			// Empty filter, and remove the _id from the set if returned fields.
			var query = db.GetCollection<BsonDocument>(collectionName).Find(filter);

			if (!withId)
			{
				query = query.Project("{_id:0}");
			}

			List<BsonDocument> docs = query.ToList();

			return docs;
		}

		/// <summary>
		/// Do a deep drill into the semantic types defined by each schema and determine what associations can be made between them.
		/// Association are determined by shared subtypes.  This method reports on all the parents of shared subtypes between all the
		/// schemas in the request.
		/// </summary>
		public List<CommonType> DiscoverAssociations(Schema[] schemas)
		{
			schemas.ForEach(s => s.FixupParents());
			List<CommonType> commonTypes = GetCommonTypes(schemas);

			return commonTypes;
		}

		/// <summary>
		/// Returns the schema by selecting a single record instance and recursing over its schema using a specific naming convention.
		/// This is basically the best we can do to extract a semantic type from a typeless database.
		/// Aliases and data types are undiscoverable.
		/// </summary>
		public Schema DiscoverSchema(string collectionName)
		{
			Schema schema = new Schema();
			schema.Name = collectionName;
			DiscoverSchema(collectionName, schema);

			return schema;
		}

		/// <summary>
		/// Internal recursion over subtypes.
		/// </summary>
		protected void DiscoverSchema(string collectionName, Schema schema)
		{
			IMongoCollection<BsonDocument> collection = GetCollection(collectionName);
			HashSet<string> keys = new HashSet<string>();
			BsonDocument rec = collection.Find(new BsonDocument()).FirstOrDefault();

			if (rec != null)
			{
				foreach (string key in rec.Names)
				{
					if (key.EndsWith("Id"))
					{
						string subtypeName = key.LeftOf("Id");
						Schema subtypeSchema = new Schema() { Name = subtypeName };
						DiscoverSchema(subtypeName, subtypeSchema);
						schema.Subtypes.Add(subtypeSchema);
					}
					else
					{
						// We don't know of any alias and we don't know the Type for this key.
						// Ignore _Id and _ref
						if (!key.BeginsWith("_"))
						{
							schema.ConcreteTypes.Add(new ConcreteType() { Name = key });
						}
					}
				}
			}
			else
			{
				throw new SemanticDatabaseException("There are no records, which are needed to obtain the schema.  Populate at least one record.");
			}
		}

		protected List<string> GetPlan(Schema schema, string parentName = "")
		{
			List<string> projections = new List<string>();
			Dictionary<string, int> asList = new Dictionary<string, int>();
			List<string> pipeline = BuildQueryPipeline(schema, parentName, projections, asList);

			if (pipeline.Count > 0)
			{
				pipeline[pipeline.Count - 1] = pipeline.Last() + ",";
				pipeline.Add(String.Format("{{$project: {{{0}, '_id':0}} }}", String.Join(",", projections)));
			}

			return pipeline;
		}

		protected void GetPlan(Schema schema, out List<string> pipeline, out List<string> projections, Dictionary<string, int> asList, string parentName = "", string schemaNum = "")
		{
			projections = new List<string>();
			pipeline = BuildQueryPipeline(schema, parentName, projections, asList, schemaNum);
		}

		protected List<CommonType> GetCommonTypes(Schema[] schemas)
		{
			List<CommonType> commonTypes = new List<CommonType>();
			List<List<Schema>> schemaTypes = new List<List<Schema>>();

			for (int i = 0; i < schemas.Length; i++)
			{
				schemaTypes.Add(schemas[i].GetTypes());
			}

			for (int i = 0; i < schemas.Length; i++)
			{
				foreach (Schema schema1 in schemaTypes[i])
				{
					for (int j = i + 1; j < schemas.Length; j++)
					{
						foreach (Schema schema2 in schemaTypes[j])
						{
							if (schema1.Name == schema2.Name)
							{
								commonTypes.Add(new CommonType() { Schema1 = schema1, Schema2 = schema2 });
							}
						}
					}
				}
			}

			return commonTypes;
		}

		protected string InternalInsert(Schema schema, BsonDocument doc)
		{
			string id = null;

			if (schema.IsConcreteType)
			{
				int refCount;
				BsonDocument currentObject = GetConcreteObjects(schema, doc);

				if (currentObject.Elements.Count() == 0)
				{
					id = null;			// nothing to insert!
				}
				else
				{
					BsonDocument dealiasedDocument = DeAliasDocument(schema, currentObject);

					if (IsDuplicate(schema.Name, dealiasedDocument, out id, out refCount))
					{
						IncrementRefCount(schema.Name, id, refCount);
					}
					else
					{
						BsonDocument withRef = AddRef1(dealiasedDocument);
						id = InsertRecord(schema, withRef);
					}
				}
			}
			else
			{
				BsonDocument currentObject = GetConcreteObjects(schema, doc);
				BsonDocument subjobj = RemoveCurrentConcreteObjects(schema, doc);
				InsertRecurseIntoSubtypes(schema, currentObject, subjobj);
				int refCount;

				if (currentObject.Elements.Count() == 0)
				{
					id = null;			// nothing to insert!
				}
				else
				{
					BsonDocument dealiasedDocument = DeAliasDocument(schema, currentObject);

					if (IsDuplicate(schema.Name, dealiasedDocument, out id, out refCount))
					{
						IncrementRefCount(schema.Name, id, refCount);
					}
					else
					{
						BsonDocument withRef = AddRef1(dealiasedDocument);
						id = InsertRecord(schema, withRef);
					}
				}
			}

			return id;
		}

		protected BsonDocument DeAliasDocument(Schema schema, BsonDocument doc)
		{
			BsonDocument ret = new BsonDocument();

			foreach (BsonElement el in doc.Elements)
			{
				string dealiasedName;

				if (schema.ContainsAliasedType(el.Name, out dealiasedName))
				{
					ret[dealiasedName] = el.Value;
				}
				else
				{
					// This preserves reference ID's.
					ret[el.Name] = el.Value;
				}
			}

			return ret;
		}

		/// <summary>
		/// Deleting a semantic type hierarchy involves checking the _ref count.
		/// Only records with a _ref count of 1 can be deleted.  We have to drill
		/// into the lowest type in the hierarchy to determine whether we can 
		/// delete super-types.
		/// </summary>
		protected string InternalDelete(Schema schema, BsonDocument doc)
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

		protected bool ConcreteTypesChanged(Schema schema, BsonDocument docOriginal, BsonDocument docNew)
		{
			BsonDocument originalObject = GetConcreteObjects(schema, docOriginal);
			BsonDocument newObject = GetConcreteObjects(schema, docNew);

			return originalObject != newObject;
		}

		// TODO: Test updating only one subtype where a type has multiple subtypes of the same type.
		// Example: Update firstName only for Person.
		// Test that this resolves reference counting and optional insert if the reference count was > 1

		// TODO: Test that updating only one field does not trigger a change in reference counts of the non-changed field, or an insert of the non-changed field.

		// TODO: Test that inserting/updating a hierarchy where the root (or any other level) has concrete types inserts/updates the subtype concrete types as well.

		/// <summary>
		/// The complete set of values for the original semantic type must be provided as well as the new values -- we can't actually just update a value based on some primary key.
		/// If there are no other references to the semantic type, the concrete types can simply be updated.
		/// If there are other references:
		///		the reference count for the current type must be decremented
		///		a new instance of the type must be inserted
		///		the super-type's "foreign key" reference must be updated
		///		this process needs to recurse upwards through the hierarchy
		/// </summary>
		protected string InternalUpdate(Schema schema, BsonDocument docOriginal, BsonDocument docNew, string schemaId)
		{
			string id = null;

			if (schema.IsConcreteType)
			{
				if (ConcreteTypesChanged(schema, docOriginal, docNew))
				{
					int refCount;

					if (schemaId == null)
					{
						refCount = GetRefCount(schema.Name, docOriginal, out id);
					}
					else
					{
						refCount = GetRefCount(schema.Name, schemaId);
						id = schemaId;
					}

					if (refCount == 1)
					{
						// Determine if another entry matching the concrete types exist.
						// If so, increment the ref count of that record and return it's ID.
						// We can then delete the record with the current id, as it is no longer referenced.
						string existingRecId;
						BsonDocument currentNewObject = DeAliasDocument(schema, GetConcreteObjects(schema, docNew));
						int existingRecRefCount; // = GetRefCount(schema.Name, currentNewObject, out existingRecId);
						

						// if (existingRecRefCount != 0)
						if (IsDuplicate(schema.Name, currentNewObject, out existingRecId, out existingRecRefCount))
						{
							// Anything to do?
							if (id != existingRecId)
							{
								IncrementRefCount(schema.Name, existingRecId, existingRecRefCount);
								Delete(schema.Name, id);
								id = existingRecId;
							}
						}
						else
						{
							Update(schema, id, docOriginal, docNew);
						}
					}
					else
					{
						// We never have 0 references, because this would have meant decrementing from 1, which would instead trigger and update above.

						DecrementRefCount(schema.Name, id, refCount);
						id = InternalInsert(schema, docNew);
					}
				}
				else
				{
					id = schemaId;
				}
			}
			else
			{
				BsonDocument currentOriginalObject = GetConcreteObjects(schema, docOriginal);
				BsonDocument record = null;

				if (schemaId == null)
				{
					// We must use the concrete objects to determine the record.
					// If there are no concrete objects, we have an error.
					// There should be a single unique record for the concrete object.
					if (currentOriginalObject.Elements.Count() == 0)
					{
						throw new SemanticDatabaseException("Cannot update the a semantic type starting with the abstract type " + schema.Name);
					}

					record = GetRecord(schema.Name, currentOriginalObject);

					if (record == null)
					{
						throw new SemanticDatabaseException("The original record for the semantic type " + schema.Name + " cannot be found.\r\nData: " + currentOriginalObject.ToString());
					}
				}
				else
				{
					// We use the subtype id to get the record.
					record = GetRecord(schema.Name, new BsonDocument("_id", new ObjectId(schemaId)));

					if (record == null)
					{
						throw new SemanticDatabaseException("An instance of " + schema.Name + " with _id = " + schemaId + " does not exist!");
					}
				}

				BsonDocument subOriginalJobj = RemoveCurrentConcreteObjects(schema, docOriginal);

				if (subOriginalJobj.Elements.Count() == 0)
				{
					// There is nothing further to do, as we're not changing anything further in the hierarchy.
					// Update the current concrete types.
					id = record.Elements.Single(el => el.Name == "_id").Value.ToString();
					int refCount = record.Elements.Single(el => el.Name == "_ref").Value.ToInt32();

					if (refCount == 1)
					{
						BsonDocument currentNewObject = GetConcreteObjects(schema, docNew);
						Update(schema, id, record, currentNewObject);
					}
					else
					{
						// TODO: THIS CODE PATH IS NOT TESTED!
						// Now we have a problem -- something else is referencing this record other than our current hierarch, 
						// but we don't know what.  But we're updating this particular type instance.  
						// Do all the super-types reflect this change in the subtype?  We'll assume no.
						// Only this hierarchy gets updated.
						DecrementRefCount(schema.Name, id, refCount);
						id = InternalInsert(schema, docNew);

						// Otherwise:
						// All supertypes referencing this hierarchy get updated.
						//BsonDocument currentNewObject = GetConcreteObjects(schema, docNew);
						//Update(schema.Name, id, record, currentNewObject);
					}
				}
				else
				{
					BsonDocument currentNewObject = GetConcreteObjects(schema, docNew);
					BsonDocument subNewJobj = RemoveCurrentConcreteObjects(schema, docNew);
					UpdateRecurseIntoSubtypes(schema, record, currentOriginalObject, subOriginalJobj, currentNewObject, subNewJobj);
					// int refCount = GetRefCount(schema.Name, currentOriginalObject, out id);
					id = record.Elements.Single(el => el.Name == "_id").Value.ToString();
					int refCount = record.Elements.Single(el => el.Name == "_ref").Value.ToInt32();

					if (refCount == 1)
					{
						// Determine if another entry exists matching the concrete type for this schema plus the subtype references.
						// Notice the variance with the concrete update -- here we include subtype id's as well.
						// If so, increment the ref count of that record and return it's ID.
						// We can then delete the record with the current id, as it is no longer referenced.
						
						// currentNewObject includes the concrete objects + our subtype id's, as inserted by UpdateRecurseIntoSubTypes
						string existingRecId;
						int existingRecRefCount;
						BsonDocument dealiasedCurrentNewObject = DeAliasDocument(schema, currentNewObject);

						if (IsDuplicate(schema.Name, dealiasedCurrentNewObject, out existingRecId, out existingRecRefCount))
						{
							// Is this the same record?
							if (id != existingRecId)
							{
								IncrementRefCount(schema.Name, existingRecId, existingRecRefCount);
								Delete(schema.Name, id);
								id = existingRecId;
							}
						}
						else
						{
							Update(schema, id, record, currentNewObject);
						}
					}
					else
					{
						// We never have 0 references, because this would have meant decrementing from 1, which would instead trigger and update above.
						DecrementRefCount(schema.Name, id, refCount);
						id = InternalInsert(schema, docNew);
					}
				}
			}

			return id;
		}

		protected void Update(Schema schema, string id, BsonDocument docOriginal, BsonDocument docNew)
		{
			string collectionName = schema.Name;
			var collection = db.GetCollection<BsonDocument>(collectionName);
			var filter = new BsonDocument("_id", new ObjectId(id));
			Dictionary<string, BsonValue> changes = GetChanges(schema, docOriginal, docNew);

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
		/// Compares two documents, assumed to have elements that do NOT have sub-documents.
		/// Only the concrete types for the schema are compared, and the resulting map is de-aliased.
		/// </summary>
		protected Dictionary<string, BsonValue> GetChanges(Schema schema, BsonDocument originalDoc, BsonDocument newDoc)
		{
			Dictionary<string, BsonValue> changes = new Dictionary<string, BsonValue>();

			foreach (ConcreteType ct in schema.ConcreteTypes)
			{
				// Looks like this does a value comparison (not a BsonValue reference comparison), which is good.
				// Field exists in original, and value is different...
				if ( (originalDoc.Contains(ct.Alias)) && (originalDoc[ct.Alias] != newDoc[ct.Alias]) )
				{
					changes[ct.Name] = newDoc[ct.Alias];
				}
				// TODO: What about field is only in the new document?
				// TODO: What about field is missing in the new document?
			}

			// Also include subtype references that exist in the new document.
			foreach (Schema subtype in schema.Subtypes)
			{
				if (newDoc.Contains(subtype.NameAsId))
				{
					if (originalDoc.Contains(subtype.NameAsId))
					{
						if (newDoc[subtype.NameAsId] != originalDoc[subtype.NameAsId])
						{
							changes[subtype.NameAsId] = newDoc[subtype.NameAsId];
						}
					}
					else
					{
						changes[subtype.NameAsId] = newDoc[subtype.NameAsId];
					}
				}
			}

			//foreach (BsonElement el in newDoc.Elements)
			//{
			//	// Looks like this does a value comparison (not a BsonValue reference comparison), which is good.
			//	// Field exists in original, and value is different...
			//	if ((originalDoc.Contains(el.Name)) && (originalDoc[el.Name] != el.Value))
			//	{
			//		changes[el.Name] = el.Value;
			//	}
			//	// TODO: What about field is only in the new document?
			//	// TODO: What about field is missing in the new document?
			//}


			return changes;
		}

		protected void Delete(string collectionName, string id)
		{
			var collection = db.GetCollection<BsonDocument>(collectionName);
			var filter = new BsonDocument("_id", new ObjectId(id));
			// Or call FindOneAndDelete(filter)
			collection.DeleteOne(filter);
		}

		protected List<string> BuildQueryPipeline(Schema schema, string parentName, List<string> projections, Dictionary<string, int> asList, string schemaNum = "")
		{
			List<string> pipeline = new List<string>();

			schema.ConcreteTypes.ForEach(ct => projections.Add(String.Format("'{0}{2}':'${1}'", ct.Alias, parentName + ct.Name, schemaNum)));

			foreach (Schema subtype in schema.Subtypes)
			{
				if (pipeline.Count > 0)
				{
					pipeline[pipeline.Count - 1] = pipeline.Last() + ",";
				}

				// This handles multiple occurrances of the same subtype, aliasing them so the unwrap and projection references the correct record.
				string asName = subtype.Alias + schemaNum;
				string aliasedAsName = asName;

				if (!asList.ContainsKey(asName))
				{
					asList[asName] = 0;
				}
				else
				{
					int n = asList[asName];
					++n;
					asList[asName] = n;
					aliasedAsName = asName + n;
				}

				pipeline.Add(String.Format("{{$lookup: {{from: '{0}', localField:'{2}{1}', foreignField: '_id', as: '{3}'}} }},", 
					subtype.Name, 
					subtype.Name + "Id", 
					parentName, 
					aliasedAsName));
				pipeline.Add(String.Format("{{$unwind: '${0}'}}", aliasedAsName));
				List<string> subpipeline = BuildQueryPipeline(subtype, aliasedAsName + ".", projections, asList, schemaNum);

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

		protected bool DecrementRefCount(string collectionName, string id, int refCount)
		{
			--refCount;
			var collection = db.GetCollection<BsonDocument>(collectionName);
			var filter = new BsonDocument("_id", new ObjectId(id));
			var update = Builders<BsonDocument>.Update.Set("_ref", refCount);
			collection.UpdateOne(filter, update);

			return refCount == 0;
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

		protected BsonDocument GetRecord(string collectionName, BsonDocument filter)
		{
			BsonDocument record = null;
			List<BsonDocument> docs = db.GetCollection<BsonDocument>(collectionName).Find(filter).ToList();
			// TODO: Assert that docs length is 0 or 1.

			if (docs.Count == 1)
			{
				record = docs[0];
			}

			return record;
		}

		protected int GetRefCount(string collectionName, BsonDocument filter, out string id)
		{
			id = null;
			int refCount = 0;
			BsonDocument record = GetRecord(collectionName, filter);

			if (record != null)
			{
				id = record.Elements.Single(el => el.Name == "_id").Value.ToString();
				refCount = record.Elements.Single(el => el.Name == "_ref").Value.ToInt32();
			}

			return refCount;
		}

		protected int GetRefCount(string collectionName, string id)
		{
			BsonDocument record = GetAll(collectionName, id)[0];
			int refCount = record.Elements.Single(el => el.Name == "_ref").Value.ToInt32();

			return refCount;
		}

		protected string InsertRecord(Schema schema, BsonDocument doc)
		{
			db.GetCollection<BsonDocument>(schema.Name).InsertOne(doc);
			BsonValue bid = doc.Elements.Single(el => el.Name == "_id").Value;
			string id = bid.ToString();

			return id;
		}

		protected BsonDocument GetConcreteObjects(Schema schema, BsonDocument doc)
		{
			BsonDocument newDoc = new BsonDocument();

			// Add the current schema's concrete types to the new JSON object.
			foreach (ConcreteType ct in schema.ConcreteTypes)
			{
				if (doc.Contains(ct.Alias))
				{
					newDoc.Add(ct.Alias, doc[ct.Alias]);
				}
			}

			return newDoc;
		}

		protected BsonDocument RemoveCurrentConcreteObjects(Schema schema, BsonDocument doc)
		{
			BsonDocument newDoc = new BsonDocument(doc);

			foreach (ConcreteType ct in schema.ConcreteTypes)
			{
				newDoc.Remove(ct.Alias);
			}

			return newDoc;
		}

		protected void InsertRecurseIntoSubtypes(Schema schema, BsonDocument currentObject, BsonDocument subdoc)
		{
			foreach (Schema subtype in schema.Subtypes)
			{
				string subtypeId = InternalInsert(subtype, subdoc);
				// TODO: Assert that the subtype name is unique.
				// Insert the object ID's referencing the subtypes

				if (subtypeId != null)
				{
					currentObject.Add(subtype.Name + "Id", new ObjectId(subtypeId));
				}
			}
		}

		protected void DeleteRecurseIntoSubtypes(Schema schema, BsonDocument currentObject, BsonDocument subdoc)
		{
			foreach (Schema subtype in schema.Subtypes)
			{
				string subtypeId = InternalDelete(subtype, subdoc);
				// TODO: Assert that the subtype name is unique.
				// Insert the object ID's referencing the subtypes
				currentObject.Add(subtype.Name + "Id", new ObjectId(subtypeId));
			}
		}

		protected void UpdateRecurseIntoSubtypes(Schema schema, BsonDocument originalFullRecord,
			BsonDocument currenOriginalObject, BsonDocument currentOriginalSubdoc,
			BsonDocument currentNewObject, BsonDocument newSubdoc)
		{
			foreach (Schema subtype in schema.Subtypes)
			{
				// The subtype record id is:
				string originalSubtypeId = originalFullRecord[subtype.Name + "Id"].ToString();
				// Returns either the same subtype ID or the same if no changes were made.
				string newSubtypeId = InternalUpdate(subtype, currentOriginalSubdoc, newSubdoc, originalSubtypeId);
				// TODO: Assert that the subtype name is unique.
				// Insert the object ID's referencing the subtypes
				currentNewObject.Add(subtype.Name + "Id", new ObjectId(newSubtypeId));

				// Here we need to get the FK from the original super-type element to be able to compare against the new FK we just set.
				// If this an "abstract" super-type (no concrete elements) how do we find the record?
			}
		}

		protected void CreateConcreteType(Schema typeDef)
		{
			CreateCollection(typeDef.Name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="schema1"></param>
		/// <param name="schema2"></param>
		/// <param name="assocationTypeName">Association type, like a general purpose "dateAssociation" or a more specific "relation" association (mother, friend, son, etc)</param>
		/// <returns></returns>
		protected Schema CreateAssociationSchema(Schema schema1, Schema schema2)
		{
			string schema1Name = schema1.Name;
			string schema2Name = schema2.Name;

			// If this is an association between the same schema, the tweak the names.
			if (schema1Name == schema2Name)
			{
				schema1Name += "1";
				schema2Name += "2";
			}

			string json = String.Format(@"
			{{
				name: '{0}_{1}', 
				concreteTypes:
				[
					{{name: '{2}Id', type: 'System.String'}},
					{{name: '{3}Id', type: 'System.String'}}
				],
				subTypes:
				[
					{{
						name: '{0}_{1}_Association',
						subtypes:
						[
							{{
								name: 'association',
								subtypes:
								[
									{{
										name: 'forwardAssociationName',
										subtypes:
										[
											{{
												name: 'name',
												alias: 'fwdName',
												concreteTypes:
												[
													{{name: 'name', alias: 'forwardAssociationName', type: 'System.String'}}
												]
											}}
										]
									}},
									{{
										name: 'reverseAssociationName',
										subtypes:
										[
											{{
												name: 'name',
 												alias: 'revName',
												concreteTypes:
												[
													{{name: 'name', alias: 'reverseAssociationName', type: 'System.String'}}
												]
											}}
										]
									}}
								]
							}}
						]
					}}
				]
			}}", schema1.Name, schema2.Name, schema1Name, schema2Name);

			return SchemaFromJson(json);
		}

		protected Schema GetNameSchema()
		{
			string json = @"
			{
				name: 'name',
				concreteTypes:
				[
					{name: 'name', type: 'System.String'}
				]
			}";

			return SchemaFromJson(json);
		}

		protected static Schema SchemaFromJson(string json)
		{
			Schema target = new Schema();
			JsonConvert.PopulateObject(json, target);

			return target;
		}
	}
}
