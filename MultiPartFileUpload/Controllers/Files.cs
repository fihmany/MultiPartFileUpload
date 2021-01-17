using System.Linq;
using MultiPartFileUpload.BL;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MultiPartFileUpload.Entities;
using Microsoft.Extensions.Logging;
using MultiPartFileUpload.Contracts;
using MultiPartFileUpload.Contracts.Outgoing;

namespace MultiPartFileUpload.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Files : ControllerBase
    {
        private readonly MultiPartFileUploadBl _bl;
        private readonly ILogger<Files> _logger;
        private const string ContentLen = "Content-Length";
        private const string UploadOffset = "Upload-Offset";
        private const string IsFinal = "Is-Final";
        private const string MaxSize = "Max-Size";
        private const string UncompressedLen = "Uncompressed-Length";
        private const string UploadLen = "Upload-Length";
        private const string ContentTypeValue = "application/offset+octet-stream";
        private const int MaxSizeValue = 30000;

        public Files(MultiPartFileUploadBl bl, ILogger<Files> logger)
        {
            _bl = bl;
            _logger = logger;
        }

        [HttpPost]
        public CreateUploadContract GenerateNewUpload(UploadMetaData metaData)
        {
            if (!_bl.ValidateInitializationMetaData(metaData))
            {
                return new CreateUploadContract { Location = "BadRequest" };
            }

            Response.Headers.Add(MaxSize, MaxSizeValue.ToString());
            return GenerateReturnLocation(Request.Path.Value, _bl.GenerateUploadProcess(metaData));
        }

        [HttpGet]
        //TODO: I authentication filter
        public string GetCurrentUploads()
        {
            return string.Join
            (
                "\n",
                MultiPartFileUploadBl.UploadsInProgress.Select(pair => $"{pair.Key}={pair.Value}").ToArray()
            ); ;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UploadFile(string id)
        {
            if (!ValidatePutRequest(Request) || !_bl.CheckIfUploadIdActive(id))
            {
                return NotFound();
            }

            UploadRequestHeaders headers = ExtractHeaders(Request.Headers);
            await _bl.UploadPart(id, headers.UploadOffset, headers.UncompressedLength, Request.Body, headers.IsFinal);
            return NoContent();
        }

        private static bool ValidatePutRequest(HttpRequest request)
        {
            IHeaderDictionary headers = request.Headers;
            return request.ContentType == ContentTypeValue &&
                   headers.ContainsKey(ContentLen) &&
                   headers.ContainsKey(UploadOffset) &&
                   headers.ContainsKey(IsFinal) &&
                   headers.ContainsKey(UncompressedLen);
        }

        private static UploadRequestHeaders ExtractHeaders(IHeaderDictionary headers)
        {
            return new UploadRequestHeaders()
            {
                ContentLength = (int) (headers.ContentLength ?? 0),
                IsFinal = headers[IsFinal].ToString() != "0",
                UploadOffset = int.Parse(headers[UploadOffset].ToString()),
                UncompressedLength = int.Parse(headers[UncompressedLen].ToString()),
                UploadLen = headers.ContainsKey(UploadLen) ? int.Parse(headers[UploadLen].ToString()) : (int?)null
            };
        }

        private static CreateUploadContract GenerateReturnLocation(string requestPath, string uploadId)
        {
            return new CreateUploadContract()
            {
                Location = $"{requestPath}/{uploadId}"
            };
        }
    }
}
