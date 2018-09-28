using PIFileProcess.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PIFileProcess {

	public class FileProcess
	{
		public static void Main(string[] args)
		{
			string sourcePath = @"C:\Users\GIZEM\Desktop\Input.csv";   // It can be taken from appconfig or console readline
			string targetPath = @"C:\Users\GIZEM\Desktop\Output.txt";  // It can be taken from appconfig or console readline
			

			if (!File.Exists(@"C:\Users\GIZEM\Desktop\Input.csv")) {
				Console.WriteLine("A file with this name could not be found");
				//Console.ReadKey(true);
				return;  
			} else {
				ReadData(sourcePath, true);
			}

			List<PlayDetail> playDetails = ReadData(sourcePath, true);

			


			if (File.Exists(@"C:\Users\GIZEM\Desktop\Output.txt")) {
				Console.WriteLine("Do not overwrite the existing file with the same name.");
				//Console.ReadKey(true);
			} else {

				DateTime startTime = DateTime.Parse("10/08/2016", CultureInfo.InvariantCulture);
				DateTime endTime = DateTime.Parse("11/08/2016", CultureInfo.InvariantCulture);

				var distinctSongPerClients = from playDetail in playDetails
											 where playDetail.PlayTs >= startTime && playDetail.PlayTs < endTime
											 group playDetail.SongId by playDetail.ClientId
										     into temp
											 select new {
												ClientId = temp.Key,
												DistinctPlayCount = temp.Distinct().Count()
											 };


				var clientsCount = from distinctSongPerClient in distinctSongPerClients
								   orderby distinctSongPerClient.DistinctPlayCount
								   group distinctSongPerClient.ClientId by distinctSongPerClient.DistinctPlayCount
								   into temp2
								   select new PlayCount {  
									  DistinctPlayCount = temp2.Key,
									  ClientCount = temp2.Count()
								   };

				WriteData(targetPath, clientsCount);
			}

			
		}

		private static List<PlayDetail> ReadData(string folderPath, bool isHeader)
		{
			List<PlayDetail> playDetails = new List<PlayDetail>();

			using (StreamReader reader = new StreamReader(folderPath)) {

				while (!reader.EndOfStream) {
					string line = reader.ReadLine();
					if (!isHeader) {
						string[] columnValues = line.Split('\t');

						PlayDetail playDetail = new PlayDetail {
							PlayId = columnValues[0],
							SongId = columnValues[1],
							ClientId = columnValues[2],
							PlayTs = Convert.ToDateTime(columnValues[3])
						};

						playDetails.Add(playDetail);
					} else {
						isHeader = false;
					}
				}
			}

			return playDetails;
		}


		private static void WriteData(string folderPath, IEnumerable<PlayCount> data)	
		{
			using (StreamWriter file = new StreamWriter(folderPath)) {

				file.WriteLine("DISTINCT_PLAY_COUNT" + '\t' + "CLIENT_COUNT");

				foreach (PlayCount item in data) {
					file.WriteLine(item.DistinctPlayCount.ToString() + '\t' + '\t' + '\t' + item.ClientCount.ToString());
				}
			}
		}

	}
}
	

