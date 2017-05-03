using Xunit;

namespace Thinkedge.Simple.Table.Test
{
	public class SimpleTableTest
	{
		public static SimpleTable CreateTestData()
		{
			var table = new SimpleTable();

			table.Name = "TestTable";
			table.SetColumnName(0, "ID");
			table.SetColumnName(1, "Data");

			var row = table.CreateRow();
			row.SetField(0, "1");
			row.SetField(1, "ABC");

			row = table.CreateRow();
			row.SetField(0, "2");
			row.SetField(1, "DEF");

			row = table.CreateRow();
			row.SetField(0, "3");
			row.SetField(1, "GHI");

			return table;
		}

		[Fact]
		public void TestName()
		{
			var table = CreateTestData();

			Assert.Equal(table.Name, "TestTable");
		}

		[Fact]
		public void CheckColumnsByIndex()
		{
			var table = CreateTestData();

			Assert.Equal(table[0][0], "1");
			Assert.Equal(table[1][0], "2");
			Assert.Equal(table[2][0], "3");
		}

		[Fact]
		public void CheckColumnsByName()
		{
			var table = CreateTestData();

			Assert.Equal(table[0]["ID"], "1");
			Assert.Equal(table[1]["ID"], "2");
			Assert.Equal(table[2]["ID"], "3");
		}
	}
}