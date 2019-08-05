using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kampus.Persistence.Migrations.Migrations
{
    [Migration(3)]
    public class Migration003_AddConstantData : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.EmbeddedScript($"{nameof(Migration003_AddConstantData)}.sql");
        }
    }
}
