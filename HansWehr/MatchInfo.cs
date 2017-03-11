using System;
using System.Collections.Generic;

namespace HansWehr
{
	public class MatchInfo
	{
		
		private static int _phraseCountPosition = 0;
		private static int _columnCountPosition = 4;
		private static int _rowCountPosition = 8;
		private static int _averageTokenCountsPosition = 12;
		private int TokenCountsPosition { get { return _averageTokenCountsPosition + (ColumnCount*4); } }
		private int PhraseDatasPosition { get { return TokenCountsPosition + (ColumnCount*4); } }

		/// <summary>
		/// Gets or sets the phrase count.
		/// </summary>
		/// <value>The number of phrases in the query.</value>
		public int PhraseCount { get; set; }

		/// <summary>
		/// Gets or sets the column count.
		/// </summary>
		/// <value>The number of columns in the fts table this result belongs to.</value>
		public int ColumnCount { get; set; }

		/// <summary>
		/// Gets or sets the row count.
		/// </summary>
		/// <value>The number of rows returned by the query.</value>
		public int RowCount { get; set; }

		/// <summary>
		/// Gets or sets the average token counts.
		/// </summary>
		/// <value>The average token count of each column in the FTS table</value>
		public int[] AverageTokenCounts { get; set; }

		/// <summary>
		/// Gets or sets the token counts.
		/// </summary>
		/// <value>The token count for each column the current row in the FTS table.</value>
		public int[] TokenCounts { get; set; }

		/// <summary>
		/// Gets or sets the phrase datas.
		/// </summary>
		/// <value>Details on occurances of each phrase in respect to this row</value>
		public PhraseData[] PhraseDatas { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:HansWehr.MatchInfo"/> class.
		/// </summary>
		/// <param name="rawBytes">The raw blob from the sqlite function matchInfo(tableName,'pcnalx') as a byte array</param>
		public MatchInfo(byte[] rawBytes)
		{
			// there should be a minimum of 32 bytes in a valid matchinfo('pcnalx')
			if (rawBytes == null || rawBytes.Length < 32) throw new ArgumentException("The byte array length is incorrect");

			//try
			//{


			PhraseCount = BitConverter.ToInt32(rawBytes, _phraseCountPosition);
			ColumnCount = BitConverter.ToInt32(rawBytes, _columnCountPosition);
			RowCount = BitConverter.ToInt32(rawBytes, _rowCountPosition);
			AverageTokenCounts = new int[ColumnCount];
			TokenCounts = new int[ColumnCount];
			PhraseDatas = new PhraseData[PhraseCount];

			for (int i = _averageTokenCountsPosition; i < rawBytes.Length; i += 4)
			{
				int currentInt = BitConverter.ToInt32(rawBytes, i);
				//int byteNumber = i / 4;

				if (i < TokenCountsPosition)
				{
					int row = (i - _averageTokenCountsPosition) / 4;
					AverageTokenCounts[row] = currentInt;
				}
				else if (i < PhraseDatasPosition)
				{
					int row = (i - TokenCountsPosition) / 4;
					TokenCounts[row] = currentInt;
				}
				else
				{
					int position = (i - PhraseDatasPosition) / 4;
					int pRow = position / (ColumnCount * 3);
					int cRow = position % ColumnCount;
					if (PhraseDatas[pRow] == null)
						PhraseDatas[pRow] = new PhraseData { ColumnDatas = new PhraseColumnData[ColumnCount] };

					PhraseDatas[pRow].ColumnDatas[cRow] = new PhraseColumnData
					{
						RowMatchCount = currentInt,
						MatchCount = BitConverter.ToInt32(rawBytes, i + 4),
						RowCount = BitConverter.ToInt32(rawBytes, i + 8)
					};

					i += 8;
				}
			}
			//}
			//catch (IndexOutOfRangeException e)
			//{
			//	throw new ArgumentException("The byte array length is incorrect", e);
			//}


		}
	}

	public class PhraseData
	{
		/// <summary>
		/// Gets or sets the column datas.
		/// </summary>
		/// <value>Details on the phrase occurances for each column.</value>
		public PhraseColumnData[] ColumnDatas { get; set; }
	}

	public class PhraseColumnData
	{
		/// <summary>
		/// Gets or sets the match count.
		/// </summary>
		/// <value>The number of times the phrase appears in the column for the current row.</value>
		public int RowMatchCount { get; set; }

		/// <summary>
		/// Gets or sets the match count.
		/// </summary>
		/// <value>The total number of times the phrase appears in the column in all rows in the FTS table.</value>
		public int MatchCount { get; set; }

		/// <summary>
		/// Gets or sets the row count.
		/// </summary>
		/// <value>The total number of rows in the FTS table for which the column contains at least one instance of the phrase.</value>
		public int RowCount { get; set; }
	}
}
