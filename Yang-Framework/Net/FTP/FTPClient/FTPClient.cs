using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace Yang_Framework.Net.FTP.FTPClient
{
	public class FTPClient
	{
		public string Address { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		public FTPClient(string Address, string Username, string Password)
		{
			this.Address = Address;
			this.Username = Username;
			this.Password = Password;
		}

		public delegate void ReceivedFileListCompleteEventhandler();
		public event ReceivedFileListCompleteEventhandler ReceivedFileListComplete;

		public void CheckConnection()
		{
			try
			{
				FtpWebRequest.DefaultWebProxy = null;
				FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + this.Address + "/"));
				ftpWebRequest.Credentials = new NetworkCredential(this.Username, this.Password);
				
				//Als Methode muss ListDirectory gewählt werden!
				ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
				
				WebResponse webResponse = ftpWebRequest.GetResponse();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void UploadFile(string remoteFolder, FileInfo fileInfo)
		{
			try
			{
				FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + this.Address + "/" + remoteFolder + "/" + fileInfo.Name));
				
				request.Method = WebRequestMethods.Ftp.UploadFile;
				
				request.Credentials = new NetworkCredential(this.Username, this.Password);
				
				Stream ftpStream = request.GetRequestStream();
				
				FileStream file = File.OpenRead(fileInfo.FullName);
				
				int length = 1024;
				byte[] buffer = new byte[length];
				int bytesRead = 0;
				
				do
				{
					bytesRead = file.Read(buffer, 0, length);
					ftpStream.Write(buffer, 0, bytesRead);
				}
				while (bytesRead != 0);
				
				file.Close();
				ftpStream.Close();
			}
			catch (WebException)
			{
				throw;
			}
		}

		public void UploadFile(FileInfo fileInfo)
		{
			this.UploadFile("", fileInfo);
		}

		public List<string> GetFileList(string remoteFolder)
		{
			FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create("ftp://" + this.Address + "/" + remoteFolder);
			ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
			
			WebResponse webResponse = null;
			
			ftpWebRequest.Credentials = new NetworkCredential(this.Username, this.Password);
			
			try
			{
				webResponse = ftpWebRequest.GetResponse();
			}
			catch (Exception)
			{
				throw;
			}
			
			List<string> files = new List<string>();
			
			StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
			
			while (!streamReader.EndOfStream)
			{
				files.Add(streamReader.ReadLine());
			}
			
			streamReader.Close();
			
			webResponse.Close();

			ReceivedFileListComplete();

			return files;
		}

		public List<string> GetFileList()
		{
			return this.GetFileList("");
		}

		public void DownloadFile(string remoteFolder, FileInfo file, string destinationFolder, FileInfo destinationFile)
		{
			try
			{
				WebClient webClient = new WebClient();
				
				webClient.Credentials = new NetworkCredential(this.Username, this.Password);
				
				byte[] data = webClient.DownloadData(new Uri("ftp://" + this.Address + "/" + remoteFolder + "/" + file.Name));
				
				FileStream fileStream = File.Create(destinationFolder + @"\" + destinationFile);
				
				fileStream.Write(data, 0, data.Length);
				
				fileStream.Close();
			}
			catch (WebException)
			{
				throw;
			}
		}

		public void DownloadFile(FileInfo file, string destinationFolder, FileInfo destinationFile)
		{
			this.DownloadFile("", file, destinationFolder, destinationFile);
		}

		public void DeleteFile(string remoteFolder, FileInfo fileInfo)
		{
			try
			{
				FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + this.Address + "/" + remoteFolder + "/" + fileInfo.Name));
				ftpWebRequest.UseBinary = true;
				ftpWebRequest.Credentials = new NetworkCredential(this.Username, this.Password);
				ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
				ftpWebRequest.Proxy = null;
				ftpWebRequest.KeepAlive = false;
				ftpWebRequest.UsePassive = false;
				ftpWebRequest.GetResponse();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void DeleteFile(FileInfo fileInfo)
		{
			DeleteFile("", fileInfo);
		}

		public void CreateFolder(string remoteFolder, string folder)
		{
			try
			{
				FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + this.Address + "/" + remoteFolder + "/" + folder));
				ftpWebRequest.UseBinary = true;
				ftpWebRequest.Credentials = new NetworkCredential(this.Username, this.Password);
				ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
				ftpWebRequest.Proxy = null;
				ftpWebRequest.KeepAlive = false;
				ftpWebRequest.UsePassive = false;
				ftpWebRequest.GetResponse();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void CreateFolder(string folder)
		{
			this.CreateFolder("", folder);
		}
	}
}
