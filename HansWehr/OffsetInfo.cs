using System;
using System.Collections.Generic;
using System.Linq;

namespace HansWehr
{
	public class OffsetInfo
	{
		public int Column { get; set; }
		public int Phrase { get; set; }
		public int ByteOffset { get; set; }
		public int ByteLength { get; set; }

		public OffsetInfo(IList<string> values)
		{
			if (values == null || values.Count < 4) throw new ArgumentException("String values are not valid.");

			Column = int.Parse(values[0]);
			Phrase = int.Parse(values[1]);
			ByteOffset = int.Parse(values[2]);
			ByteLength = int.Parse(values[3]);
		}

		public static List<OffsetInfo> Parse(string values)
		{
			int i = 0;
			return values.Split(' ')
						 .GroupBy(word => i++ / 4)
						 .Select(word => new OffsetInfo(word.ToList()))
						 .ToList();
	         }
	}
}
