using System;
using System.IO;
using Design_Pattern.Interfaces;

namespace Design_Pattern.Services
{
    public class FileManager:IFileManager
    {
        private static FileManager _instance;
        private StreamWriter _writer;
        private StreamReader _reader;
        private FileStream _fileStream;

        private string _filePath = "log.txt";
        private FileManager()
        {
            _fileStream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _reader = new StreamReader(_fileStream);
            _writer = new StreamWriter(_fileStream);
            _writer.AutoFlush = true;
        }

        public static FileManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FileManager();
            }
            return _instance;
        }
        public void Write(string content)
        {
            _fileStream.Seek(0, SeekOrigin.End);
            _writer.WriteLine(content);
        }
        public string Read()
        {
            _fileStream.Seek(0, SeekOrigin.Begin);
            return _reader.ReadToEnd();
        }
        public void Close()
        {
            _writer?.Close();
            _reader?.Close();
            _fileStream?.Close();
        }
    }
}
