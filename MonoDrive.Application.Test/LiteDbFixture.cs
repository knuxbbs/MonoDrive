using System;
using LiteDB;
using MonoDrive.Application.Data;

namespace MonoDrive.Application.Test
{
    public class LiteDbFixture : IDisposable
    {
        public LiteDbFixture()
        {
            Database = new LiteDatabase(LiteDbHelper.GetFilePath(@"MonoDrive.db"));
        }
        
        public LiteDatabase Database { get; }
        
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}