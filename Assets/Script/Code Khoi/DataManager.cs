using System.IO;
using UnityEngine;

public class DataManager
{
    // Hằng lưu tên file
    private const string FILE_NAME = "data.txt";

    // Hàm đọc dữ liệu từ file
    public static DataGame ReadData()
    {
        try
        {
            if (File.Exists(FILE_NAME))
            {
                // Mở file
                var fileStream = new FileStream(FILE_NAME, FileMode.Open);

                // Đọc nội dung
                using (var reader = new StreamReader(fileStream))
                {
                    // Đọc dữ liệu dạng JSON
                    var json = reader.ReadToEnd();

                    // Chuyển từ JSON sang đối tượng DataGame
                    var data = JsonUtility.FromJson<DataGame>(json);
                    return data;
                }
            }
            Debug.Log("File không tồn tại!");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.Log("Lỗi đọc file: " + e.Message);
        }
        return null;
    }

    // Hàm ghi dữ liệu ra file
    public static bool SaveData(DataGame data)
    {
        try
        {
            // Chuyển đối tượng DataGame sang JSON
            var json = JsonUtility.ToJson(data);

            // Tạo file
            var fileStream = new FileStream(FILE_NAME, FileMode.Create);

            // Ghi file
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(json);
            }
            Debug.Log("Lưu dữ liệu thành công!");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log("Lỗi ghi file: " + e.Message);
        }
        return false;
    }
}
