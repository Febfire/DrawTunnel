using System.Collections.Generic;
using System.Linq;
using LiteDB;
using EntityStore.Models;

namespace EntityStore.Controller
{
    public delegate void EntityEventHandler(object sender, DBEntity entity);
    public delegate void EntitiesEventHandler(object sender, List<DBEntity> entity);
    /// <summary>
    /// LiteDB数据操作封装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DBEntityControl
    {
        public LiteDatabase DB { get { return _database; } }

        private LiteDatabase _database;

        private LiteCollection<DBEntity> _collection;

        public List<object> Senders { get; set; }
        public string Name{ get;private set;}
        public DBEntityControl(string dbName)
        {
            _database = new LiteDatabase(dbName);
            _collection = _database.GetCollection<DBEntity>("DBEntity");
            Name = dbName;
            Senders = new List<object>();
        }

        
        internal LiteDatabase GetDB
        {
            get
            {
                return _database;
            }
        }

        public event EntityEventHandler EntityInserted;
        public event EntityEventHandler EntityUpdated;
        public event EntitiesEventHandler EntityDeleted;
       
        public void Insert(DBEntity ent,object sender)
        {
            _collection.Insert(ent);
            Senders.Add(sender);
            EntityInserted?.Invoke(this, ent);
            Senders.Clear();
        }

        public void Update(DBEntity ent, object sender)
        {
            _collection.Update(ent);
            Senders.Add(sender);
            EntityUpdated?.Invoke(this, ent);
            Senders.Clear();
        }

        public int Delete(Query query, object sender)
        {
            List<DBEntity> rets = FindMany(query);
            Senders.Add(sender);
            EntityDeleted?.Invoke(this, rets);
            Senders.Clear();
            return _collection.Delete(query);
        }

        public DBEntity FindOne(Query query)
        {
            var ret = _collection.FindOne(query);
            return ret;
        }

        public List<DBEntity> FindMany(Query query)
        {
            try
            {
                var rets = _collection.Find(query).ToList();
                return rets;
            }
            catch (System.Exception )
            {
                return null;
            }
        }     
    }
}
