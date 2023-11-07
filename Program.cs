using Aspose.Zip.Rar;
using System.Net;

string updateHRef = "";

for (int i = 0; i < args.Length; i++)
{
    try
    {
        updateHRef = args[i];
    }
    catch
    {

    }
}

UpdateHandler.Update(updateHRef);

public class UpdateHandler
{
    public static void Update(string updateHRef)
    {
        string pathToUpdateFile = $"{AppDomain.CurrentDomain.BaseDirectory}update.rar";

        Console.WriteLine("Загрузка файлов утилиты...");
        try
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(updateHRef, pathToUpdateFile);
            }
        }
        catch
        {
            Console.WriteLine("! Ошибка загрузки файла обновления !");
            return;
        }

        if (File.Exists(pathToUpdateFile))
        {
            Console.WriteLine("Файл обновления был успешно скачан!");
        }
        else
        {
            Console.WriteLine("! Ошибка загрузки файла обновления !");
            return;
        }

        RarArchive archive = null;
        try
        {
            archive = new RarArchive(pathToUpdateFile);
        }
        catch
        {
            Console.WriteLine("! Ошибка открытия файла обновления !");
            
            return;
        }

        Console.WriteLine("Начат процесс обновления утилиты...");

        bool extrectionSuccessFlag = false;
        SpinWait waiter = new SpinWait();
        
        DateTime startTimeMark = DateTime.Now;

        while (!extrectionSuccessFlag && (DateTime.Now - startTimeMark).TotalSeconds < 10)
        {
            try
            {
                archive.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory);
                extrectionSuccessFlag = true;
            }
            catch
            {
                waiter.SpinOnce();
            }
        }

        if (extrectionSuccessFlag)
        {
            Console.WriteLine("Утилита была успешно обновлена!");
        }
        else
        {
            Console.WriteLine("! Ошибка распаковки файлов обновления !");
        }

        archive.Dispose();
        File.Delete(pathToUpdateFile);

        Console.WriteLine("Файл обновления был удален!");

        return;
    }

}