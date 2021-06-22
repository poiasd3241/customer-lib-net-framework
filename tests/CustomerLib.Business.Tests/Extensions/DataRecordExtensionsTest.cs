using System;
using System.Data;
using CustomerLib.Business.Extensions;
using Moq;
using Xunit;

namespace CustomerLib.Business.Tests.Extensions
{
	public class DataRecordExtensionsTest
	{
		private class GetValueOrDefaultWhenDBNullData : TheoryData<object, string>
		{
			public GetValueOrDefaultWhenDBNullData()
			{
				Add("text", "text");
				Add(DBNull.Value, null);
			}
		}

		[Theory]
		[ClassData(typeof(GetValueOrDefaultWhenDBNullData))]
		public void ShouldGetValueOrDefaultWhenDBNull(object value, string expected)
		{
			// Given
			var columnName = "whatever";
			var record = new Mock<IDataRecord>();
			record.Setup(dataRecord => dataRecord[columnName]).Returns(value);

			// When
			var actual = record.Object.GetValueOrDefault<string>(columnName);

			// Then
			Assert.Equal(expected, actual);
		}
	}
}
