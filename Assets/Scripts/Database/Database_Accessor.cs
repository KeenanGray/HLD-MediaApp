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
        public static string identityPoolId = "us-east-1:1d281ad5-139a-45ae-915d-bcd555a2e228";
        public string cognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;
        public string S3Region = RegionEndpoint.USEast1.SystemName;

        private RegionEndpoint _CognitoIdentityRegion
        {
            get { return RegionEndpoint.GetBySystemName(cognitoIdentityRegion); }
        }
        private RegionEndpoint _S3Region
        {
            get { return RegionEndpoint.GetBySystemName(S3Region); }
        }

        #region private members

        private CognitoAWSCredentials _credentials;

        private CognitoAWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                {
                    _credentials = new CognitoAWSCredentials(
                        "253927263007", // Account number
                        identityPoolId, // Identity pool ID
                        "arn:aws:iam::253927263007:role/Cognito_HeidiLatskyDanceUnauth_User", // Role for unauthenticated users
                        null, // Role for authenticated users, not set
                        RegionEndpoint.USEast1
                    );
                }

                return _credentials;
            }
        }

        private IAmazonS3 _client;

        private IAmazonS3 Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new AmazonS3Client(Credentials, _S3Region);
                }

                //test comment
                return _client;
            }
        }

        #endregion

        /// <summary>
        /// Get Object from S3 Bucket
        /// </summary>
        public async Task GetObject(string filename, string S3BucketName)
        {
            Amazon.S3.Model.GetObjectResponse result = await Client.GetObjectAsync(
                S3BucketName,
                filename
            );

            if (result != null)
            {
                filename = S3BucketName + "/" + filename;
                var file_manager = GameObject.FindObjectsOfType<UIB_FileManager>()[0];
                await file_manager.WriteFileFromResponse(result, filename);
                Directory.SetLastAccessTime(UIB_PlatformManager.persistentDataPath, DateTime.Now);
            }
        }

        /// <summary>
        /// Get Objects from S3 Bucket
        /// </summary>
        public async Task GetObjects(string S3BucketName)
        {
            var result = await Client.ListObjectsAsync(S3BucketName);
            foreach (var obj in result.S3Objects)
            {
            }
        }

        public async Task CheckIfObjectHasUpdate(string filename, string S3BucketName)
        {
            var request = new GetObjectMetadataRequest()
            {
                BucketName = S3BucketName,
                Key = filename
            };

            DateTime S3LastModified = new DateTime();
            DateTime localFilesLastModified = new DateTime();

            InitializationManager.checkingForUpdates++;

            var result = await Client.GetObjectMetadataAsync(request);

            S3LastModified = result.LastModified.ToUniversalTime();
            localFilesLastModified = DateTime.Parse(PlayerPrefs.GetString("LastUpdated"));

            var timeDiff = S3LastModified.CompareTo(localFilesLastModified);

            //Compare the difference in time between the local directory and files in the cloud
            if (timeDiff < 0)
            {
                InitializationManager.DownloadCount--;
            }
            else if (timeDiff == 0)
            {
                UIB_FileManager.HasUpdatedAFile = true;

                await GetObject(filename, S3BucketName);
                InitializationManager.DownloadCount--;
            }
            else if (timeDiff > 0)
            {
                UIB_FileManager.HasUpdatedAFile = true;
                await GetObject(filename, S3BucketName);
                InitializationManager.hasUpdatedFiles = true;
                InitializationManager.DownloadCount--;
            }
        }
    }
}
