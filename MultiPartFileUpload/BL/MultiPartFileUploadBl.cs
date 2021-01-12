using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MultiPartFileUpload.Config;
using MultiPartFileUpload.Entities;

namespace MultiPartFileUpload.BL
{
    public class MultiPartFileUploadBl
    {
        public ConcurrentDictionary<string, UploadProcess> UploadsInProgress;
        private readonly ILogger<MultiPartFileUploadBl> _logger;
        private string _basePath;
        private const string DnrExtenstion = "dnr";
        public MultiPartFileUploadBl(ILogger<MultiPartFileUploadBl> logger, IOptions<FileUploadConfig> config)
        {
            _logger = logger;
            _basePath = config.Value.BaseFolderPath;
            UploadsInProgress = new ConcurrentDictionary<string, UploadProcess>();
            CreateUploadPathIfNotExists();
        }

        public string GenerateUploadProcess(UploadMetaData uploadMetaData)
        {
            //TODO send metadata to correct place
            //TODO Check if we support the compression
            string uploadId = GenerateId();
            string filePath = Path.Combine(_basePath, $"{uploadId}.{DnrExtenstion}");
            UploadProcess newUpload = new UploadProcess()
            {
                Id = uploadId,
                Compression = uploadMetaData.Compression,
                Finished = false,
                UploadedFile = File.Create(filePath)

            };
            if (!UploadsInProgress.TryAdd(newUpload.Id, newUpload))
            {
                throw new Exception("Error creating new upload");
            }
            return newUpload.Id;
        }

        public async Task UploadPart(string uploadId, int offset, Stream body, bool IsFinal)
        {
            FileStream uploadStream = UploadsInProgress[uploadId].UploadedFile;
            if (uploadStream.Seek(offset, SeekOrigin.Begin) == offset)
            {
                await body.CopyToAsync(uploadStream);
            }

            if (IsFinal)
            {
                uploadStream.Close();
                UploadsInProgress[uploadId].Finished = true;
            }

        }

        public bool CheckIfUploadIdActive(string id)
        {
            return UploadsInProgress.ContainsKey(id) && !UploadsInProgress[id].Finished;
        }

        private void CreateUploadPathIfNotExists()
        {
            Directory.CreateDirectory(_basePath);
        }

        private string GenerateId()
        {
            string id;
            do
            {
                id = Guid.NewGuid().ToString();
            } while (UploadsInProgress.ContainsKey(id));

            return id;
        }
    };

    public class UploadProcess
    {
        public string Id;
        public int Size;
        public bool Finished;
        public string Compression;
        public FileStream UploadedFile;
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj is UploadProcess other && Equals(other);
        }

        protected bool Equals(UploadProcess other)
        {
            return Id == other.Id && Size == other.Size && Finished == other.Finished;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Size, Finished);
        }

        public override string ToString()
        {
            return $"{this.Id} {this.Compression} {this.Finished} {this.Size}";
        }
    }

}
