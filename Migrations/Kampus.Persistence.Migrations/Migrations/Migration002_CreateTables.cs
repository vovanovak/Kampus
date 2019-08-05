using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kampus.Persistence.Migrations.Migrations
{
    [Migration(2)]
    public class Migration002_CreateTables : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.EmbeddedScript($"{nameof(Migration002_CreateTables)}.sql");
        }
    }
}
