using System;
namespace HansWehr
{
	/// <summary>
	/// Handles the extracting the database embedded into the app, into a file location on the device
	/// </summary>
	public interface IDatabaseLoader
	{
		/// <summary>
		/// Gets the path to the database.
		/// </summary>
		/// <value>The path.</value>
		string FilePath { get; }

	}
}
