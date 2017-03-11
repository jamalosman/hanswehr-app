using System;
using System.IO;
using Android.Content;
using Android.Content.Res;

namespace HansWehr.Droid
{
	public class DatabaseLoader : IDatabaseLoader
	{
		string _fileName = "hanswehr.db";
		string _folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		public DatabaseLoader(Context context)
		{
			try
			{
				var stream = new FileStream(Path.Combine(_folder, _fileName), FileMode.Open);
			}
			catch (FileNotFoundException)
			{
				var databaseAsset = context.Assets.Open("hanswehr.db");


				using (var destination = new FileStream(Path.Combine(_folder, _fileName), FileMode.Create))
				{
					databaseAsset.CopyTo(destination);
				}
			}



		}

		public string FilePath
		{
			get
			{
				return Path.Combine(_folder, _fileName);
			}
		}
	}
}
