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
			Schema personDateAssociation = sd.Associate(personSchema, dateSchema);
		}
	}
}
