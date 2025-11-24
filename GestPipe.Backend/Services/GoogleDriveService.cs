using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

public class GoogleDriveService
{
    private readonly DriveService _drive;
    private readonly string _pythonBasePath;
    private readonly string _rootFolderId;

    public GoogleDriveService(IConfiguration config)
    {
        var credPath = config["GoogleDrive:CredentialsPath"];
        _pythonBasePath = config["GoogleDrive:PythonBasePath"];
        _rootFolderId = config["GoogleDrive:RootFolderId"];

        using var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read);
        var credential = GoogleCredential.FromStream(stream)
            .CreateScoped(DriveService.Scope.Drive);

        _drive = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = config["GoogleDrive:ApplicationName"]
        });
    }
    //public async Task<int> SyncUserFolderAsync(string userId)
    //{
    //    string folderName = $"user_{userId}";   // hoặc "user_Khang" nếu test tay

    //    // 1. Tìm folder user_{userId} *bên trong* CustomGesture
    //    var listReq = _drive.Files.List();
    //    listReq.Q =
    //        $"mimeType = 'application/vnd.google-apps.folder' " +
    //        $"and name = '{folderName}' " +
    //        $"and '{_rootFolderId}' in parents " +   // 👈 chỉ tìm trong CustomGesture
    //        $"and trashed = false";
    //    listReq.Fields = "files(id, name)";
    //    var folderResult = await listReq.ExecuteAsync();

    //    var folder = folderResult.Files.FirstOrDefault();
    //    if (folder == null)
    //        return 0;

    //    // 2. Lấy file con như cũ
    //    var fileListReq = _drive.Files.List();
    //    fileListReq.Q = $"'{folder.Id}' in parents and trashed = false";
    //    fileListReq.Fields = "files(id, name, mimeType)";
    //    var filesResult = await fileListReq.ExecuteAsync();

    //    string localUserDir = Path.Combine(_pythonBasePath, folderName);
    //    Directory.CreateDirectory(localUserDir);

    //    int count = 0;
    //    foreach (var file in filesResult.Files)
    //    {
    //        if (file.MimeType == "application/vnd.google-apps.folder")
    //            continue; // nếu có folder con nữa thì tạm bỏ qua

    //        var getReq = _drive.Files.Get(file.Id);
    //        string localPath = Path.Combine(localUserDir, file.Name);

    //        using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write);
    //        await getReq.DownloadAsync(fs);    // ghi đè file
    //        count++;
    //    }

    //    return count;
    //}

    public async Task<int> SyncUserFolderAsync(string userId)
    {
        string folderName = $"user_{userId}";

        // 1. Tìm folder user_{userId} bên trong CustomGesture
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
            return 0;

        // 2. Thư mục local gốc
        string localUserDir = Path.Combine(_pythonBasePath, folderName);

        //  👉 GIỮ file cũ, chỉ tạo nếu chưa có:
        Directory.CreateDirectory(localUserDir);

        //  👉 NẾU MUỐN XÓA HẾT RỒI TẢI LẠI (full replace) thì dùng:
        // if (Directory.Exists(localUserDir))
        // {
        //     Directory.Delete(localUserDir, true);
        // }
        // Directory.CreateDirectory(localUserDir);

        // 3. Đệ quy sync cả cây thư mục (models, raw_data, training_results, ...)
        return await SyncDriveFolderToLocalAsync(folder.Id, localUserDir);
    }

    private async Task<int> SyncDriveFolderToLocalAsync(string driveFolderId, string localFolderPath)
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
                // folder con → đệ quy
                var subLocalPath = Path.Combine(localFolderPath, file.Name);
                count += await SyncDriveFolderToLocalAsync(file.Id, subLocalPath);
            }
            else
            {
                // file thường → ghi đè (chỉ đè file trùng tên, file khác vẫn giữ)
                var getReq = _drive.Files.Get(file.Id);
                string localPath = Path.Combine(localFolderPath, file.Name);

                using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write);
                await getReq.DownloadAsync(fs);
                count++;
            }
        }

        return count;
    }


    //private async Task<int> SyncDriveFolderToLocalAsync(string driveFolderId, string localFolderPath)
    //{
    //    int count = 0;
    //    Directory.CreateDirectory(localFolderPath);

    //    var fileListReq = _drive.Files.List();
    //    fileListReq.Q = $"'{driveFolderId}' in parents and trashed = false";
    //    fileListReq.Fields = "files(id, name, mimeType)";
    //    var filesResult = await fileListReq.ExecuteAsync();

    //    foreach (var file in filesResult.Files)
    //    {
    //        if (file.MimeType == "application/vnd.google-apps.folder")
    //        {
    //            // Folder con -> đệ quy
    //            var subLocalPath = Path.Combine(localFolderPath, file.Name);
    //            count += await SyncDriveFolderToLocalAsync(file.Id, subLocalPath);
    //        }
    //        else
    //        {
    //            // File thường -> ghi đè
    //            var getReq = _drive.Files.Get(file.Id);
    //            string localPath = Path.Combine(localFolderPath, file.Name);

    //            using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write);
    //            await getReq.DownloadAsync(fs);
    //            count++;
    //        }
    //    }

    //    return count;
    //}
}
