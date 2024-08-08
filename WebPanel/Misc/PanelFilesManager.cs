namespace WebPanel.Misc
{
    public static class PanelFilesManager
    {
        public static string MAIN_PATH = "SavedFiles";
        public static async Task<bool> SaveFile(Stream stream, string fileName,string tableId,string elementFullName)
        {
            if (stream == null) return false;
            if (stream.Length == 0) return false;

            try
            {
                var filePath = $"{MAIN_PATH}/{tableId}/{elementFullName}/{fileName}";

                using FileStream fileStream = new(filePath, FileMode.Create);
                await stream.CopyToAsync(fileStream);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void DeleteFilePath(string directoryFilePath)
        {
            if (Directory.Exists(directoryFilePath))
                Directory.Delete(directoryFilePath, true);
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public static void CheckElementPath(string tableId,string elementFullName)
        {
            if (!Directory.Exists($"{MAIN_PATH}/{tableId}/{elementFullName}"))
            {
                Directory.CreateDirectory($"{MAIN_PATH}/{tableId}/{elementFullName}");
            }
        } 

        public static void CheckPath()
        {
            if (!Directory.Exists(MAIN_PATH))
            {
                Directory.CreateDirectory(MAIN_PATH);
                Console.WriteLine("Path created: " + MAIN_PATH);
            }
            
        }
    }
}
