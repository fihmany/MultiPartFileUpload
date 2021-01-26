using System;
using System.IO;
using System.Threading.Tasks;
using MultiPartFileUpload.Config;
using System.Collections.Generic;
using MultiPartFileUpload.Entities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using MultiPartFileUpload.Compressions;
using MultiPartFileUpload.Managers;

namespace MultiPartFileUpload.BL
{
    public class MultiPartFileUploadBl
    {
        public static ConcurrentDictionary<string, UploadProcess> UploadsInProgress = new ConcurrentDictionary<string, UploadProcess>();
        private readonly ILogger<MultiPartFileUploadBl> _logger;
        private readonly CompressionManager _compressionManager;
        private readonly string _basePath;
        private const string DnrExtenstion = "dnr";
        public MultiPartFileUploadBl(ILogger<MultiPartFileUploadBl> logger, IOptions<FileUploadConfig> config, CompressionManager compressionManager)
        {
            _logger = logger;
            _compressionManager = compressionManager;
            _basePath = config.Value.BaseFolderPath;
            //UploadsInProgress = new ConcurrentDictionary<string, UploadProcess>();
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
                UploadedFile = File.Create(filePath),
                Parts = new Dictionary<int, Part>()

            };
            if (!UploadsInProgress.TryAdd(newUpload.Id, newUpload))
            {
                throw new Exception("Error creating new upload");
            }
            return newUpload.Id;
        }

        public async Task<bool> UploadPart(string uploadId, int offset, int uncompressedSize ,Stream body, bool isFinal)
        {
            ICompression compression = _compressionManager.GetCompression(UploadsInProgress[uploadId].Compression);
            Stream decompressedStream = compression.Decompress(body);
            
            FileStream uploadStream = UploadsInProgress[uploadId].UploadedFile;
            if (uploadStream.Seek(offset, SeekOrigin.Begin) == offset)
            {
                await decompressedStream.CopyToAsync(uploadStream);
            }

            AddPart(uploadId, offset, uncompressedSize, isFinal);

            if (!isFinal)
            {
                return true;
            }

            uploadStream.Close();
            UploadsInProgress[uploadId].Finished = true;

            if (!ValidateUploadFinished(UploadsInProgress[uploadId].Parts))
            {
                throw new Exception("Upload is not complete");
            }

            return true;
        }

        public bool CheckIfUploadIdActive(string id)
        {
            return UploadsInProgress.ContainsKey(id) && !UploadsInProgress[id].Finished;
        }

        public bool ValidateInitializationMetaData(UploadMetaData metaData)
        {
            //TODO: validate metadata
            return true;
        }

        private static void AddPart(string uploadId, int offset, int decompressedSize, bool isFinal)
        {
            UploadsInProgress[uploadId].Parts.Add(offset, new Part {IsFinal = isFinal, Size = decompressedSize, Offset = offset});
        }

        private static bool ValidateUploadFinished(IReadOnlyDictionary<int, Part> parts)
        {
            foreach ((int offset, Part part) in parts)
            {
                if (!part.IsFinal && !parts.ContainsKey(offset + part.Size))
                {
                    return false;
                }
            }

            return true;
        }

        private void CreateUploadPathIfNotExists()
        {
            Directory.CreateDirectory(_basePath);
        }

        private static string GenerateId()
        {
            string id;
            do
            {
                id = Guid.NewGuid().ToString();
            } while (UploadsInProgress.ContainsKey(id));

            return id;
        }
    };
}
