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

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;
using UI_Builder;

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
            get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
        }
        public string S3Region = RegionEndpoint.USEast1.SystemName;
        private RegionEndpoint _S3Region
        {
            get { return RegionEndpoint.GetBySystemName(S3Region); }
        }

        public void Init()
        {
            IdentityPoolId = "us-east-1:1d281ad5-139a-45ae-915d-bcd555a2e228";

            UnityInitializer.AttachToGameObject(gameObject);
            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        }

        #region private members

        private IAmazonS3 _s3Client;
        private AWSCredentials _credentials;

        private AWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
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
        public void GetObject(string filename, string S3BucketName)
        {
            Debug.Log("Downloading " + S3BucketName + "/" + filename);

            InitializationManager.DownloadCount++;
            Client.GetObjectAsync(S3BucketName, filename, (responseObj) =>
            {
                var response = responseObj.Response;
                if (response.ResponseStream != null)
                {
                    filename = S3BucketName + "/" + filename;
                    UIB_FileManager.WriteFileFromResponse(response, filename);
                    Directory.SetLastAccessTime(Application.persistentDataPath, DateTime.Now);
                    InitializationManager.DownloadCount--;
                }
            });
        }

        /// <summary>
        /// Get Objects from S3 Bucket
        /// </summary>
        public void GetObjects(string S3BucketName)
        {
            var request = new ListObjectsRequest()
            {
                BucketName = S3BucketName
            };

            Client.ListObjectsAsync(request, (responseObject) =>
            {
                if (responseObject.Exception == null)
                {
                    // Debug.Log("Got Response Printing now ");
                    responseObject.Response.S3Objects.ForEach((o) =>
                    {
                        Debug.Log("key:" + o.Key);
                        //GetObject(o.Key, S3BucketName);
                    });
                }
                else
                {
                    Debug.LogWarning("Got exception" + responseObject.Exception);
                }
            });
        }

        public void CheckIfObjectHasUpdate(string path, string filename, string S3BucketName)
        {
            var request = new GetObjectMetadataRequest()
            {
                BucketName = S3BucketName,
                Key = filename
            };

            DateTime S3LastModified = new DateTime();
            DateTime localFilesLastModified = new DateTime();

            InitializationManager.DownloadCount++;
            InitializationManager.checkingForUpdates++;

            Client.GetObjectMetadataAsync(request, (responseObject) =>
            {
                if (responseObject.Exception == null)
                {
                    S3LastModified = responseObject.Response.LastModified.ToUniversalTime();
                    localFilesLastModified = File.GetLastWriteTimeUtc(path);

//                    Debug.Log("last modified " + S3LastModified + " local changed " + localFilesLastModified);
                    var timeDiff = S3LastModified.CompareTo(localFilesLastModified);

                    //Compare the difference in time between the local directory and files in the cloud
                    if (timeDiff < 0)
                    {
                        InitializationManager.DownloadCount--;
                        InitializationManager.checkingForUpdates--;
                    }
                    else if (timeDiff == 0)
                    {
                        InitializationManager.DownloadCount--;
                        InitializationManager.checkingForUpdates--;
                        Debug.LogWarning("same time - seems wierd if you get here.");
                        GetObject(filename, S3BucketName);
                    }
                    else if (timeDiff > 0)
                    {
                        InitializationManager.DownloadCount--;
                        InitializationManager.checkingForUpdates--;
                        Debug.LogWarning("Downloading from the Cloud " + filename);
                        GetObject(filename, S3BucketName);
                    }

                }
                else {
                    InitializationManager.DownloadCount--;
                    InitializationManager.checkingForUpdates--;
                    Debug.Log(responseObject.Exception);
                }

            });


        }

       

    }
}