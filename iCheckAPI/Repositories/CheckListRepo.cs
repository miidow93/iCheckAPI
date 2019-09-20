using iCheckAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCheckAPI.Repositories
{
    public class CheckListRepo: ICheckListRepo
    {
        // private readonly ICheckListContext _context;

        private readonly IMongoCollection<CheckList> _context;

        public CheckListRepo(ISettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _context = database.GetCollection<CheckList>("checkList");
        }

        public async Task<IEnumerable<CheckList>> GetAllCheckList()
        {
            return await _context.Find(_ => true).ToListAsync();
        }

        public Task<CheckList> GetCheckListByDate(DateTime date)
        {
            FilterDefinition<CheckList> filter = Builders<CheckList>.Filter.Eq(d => d.Date, date);
            return _context.Find(filter).FirstOrDefaultAsync();
        }

        public Task<CheckList> GetCheckListByID(string id)
        {
            FilterDefinition<CheckList> filter = Builders<CheckList>.Filter.Eq(d => d.Id, id);
            return _context.Find(filter).FirstOrDefaultAsync();
        }

        public async Task Create(CheckList checkList)
        {
            System.Diagnostics.Debug.WriteLine(checkList.CatchAll);
            // CatchAllSerializer<CheckList>.DeserializeBsonDocument(checkList.CatchAll);
            /*var catchAll = new Dictionary<string, object>();
            for(int i = 0; i < checkList.CatchAll.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine(checkList.CatchAll.ElementAt(i).Key + " " + checkList.CatchAll.ElementAt(i).Value);
                catchAll.Add(checkList.CatchAll.ElementAt(i).Key, checkList.CatchAll.ElementAt(i).Value);
            }
            checkList.CatchAll.Clear();
            checkList.CatchAll.*/
            await _context.InsertOneAsync(checkList);
        }

        public async Task<bool> Update(CheckList checkList)
        {
            ReplaceOneResult updateResult = await _context.ReplaceOneAsync(filter: g => g.Id == checkList.Id, 
                replacement: checkList);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        /*public async Task<bool> Delete(ObjectId id)
        {
            FilterDefinition<CheckList> filter = Builders<CheckList>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await _context.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;

        }*/

        public async Task<bool> Delete(string id)
        {
            FilterDefinition<CheckList> filter = Builders<CheckList>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await _context.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;

        }
    }

    public interface ICheckListRepo
    {
        Task<IEnumerable<CheckList>> GetAllCheckList();
        Task<CheckList> GetCheckListByDate(DateTime date);
        Task<CheckList> GetCheckListByID(string id);
        Task Create(CheckList checkList);
        Task<bool> Update(CheckList checkList);
        // Task<bool> Delete(ObjectId id);
        Task<bool> Delete(string id);
    }
}
