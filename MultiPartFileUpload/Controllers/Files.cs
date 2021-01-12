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
        private const int MaxSizeValue = 30000;

        public Files(MultiPartFileUploadBl bl, ILogger<Files> logger)
        {
            _bl = bl;
            _logger = logger;
        }

        [HttpPost]
        public CreateUploadContract GenerateNewUpload()
        {
            UploadMetaData tmp = new UploadMetaData()
            {
                Compression = "None",
                Aa = "aa",
                Bb = "bb",
                Data = new Dictionary<string, string>()
            };
            Response.Headers.Add(MaxSize, MaxSizeValue.ToString());
            return GenerateReturnLocation(Request.Path.Value, _bl.GenerateUploadProcess(tmp));
        }

        [HttpGet]
        public string GetCurrentUploads()
        {
            return string.Join
            (
                "\n",
                _bl.UploadsInProgress.Select(pair => $"{pair.Key}={pair.Value}").ToArray()
            ); ;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult>UploadFile(string id)
        {
            if (!ValidatePutRequest(Request.Headers) || !_bl.CheckIfUploadIdActive(id))
            {
                return NotFound();
            }

            UploadRequestHeaders headers = ExtractHeaders(Request.Headers);
            await _bl.UploadPart(id, headers.UploadOffset, Request.Body, headers.IsFinal);
            return Ok();
        }

        private bool ValidatePostRequest()
        {
            return true;
        }

        private static bool ValidatePutRequest(IHeaderDictionary headers)
        {
            return headers.ContainsKey(ContentLen) && headers.ContainsKey(UploadOffset) && headers.ContainsKey(IsFinal);
        }

        private static UploadRequestHeaders ExtractHeaders(IHeaderDictionary headers)
        {
            return new UploadRequestHeaders()
            {
                ContentLength = headers.ContentLength ?? 0,
                IsFinal = headers[IsFinal].ToString() != "0",
                UploadOffset = int.Parse(headers[UploadOffset].ToString())
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
