using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

using Clifton.MongoSemanticDatabase;

namespace MongoDbTests
{
	class Program
	{
		static void Main(string[] args)
		{
			SemanticDatabase sd = new SemanticDatabase();
			sd.Open("newdb");
			List<string> collections = sd.GetCollections();

			IMongoClient client = new MongoClient();
			IMongoDatabase db = client.GetDatabase("newdb");
			var collection = db.GetCollection<BsonDocument>("PersonPhone");
			var aggregate = collection.Aggregate().Lookup("Person", "PersonID", "ID", "Person").Lookup("Phone", "PhoneID", "ID", "Phone");

			// Useful link that helped me figure this out: https://groups.google.com/forum/#!topic/mongodb-user/Otg17LUE_7M
			var pipes = new[] 
			{
				@"{$lookup:
					{
						from: 'Person',
						localField: 'PersonID',
						foreignField: 'ID',
						as: 'PersonName'
					}}",
				@"{$lookup:
				{
					from: 'Phone',
					localField: 'PhoneID',
					foreignField: 'ID',
					as: 'PersonPhone'
				}}",
				@"{ $match: {PersonID: 2} }",
				@"{$project: {'PersonName.LastName':1, 'PersonName.FirstName':1, 'PersonPhone.Number': 1, _id:0}}"
			};

			var pipeline = pipes.Select(s => BsonDocument.Parse(s)).ToList();

			// Option 1:
			// var test = collection.Aggregate<BsonDocument>(pipeline);

			// Option 2:
			var test = collection.Aggregate();
			foreach (string pipe in pipes)
			{
				test = test.AppendStage<BsonDocument>(pipe);
			}


			// var filter = new BsonDocument();
			//var result = collection.Find(filter).ToList();
			// var result = aggregate.ToList();

			var result = test.ToList();

			foreach (var doc in result)
			{
				var elements = doc.Elements;

				foreach (var el in elements)
				{
					Console.WriteLine(el.Name + " = " + el.Value);
				}
			}
		}
	}
}
