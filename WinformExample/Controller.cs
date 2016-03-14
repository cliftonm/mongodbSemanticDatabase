using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using MongoDB.Bson;

using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public class Controller
	{
		protected Model model;

		public Controller(Model model)
		{
			this.model = model;
		}

		public void InstantiateMissingLookups()
		{
			Helpers.Try(() =>
			{
				if (!model.Db.GetCollections().Contains("monthLookup"))
				{
					InstantiateMonthLookup(model.Db, model.GetSchema("monthLookup"));
				}
			});
		}

		protected void InstantiateMonthLookup(SemanticDatabase sd, Schema schema)
		{
			sd.Insert(schema, BsonDocument.Parse("{month: 1, monthName: 'January', monthAbbr: 'Jan'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 2, monthName: 'February', monthAbbr: 'Feb'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 3, monthName: 'March', monthAbbr: 'Mar'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 4, monthName: 'April', monthAbbr: 'Apr'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 5, monthName: 'May', monthAbbr: 'May'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 6, monthName: 'June', monthAbbr: 'Jun'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 7, monthName: 'July', monthAbbr: 'Jul'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 8, monthName: 'August', monthAbbr: 'Aug'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 9, monthName: 'September', monthAbbr: 'Sep'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 10, monthName: 'October', monthAbbr: 'Oct'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 11, monthName: 'November', monthAbbr: 'Nov'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 12, monthName: 'December', monthAbbr: 'Dec'}"));
		}
	}
}
