using System.Collections.Generic;
using System.Linq;
using LiteDB;



namespace EntityStore
{
    /// <summary>
    /// LiteDB数据操作简单封装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbControl<T>
    {

        private LiteDatabase Db;

        private LiteCollection<T> Collection;

        public LiteDatabase GetDb { get { return Db; } }

        public LiteCollection<T> GetCollection { get { return Collection; } }
        public DbControl(string dbName, string colName)
        {
            Db = new LiteDatabase(dbName);
            Collection = Db.GetCollection<T>(colName);
            
        }

        public void Insert(T ent)
        {
             Collection.Insert(ent);       
        }


        public int Delete(Query query)
        {

            return Collection.Delete(query);           
        }

        public T FindOne(Query query)
        {
            return Collection.FindOne(query);
        }

        public List<T> FindMany(Query query)
        {

            return Collection.Find(query).ToList();

        }

    }
}
