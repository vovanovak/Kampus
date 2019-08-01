using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace Kampus.Persistence.Migrations.Migrations
{
    [Migration(1)]
    public class Migration001_Initial : ForwardOnlyMigration
    {
        public override void Up()
        {
            Console.WriteLine("Created successfully");
        }
    }
}
