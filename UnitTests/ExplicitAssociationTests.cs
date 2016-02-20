using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Bson;
using MongoDB.Driver;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	[TestClass]
	public class ExplicitAssociationTests
	{
		[TestMethod]
		public void PersonDateAssociationTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema dateSchema = Helpers.CreatePureDateSchema();
			Schema personSchema = Helpers.CreatePersonSchema();
			Schema personDateSchema = sd.Associate(personSchema, dateSchema);

			string personId = sd.Insert(personSchema, BsonDocument.Parse("{firstName: 'Marc', lastName: 'Clifton'}"));
			string dateId = sd.Insert(dateSchema, BsonDocument.Parse("{month: 8, day: 19, year: 1962}"));
			
			BsonDocument doc = new BsonDocument("personId", new ObjectId(personId));
			doc.Add("dateId", new ObjectId(dateId));
			doc.Add("forwardAssociationName", "birthdate");
			doc.Add("reverseAssociationName", "birthdate");
			sd.Insert(personDateSchema, doc);

			List<BsonDocument> docs = sd.Query(personDateSchema);
			Assert.IsTrue(docs.Count == 1);

			string plan1 = sd.ShowPlan(personSchema);
			string plan2 = sd.ShowPlan(dateSchema);

			docs = sd.QueryAssociationServerSide(personSchema, dateSchema);
			Assert.IsTrue(docs.Count == 1);
			Assert.IsTrue(docs[0].ToString() == "{ \"firstName\" : \"Marc\", \"lastName\" : \"Clifton\", \"month\" : 8, \"day\" : 19, \"year\" : 1962, \"fwdAssocName\" : \"birthdate\", \"revAssocName\" : \"birthdate\" }");

			// In it's full glory, here's the server-side association query:
			/*
db.person.aggregate({$lookup: {from: 'personName', localField:'personNameId', foreignField: '_id', as: 'personName'} },
{$unwind: '$personName'},
{$lookup: {from: 'firstName', localField:'personName.firstNameId', foreignField: '_id', as: 'firstName'} },
{$unwind: '$firstName'},
{$lookup: {from: 'name', localField:'firstName.nameId', foreignField: '_id', as: 'fname'} },
{$unwind: '$fname'},
{$lookup: {from: 'lastName', localField:'personName.lastNameId', foreignField: '_id', as: 'lastName'} },
{$unwind: '$lastName'},
{$lookup: {from: 'name', localField:'lastName.nameId', foreignField: '_id', as: 'lname'} },
{$unwind: '$lname'},
// bind with person_date association
{$lookup: {from: 'person_date', localField: '_id', foreignField: 'personId', as: 'pd'} },
{$unwind: '$pd'},
{$lookup: {from: 'date', localField: 'pd.dateId', foreignField: '_id', as: 'date'} },
{$unwind: '$date'},
// get the date part
{$lookup: {from: 'month', localField: 'date.monthId', foreignField: '_id', as: 'month'} },
{$unwind: '$month'},
{$lookup: {from: 'day', localField: 'date.dayId', foreignField: '_id', as: 'day'} },
{$unwind: '$day'},
{$lookup: {from: 'year', localField: 'date.yearId', foreignField: '_id', as: 'year'} },
{$unwind: '$year'},
// get the forward and reverse names
{$lookup: {from: 'person_date_Association', localField: 'pd.person_date_AssociationId', foreignField: '_id', as: 'pdassoc'} },
{$unwind: '$pdassoc'},
{$lookup: {from: 'association', localField: 'pdassoc.associationId', foreignField: '_id', as: 'assoc'} },
{$unwind: '$assoc'},
{$lookup: {from: 'forwardAssociationName', localField: 'assoc.forwardAssociationNameId', foreignField: '_id', as: 'fan'} },
{$unwind: '$fan'},
{$lookup: {from: 'reverseAssociationName', localField: 'assoc.reverseAssociationNameId', foreignField: '_id', as: 'ran'} },
{$unwind: '$ran'},
{$lookup: {from: 'name', localField: 'fan.nameId', foreignField: '_id', as: 'fanName'} },
{$unwind: '$fanName'},
{$lookup: {from: 'name', localField: 'ran.nameId', foreignField: '_id', as: 'ranName'} },
{$unwind: '$ranName'},
// full projection:
{$project: {
    'firstName':'$fname.name',
    'lastName':'$lname.name', 
    'month':'$month.value',
    'day':'$day.value',
    'year':'$year.value', 
    'fwdAssocName':'$fanName.name',
    'revAssocName':'$ranName.name',
    '_id':0} }
)
			*/
		}

		[TestMethod]
		public void PersonPersonAssociationTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema personSchema = Helpers.CreatePersonSchema();
			Schema personPersonSchema = sd.Associate(personSchema, personSchema);

			string personId1 = sd.Insert(personSchema, BsonDocument.Parse("{firstName: 'Marc', lastName: 'Clifton'}"));
			string personId2 = sd.Insert(personSchema, BsonDocument.Parse("{firstName: 'Elisabeth', lastName: 'Clifton'}"));

			BsonDocument doc = new BsonDocument("person1Id", new ObjectId(personId1));
			doc.Add("person2Id", new ObjectId(personId2));
			doc.Add("forwardAssociationName", "son");			// Marc is the son of Elisabeth
			doc.Add("reverseAssociationName", "mother");		// Elisabeth is the mother of Marc
			sd.Insert(personPersonSchema, doc);

			List<BsonDocument> docs = sd.Query(personPersonSchema);
			Assert.IsTrue(docs.Count == 1);
		}
	}
}
