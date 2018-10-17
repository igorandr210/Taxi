using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Taxi.Models.Drivers;

namespace Taxi.Services
{
    public class UploadSevice : IUploadService
    {
        private IAmazonS3 _s3;

        private const string bucketName = "taxi-storage-v1";

        public UploadSevice(IAmazonS3 amazonS3)
        {
            _s3 = amazonS3;
        }

        public async Task PutObjectToStorage(string key, string filePath)
        {
            try
            {
                // 1. Put object-specify only key name for the new object.
                var fileTransferUtility =
                    new TransferUtility(_s3);

                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    FilePath = filePath,
                    Key = key
                };
                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                //     PutObjectResponse response1 = await _s3.PutObjectAsync(putRequest1);
                // 2. Put the object-set ContentType and add metadata.
                //var putRequest = new PutObjectRequest
                //{
                //    BucketName = bucketName,
                //    Key = key,
                //    FilePath = filePath,
                //};
                //PutObjectResponse response = await _s3.PutObjectAsync(putRequest);

                //putRequest2.Metadata.Add("x-amz-meta-title", "someTitle");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
        }

        public async Task<bool> DeleteObjectAsync(string keyName)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };
                
                await _s3.DeleteObjectAsync(deleteObjectRequest);
                return true;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);

                return false;
            }
        }

        public async Task<FileDto> GetObjectAsync(string key)
        {
            Stream stream = new MemoryStream();
            string contentType;
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };
                using (GetObjectResponse response = await _s3.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    contentType = response.Headers["Content-Type"];
                    
                    responseStream.CopyTo(stream);

                    //var path = $"C:\\dev\\{key}";
                    //await response.WriteResponseStreamToFileAsync(path,false,new CancellationToken());
                    return new FileDto
                    {
                        Stream = stream,
                        ContentType = contentType
                    };
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                return null;
            }
        }
        
    }
}
