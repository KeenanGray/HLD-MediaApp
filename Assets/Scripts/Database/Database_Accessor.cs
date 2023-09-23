//
// Copyright 2014-2015 Amazon.com,

// Inc. or its affiliates. All Rights Reserved.
//

// Licensed under the AWS Mobile SDK For Unity

// Sample Application License Agreement (the "License").

// You may not use this file except in compliance with the

// License. A copy of the License is located

// in the "license" file accompanying this file. This file is

// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR

// CONDITIONS OF ANY KIND, express or implied. See the License

// for the specific language governing permissions and

// limitations under the License.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace HLD
{
    public enum DatabaseResult
    {
        SUCCESS,
        FAILURE,
        NOCONNECTION,
        RETRY
    }

    public class Database_Accessor : MonoBehaviour
    {
        public static string IdentityPoolId = "";

        public string CognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;

        private RegionEndpoint _CognitoIdentityRegion
        {
            get
            {
                return RegionEndpoint.GetBySystemName(CognitoIdentityRegion);
            }
        }

        public string S3Region = RegionEndpoint.USEast1.SystemName;

        private RegionEndpoint _S3Region
        {
            get
            {
                return RegionEndpoint.GetBySystemName(S3Region);
            }
        }

        public static Dictionary<string, int> FallbackCounter;

        #region private members

        private IAmazonS3 _s3Client;

        private AWSCredentials _credentials;

        private AWSCredentials Credentials
        {
            get
            {
                IdentityPoolId = "us-east-1:1d281ad5-139a-45ae-915d-bcd555a2e228";
                if (_credentials == null)
                    _credentials =
                        new CognitoAWSCredentials(IdentityPoolId,
                            _CognitoIdentityRegion);
                return _credentials;
            }
        }

        private IAmazonS3 Client
        {
            get
            {
                if (_s3Client == null)
                {
                    _s3Client = new AmazonS3Client(Credentials, _S3Region);
                }

                //test comment
                return _s3Client;
            }
        }

        #endregion


        /// <summary>
        /// Get Object from S3 Bucket
        /// </summary>
        public async Task GetObject(string filename, string S3BucketName)
        {
            InitializationManager.DownloadCount++;
            Debug.Log("Get Object " + filename);

            Amazon.S3.Model.GetObjectResponse result = await Client.GetObjectAsync(S3BucketName, filename);

            if (result != null)
            {
                filename = S3BucketName + "/" + filename;
                Debug.Log("File Downloaded " + filename);

                UIB_FileManager.WriteFileFromResponse(result, filename);

                Debug.Log("Write Object " + filename);

                Directory.SetLastAccessTime(UIB_PlatformManager.persistentDataPath, DateTime.Now);
                InitializationManager.DownloadCount--;
            }

        }

        /// <summary>
        /// Get Objects from S3 Bucket
        /// </summary>
        public async Task GetObjects(string S3BucketName)
        {
            Debug.Log("GetObjects");


            var result = await Client.ListObjectsAsync(S3BucketName);
            foreach (var obj in result.S3Objects)
            {
                Debug.Log("key:" + obj);
            }


        }

        public async Task CheckIfObjectHasUpdate(string filename, string S3BucketName)
        {
            var request =
                new GetObjectMetadataRequest()
                { BucketName = S3BucketName, Key = filename };


            DateTime S3LastModified = new DateTime();
            DateTime localFilesLastModified = new DateTime();

            InitializationManager.DownloadCount++;
            InitializationManager.checkingForUpdates++;

            var result = await Client.GetObjectMetadataAsync(request);

            S3LastModified =
                      result
                          .LastModified
                          .ToUniversalTime();
            localFilesLastModified =
                DateTime
                    .Parse(PlayerPrefs.GetString("LastUpdated"));

            var timeDiff =
                S3LastModified.CompareTo(localFilesLastModified);

            //Compare the difference in time between the local directory and files in the cloud
            if (timeDiff < 0)
            {
                // Debug.Log("online file is older");
                InitializationManager.DownloadCount--;
                InitializationManager.checkingForUpdates--;
            }
            else if (timeDiff == 0)
            {
                InitializationManager.DownloadCount--;
                InitializationManager.checkingForUpdates--;
                Debug
                    .LogWarning("same time - seems wierd if you get here.");
                UIB_FileManager.HasUpdatedAFile = true;
                await GetObject(filename, S3BucketName);
            }
            else if (timeDiff > 0)
            {
                //Debug.Log("online file is newer");
                InitializationManager.DownloadCount--;
                InitializationManager.checkingForUpdates--;

                // Debug.LogWarning("Downloading from the Cloud " + filename);
                UIB_FileManager.HasUpdatedAFile = true;
                await GetObject(filename, S3BucketName);
                InitializationManager.hasUpdatedFiles = true;
            }

            else
            {
                InitializationManager.DownloadCount--;
                InitializationManager.checkingForUpdates--;
            }

        }











        internal void GetObjectWithFallback(
            string filename,
            string S3BucketName
        )
        {
            throw new NotImplementedException();
            /*            if (FallbackCounter == null)
                        {
                            FallbackCounter = new Dictionary<string, int>();
                        }
                        var count = 0;
                        if (FallbackCounter.ContainsKey(filename))
                        {

                        }
                        else
                            FallbackCounter.Add(filename, count);

                        //Debug.Log("Downloading " + S3BucketName + "/" + filename + " with fallback");

                        InitializationManager.DownloadCount++;
                        Client.GetObjectAsync(S3BucketName, filename, (responseObj) =>
                        {
                            var response = responseObj.Response;
                            FallbackCounter[filename]++;

                            if (response.ResponseStream != null)
                            {
                                filename = S3BucketName + "/" + filename;
                                UIB_FileManager.WriteFileFromResponse(response, filename);
                                Directory.SetLastAccessTime(Application.persistentDataPath, DateTime.Now);
                                InitializationManager.DownloadCount--;
                            }
                            else
                            {

                            }

                            if (FallbackCounter[filename] > 60 * 10)
                            {

                            }
                        });
                        */

        }
    }
}
