﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace CardWebHooks
{
    public class DBContext
    {
        public MongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public DBContext()
        {
            Client = new MongoClient("mongodb://tower:27017/admin");
            Database = Client.GetDatabase("Cards");

        }


    }
}