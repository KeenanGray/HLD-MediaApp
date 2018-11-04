/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;

public class MangoPackage : MonoBehaviour {

    internal class Bio {
        string name;
        string title;
        string desc;
    }

    IMongoCollection<BsonDocument> collection;
    IMongoDatabase db;

    private async void Start()
    {
        await StartAsync();
    }

    // Use this for initialization
    async System.Threading.Tasks.Task StartAsync()
    {
        var connectionString = "mongodb://Admin:admin1234@ds151393.mlab.com:51393/heroku_pm1crn83";

        var client = new MongoClient(connectionString);
        db = client.GetDatabase("heroku_pm1crn83");

        collection = db.GetCollection<BsonDocument>("The_Displayed");

        using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(new BsonDocument()))
        {
            while (await cursor.MoveNextAsync())
            {
                IEnumerable<BsonDocument> batch = cursor.Current;
                foreach (BsonDocument document in batch)
                {
                    Bio_Factory.CreateBioPages(document);
                }
            }
        }

    }

	// Update is called once per frame
	void Update () {
		
	}
}
*/