using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.Res;

namespace HansWehr.Droid
{
	public class DatabaseLoader : IDatabaseLoader
	{
		string _fileName = "hanswehr.db";
		string _folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		public DatabaseLoader()
		{

			if (File.Exists(Path.Combine(_folder, _fileName)))
			{
				var databaseAsset = Application.Context.Assets.Open(_fileName);

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
