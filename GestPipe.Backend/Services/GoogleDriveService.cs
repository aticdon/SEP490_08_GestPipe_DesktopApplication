using GestPipe.Backend.Models.DTOs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GestPipe.Backend.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _drive;
        private readonly string _pythonBasePath;
        private readonly string _rootFolderId;
        private readonly string _uploadRootFolderId;

        private static readonly ConcurrentDictionary<string, DriveSyncProgress> _progressDict
    = new ConcurrentDictionary<string, DriveSyncProgress>();
        private static readonly ConcurrentDictionary<string, DriveUploadProgress> _uploadProgressDict
    = new ConcurrentDictionary<string, DriveUploadProgress>();

        public GoogleDriveService(IConfiguration config)
        {
            _pythonBasePath = config["GoogleDrive:PythonBasePath"];
            _rootFolderId = config["GoogleDrive:RootFolderId"];
            _uploadRootFolderId = config["GoogleDrive:UploadRootFolderId"];

            var clientId = config["GoogleDrive:ClientId"];
            var clientSecret = config["GoogleDrive:ClientSecret"];
            var refreshToken = config["GoogleDrive:RefreshToken"];

            if (string.IsNullOrWhiteSpace(_pythonBasePath))
                throw new InvalidOperationException("GoogleDrive:PythonBasePath is not configured.");
            if (string.IsNullOrWhiteSpace(_rootFolderId))
                throw new InvalidOperationException("GoogleDrive:RootFolderId is not configured.");
            if (string.IsNullOrWhiteSpace(_uploadRootFolderId))
                throw new InvalidOperationException("GoogleDrive:UploadRootFolderId is not configured.");
            if (string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(clientSecret) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new InvalidOperationException("GoogleDrive OAuth2 config (ClientId/ClientSecret/RefreshToken) is missing.");
            }

            var token = new TokenResponse
            {
                RefreshToken = refreshToken
                // AccessToken sẽ được Google client tự refresh khi cần
            };

            var flow = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    },
                    Scopes = new[] { DriveService.Scope.Drive }
                });

            var credential = new UserCredential(flow, "gestpipe-owner", token);

            _drive = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = config["GoogleDrive:ApplicationName"]
            });
        }

        public DriveSyncProgress GetProgress(string userId)
        {
            if (_progressDict.TryGetValue(userId, out var p))
                return p;

            return new DriveSyncProgress
            {
                TotalFiles = 0,
                SyncedFiles = 0,
                IsCompleted = false
            };
        }

        // ⭐ NEW: tăng số file đã sync
        private void IncreaseSyncedFiles(string userId)
        {
            if (_progressDict.TryGetValue(userId, out var p))
            {
                p.SyncedFiles++;
            }
        }

        public DriveUploadProgress GetUploadProgress(string userId)
        {
            if (_uploadProgressDict.TryGetValue(userId, out var p))
                return p;

            return new DriveUploadProgress
            {
                TotalFiles = 0,
                UploadedFiles = 0,
                IsCompleted = false
            };
        }

        private void IncreaseUploadedFiles(string userId)
        {
            if (_uploadProgressDict.TryGetValue(userId, out var p))
            {
                p.UploadedFiles++;
            }
        }
        public async Task SyncUserFolderAsync(string userId)
        {
            string folderName = $"user_{userId}";

            // ⭐ NEW: khởi tạo progress
            var progress = new DriveSyncProgress
            {
                TotalFiles = 0,
                SyncedFiles = 0,
                IsCompleted = false
            };
            _progressDict[userId] = progress;

            var listReq = _drive.Files.List();
            listReq.Q =
                "mimeType = 'application/vnd.google-apps.folder' " +
                $"and name = '{folderName}' " +
                $"and '{_rootFolderId}' in parents " +
                "and trashed = false";
            listReq.Fields = "files(id, name)";
            var folderResult = await listReq.ExecuteAsync();

            var folder = folderResult.Files.FirstOrDefault();
            if (folder == null)
            {
                Console.WriteLine($"[SyncUser] Folder '{folderName}' not found in root '{_rootFolderId}'.");
                progress.IsCompleted = true;
                return;
            }

            string localUserDir = Path.Combine(_pythonBasePath, folderName);
            Directory.CreateDirectory(localUserDir);

            // Đếm tổng số file (không tính folder)
            int totalFiles = await CountFilesInDriveFolderAsync(folder.Id);
            progress.TotalFiles = totalFiles;   // ⭐ NEW: ghi tổng số file

            // Tải file về local
            int synced = await SyncDriveFolderToLocalAsync(folder.Id, localUserDir, userId);

            progress.SyncedFiles = synced;
            progress.IsCompleted = true;

            Console.WriteLine($"[SyncUser] Downloaded {synced}/{totalFiles} files to '{localUserDir}'.");
        }


        /// <summary>
        /// Đệ quy đếm số file (không bao gồm folder) trong 1 folder trên Drive.
        /// </summary>
        private async Task<int> CountFilesInDriveFolderAsync(string driveFolderId)
        {
            int total = 0;

            var listReq = _drive.Files.List();
            listReq.Q = $"'{driveFolderId}' in parents and trashed = false";
            listReq.Fields = "files(id, name, mimeType)";
            var filesResult = await listReq.ExecuteAsync();

            foreach (var file in filesResult.Files)
            {
                if (file.MimeType == "application/vnd.google-apps.folder")
                {
                    total += await CountFilesInDriveFolderAsync(file.Id);
                }
                else
                {
                    total++;
                }
            }

            return total;
        }
        private async Task<int> SyncDriveFolderToLocalAsync(
    string driveFolderId,
    string localFolderPath,
    string userId)
        {
            int count = 0;
            Directory.CreateDirectory(localFolderPath);

            var fileListReq = _drive.Files.List();
            fileListReq.Q = $"'{driveFolderId}' in parents and trashed = false";
            fileListReq.Fields = "files(id, name, mimeType)";
            var filesResult = await fileListReq.ExecuteAsync();

            foreach (var file in filesResult.Files)
            {
                if (file.MimeType == "application/vnd.google-apps.folder")
                {
                    var subLocalPath = Path.Combine(localFolderPath, file.Name);
                    count += await SyncDriveFolderToLocalAsync(file.Id, subLocalPath, userId);
                }
                else
                {
                    var getReq = _drive.Files.Get(file.Id);
                    string localPath = Path.Combine(localFolderPath, file.Name);

                    using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write);
                    await getReq.DownloadAsync(fs);
                    count++;

                    // ⭐ NEW: báo đã sync thêm 1 file
                    IncreaseSyncedFiles(userId);

                    Console.WriteLine($"[SyncLocal] Downloaded file: {localPath}");
                }
            }

            return count;
        }

        // ===================== UPLOAD (ĐẨY LÊN) =====================

        /// <summary>
        /// Upload folder user_{userId} từ local lên Drive (bỏ qua file .py),
        /// trả về tổng số file local & số file upload/update thành công.
        /// </summary>
        public async Task UploadUserFolderAsync(string userId)
        {
            string folderName = $"user_{userId}";
            string localUserDir = Path.Combine(_pythonBasePath, folderName);

            Console.WriteLine($"[UploadUser] Local dir: {localUserDir}");

            // ⭐ Khởi tạo progress
            var progress = new DriveUploadProgress
            {
                TotalFiles = 0,
                UploadedFiles = 0,
                IsCompleted = false
            };
            _uploadProgressDict[userId] = progress;

            if (!Directory.Exists(localUserDir))
            {
                Console.WriteLine("[UploadUser] Local dir NOT FOUND");
                progress.IsCompleted = true;
                return;
            }

            int totalFiles = CountLocalFiles(localUserDir);
            progress.TotalFiles = totalFiles; // ⭐ Ghi tổng số file

            string driveUserFolderId = await EnsureUserUploadFolderAsync(folderName);
            Console.WriteLine($"[UploadUser] Drive user folder id = {driveUserFolderId}");

            int uploaded = await UploadLocalFolderToDriveAsync(localUserDir, driveUserFolderId, userId);

            progress.UploadedFiles = uploaded;
            progress.IsCompleted = true;

            Console.WriteLine($"[UploadUser] Uploaded {uploaded}/{totalFiles} files to Drive.");
        }


        /// <summary>
        /// Đếm số file trong thư mục local (bao gồm sub-folder), bỏ qua .py.
        /// </summary>
        private int CountLocalFiles(string localFolderPath)
        {
            int count = 0;
            foreach (var file in Directory.GetFiles(localFolderPath, "*", SearchOption.AllDirectories))
            {
                string ext = Path.GetExtension(file);
                if (ext.Equals(".py", StringComparison.OrdinalIgnoreCase))
                    continue;

                count++;
            }
            return count;
        }

        private async Task<string> EnsureUserUploadFolderAsync(string folderName)
        {
            var listReq = _drive.Files.List();
            listReq.Q =
                "mimeType = 'application/vnd.google-apps.folder' " +
                $"and name = '{folderName}' " +
                $"and '{_uploadRootFolderId}' in parents " +
                "and trashed = false";
            listReq.Fields = "files(id, name)";

            var result = await listReq.ExecuteAsync();
            var folder = result.Files.FirstOrDefault();
            if (folder != null)
                return folder.Id;

            var meta = new Google.Apis.Drive.v3.Data.File
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new[] { _uploadRootFolderId }
            };

            var createReq = _drive.Files.Create(meta);
            createReq.Fields = "id";
            var created = await createReq.ExecuteAsync();
            return created.Id;
        }

        private async Task<int> UploadLocalFolderToDriveAsync(
            string localFolderPath,
            string driveParentId,
            string userId)
        {
            int count = 0;

            Console.WriteLine($"[UploadLocal] Folder: {localFolderPath}");

            // 1. Upload sub-folder
            foreach (var dir in Directory.GetDirectories(localFolderPath))
            {
                string folderName = Path.GetFileName(dir);
                Console.WriteLine($"[UploadLocal]  └─ Subdir: {folderName}");

                var listReq = _drive.Files.List();
                listReq.Q =
                    "mimeType = 'application/vnd.google-apps.folder' " +
                    $"and name = '{folderName}' " +
                    $"and '{driveParentId}' in parents " +
                    "and trashed = false";
                listReq.Fields = "files(id, name)";
                var result = await listReq.ExecuteAsync();
                var exist = result.Files.FirstOrDefault();

                string childFolderId;
                if (exist != null)
                {
                    childFolderId = exist.Id;
                }
                else
                {
                    var meta = new Google.Apis.Drive.v3.Data.File
                    {
                        Name = folderName,
                        MimeType = "application/vnd.google-apps.folder",
                        Parents = new[] { driveParentId }
                    };
                    var createReq = _drive.Files.Create(meta);
                    createReq.Fields = "id";
                    var created = await createReq.ExecuteAsync();
                    childFolderId = created.Id;
                }

                count += await UploadLocalFolderToDriveAsync(dir, childFolderId, userId);
            }

            // 2. Upload file trong folder hiện tại
            foreach (var file in Directory.GetFiles(localFolderPath))
            {
                string fileName = Path.GetFileName(file);
                var ext = Path.GetExtension(file);

                // BỎ QUA .py
                if (ext.Equals(".py", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[UploadLocal] SKIP .py file: {fileName}");
                    continue;
                }

                Console.WriteLine($"[UploadLocal]     -> File: {fileName}");

                var searchReq = _drive.Files.List();
                searchReq.Q = $"name = '{fileName}' and '{driveParentId}' in parents and trashed = false";
                searchReq.Fields = "files(id, name)";
                var searchResult = await searchReq.ExecuteAsync();

                using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                const string mimeType = "application/octet-stream";

                Google.Apis.Upload.IUploadProgress progress;

                if (searchResult.Files.Any())
                {
                    // UPDATE
                    string fileId = searchResult.Files[0].Id;
                    var updateReq = _drive.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId, stream, mimeType);

                    progress = await updateReq.UploadAsync();

                    if (progress.Status == Google.Apis.Upload.UploadStatus.Completed)
                    {
                        Console.WriteLine($"[UploadLocal] Updated {fileName} ({fileId}) in parent {driveParentId}");
                        count++;
                        IncreaseUploadedFiles(userId);
                    }
                    else
                    {
                        Console.WriteLine($"[UploadLocal] FAILED update {fileName}: {progress.Status} - {progress.Exception?.Message}");
                    }
                }
                else
                {
                    // CREATE
                    var fileMeta = new Google.Apis.Drive.v3.Data.File
                    {
                        Name = fileName,
                        Parents = new[] { driveParentId }
                    };

                    var uploadReq = _drive.Files.Create(fileMeta, stream, mimeType);
                    uploadReq.Fields = "id";

                    progress = await uploadReq.UploadAsync();

                    if (progress.Status == Google.Apis.Upload.UploadStatus.Completed &&
                        uploadReq.ResponseBody != null)
                    {
                        Console.WriteLine($"[UploadLocal] Created {fileName} ({uploadReq.ResponseBody.Id}) in parent {driveParentId}");
                        count++;
                        IncreaseUploadedFiles(userId);
                    }
                    else
                    {
                        Console.WriteLine($"[UploadLocal] FAILED create {fileName}: {progress.Status} - {progress.Exception?.Message}");
                    }
                }
            }

            return count;
        }
    }
}
