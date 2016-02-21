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
			Assert.IsTrue(docs[0].ToString().Contains("\"forwardAssociationName\" : \"birthdate\""));
			Assert.IsTrue(docs[0].ToString().Contains("\"reverseAssociationName\" : \"birthdate\""));

			// For debugging:
			// string plan1 = sd.ShowPlan(personSchema);
			// string plan2 = sd.ShowPlan(dateSchema);

			// See note 1 at end of file.
			docs = sd.QueryAssociationServerSide(personSchema, dateSchema);
			Assert.IsTrue(docs.Count == 1);
			Assert.IsTrue(docs[0].ToString() == "{ \"firstName\" : \"Marc\", \"lastName\" : \"Clifton\", \"month\" : 8, \"day\" : 19, \"year\" : 1962, \"fwdAssocName\" : \"birthdate\", \"revAssocName\" : \"birthdate\" }");
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
			Assert.IsTrue(docs[0].ToString().Contains("\"forwardAssociationName\" : \"son\""));
			Assert.IsTrue(docs[0].ToString().Contains("\"reverseAssociationName\" : \"mother\""));

			// See note 2 at end of file.
			docs = sd.QueryAssociationServerSide(personSchema, personSchema);
			Assert.IsTrue(docs.Count == 1);
			Assert.IsTrue(docs[0].ToString() == "{ \"firstName1\" : \"Marc\", \"lastName1\" : \"Clifton\", \"firstName2\" : \"Elisabeth\", \"lastName2\" : \"Clifton\", \"fwdAssocName\" : \"son\", \"revAssocName\" : \"mother\" }");
		}

		[TestMethod]
		public void AssociateAssociationTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema personSchema = Helpers.CreatePersonSchema();
			Schema propertySchema = Helpers.CreatePropertySchema();

			string personId = sd.Insert(personSchema, BsonDocument.Parse("{firstName: 'Marc', lastName: 'Clifton'}"));
			string propertyId = sd.Insert(propertySchema, BsonDocument.Parse("{propertyName: 'Roxbury Rd'}"));

			// Create person-property association record.
			Schema personPropertySchema = sd.Associate(personSchema, propertySchema);
			BsonDocument doc = new BsonDocument("personId", new ObjectId(personId));
			doc.Add("propertyId", new ObjectId(propertyId));
			doc.Add("forwardAssociationName", "purchased");
			doc.Add("reverseAssociationName", "purchased by");
			string personPropertyId = sd.Insert(personPropertySchema, doc);

			// Verify the person-property association.
			List<BsonDocument> docs = sd.QueryAssociationServerSide(personSchema, propertySchema);
			Assert.IsTrue(docs.Count == 1);
			Assert.IsTrue(docs[0].ToString() == "{ \"firstName\" : \"Marc\", \"lastName\" : \"Clifton\", \"propertyName\" : \"Roxbury Rd\", \"fwdAssocName\" : \"purchased\", \"revAssocName\" : \"purchased by\" }");

			Schema dateSchema = Helpers.CreatePureDateSchema();
			string dateId = sd.Insert(dateSchema, BsonDocument.Parse("{month: 12, day: 30, year: 2015}"));

			// Create [person-property association] - date association record.
			Schema personProperty_DateSchema = sd.Associate(personPropertySchema, dateSchema);
			doc = new BsonDocument(personPropertySchema.Name + "Id", new ObjectId(personPropertyId));
			doc.Add(dateSchema.Name + "Id", new ObjectId(dateId));
			doc.Add("forwardAssociationName", "purchased on");
			doc.Add("reverseAssociationName", "purchased on");
			sd.Insert(personProperty_DateSchema, doc);

			// Verify the [person-property association] - date association record.
			docs = sd.QueryAssociationServerSide(personPropertySchema, dateSchema);
			Assert.IsTrue(docs.Count == 1);
			Assert.IsTrue(docs[0].ToString().Contains("\"forwardAssociationName\" : \"purchased\", \"reverseAssociationName\" : \"purchased by\", \"month\" : 12, \"day\" : 30, \"year\" : 2015, \"fwdAssocName\" : \"purchased on\", \"revAssocName\" : \"purchased on\""));
		}
	}
}


// NOTE 1:
// In it's full glory, here's the server-side association query plan:

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
'_id':0} })
*/

// NOTE 2:

/*
db.person.aggregate({$lookup: {from: 'personName', localField:'personNameId', foreignField: '_id', as: 'personName1'} },
{$unwind: '$personName1'},
{$lookup: {from: 'firstName', localField:'personName1.firstNameId', foreignField: '_id', as: 'firstName1'} },
{$unwind: '$firstName1'},
{$lookup: {from: 'name', localField:'firstName1.nameId', foreignField: '_id', as: 'fname1'} },
{$unwind: '$fname1'},
{$lookup: {from: 'lastName', localField:'personName1.lastNameId', foreignField: '_id', as: 'lastName1'} },
{$unwind: '$lastName1'},
{$lookup: {from: 'name', localField:'lastName1.nameId', foreignField: '_id', as: 'lname1'} },
{$unwind: '$lname1'},
{$lookup: {from: 'person_person', localField: '_id', foreignField: 'person1Id', as: 'person_person'} },
{$unwind: '$person_person'},
{$lookup: {from: 'person', localField: 'person_person.person2Id', foreignField: '_id', as: 'person2'} },
{$unwind: '$person2'},
{$lookup: {from: 'person_person_Association', localField: 'person_person.person_person_AssociationId', foreignField: '_id', as: 'specAssoc'} },
{$unwind: '$specAssoc'},
{$lookup: {from: 'association', localField: 'specAssoc.associationId', foreignField: '_id', as: 'assoc'} },
{$unwind: '$assoc'},
{$lookup: {from: 'forwardAssociationName', localField: 'assoc.forwardAssociationNameId', foreignField: '_id', as: 'fan'} },
{$unwind: '$fan'},
{$lookup: {from: 'reverseAssociationName', localField: 'assoc.reverseAssociationNameId', foreignField: '_id', as: 'ran'} },
{$unwind: '$ran'},
{$lookup: {from: 'name', localField: 'fan.nameId', foreignField: '_id', as: 'fanName'} },
{$unwind: '$fanName'},
{$lookup: {from: 'name', localField: 'ran.nameId', foreignField: '_id', as: 'ranName'} },
{$unwind: '$ranName'},
{$lookup: {from: 'personName', localField:'person2.personNameId', foreignField: '_id', as: 'personName2'} },
{$unwind: '$personName2'},
{$lookup: {from: 'firstName', localField:'personName2.firstNameId', foreignField: '_id', as: 'firstName2'} },
{$unwind: '$firstName2'},
{$lookup: {from: 'name', localField:'firstName2.nameId', foreignField: '_id', as: 'fname2'} },
{$unwind: '$fname2'},
{$lookup: {from: 'lastName', localField:'personName2.lastNameId', foreignField: '_id', as: 'lastName2'} },
{$unwind: '$lastName2'},
{$lookup: {from: 'name', localField:'lastName2.nameId', foreignField: '_id', as: 'lname2'} },
{$unwind: '$lname2'},
{$project: {
    'firstName':'$fname1.name',
    'lastName':'$lname1.name',
    'firstName':'$fname2.name',
    'lastName':'$lname2.name',
    'fwdAssocName':'$fanName.name',
    'revAssocName':'$ranName.name', 
    '_id':0} })

 */