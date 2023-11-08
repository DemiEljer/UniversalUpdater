using Aspose.Zip.Rar;
using System.Net;

string updateHRef = "";
int maxSecondsWaiting = 10;
string extractionPath = null;

bool timeInputFlag = false;
bool pathInputFlag = false;
bool helpShowFlag = false;
for (int i = 0; i < args.Length; i++)
{
    try
    {
        if (args[i] == "-t")
        {
            timeInputFlag = true;
            pathInputFlag = false;
        }
        else if (args[i] == "-h")
        {
            helpShowFlag = true;
            timeInputFlag = false;
            pathInputFlag = false;
        }
        else if (args[i] == "-p")
        {
            pathInputFlag = true;
            timeInputFlag = false;
        }
        else
        {
            if (timeInputFlag)
            {
                try
                {
                    maxSecondsWaiting = int.Parse(args[i]);
                }
                catch
                {
                    maxSecondsWaiting = 10;
                }
            }
            else if (pathInputFlag)
            {
                extractionPath = args[i];
            }
            else
            {
                updateHRef = args[i];
            }

            timeInputFlag = false;
            pathInputFlag = false;
        }
    }
    catch
    {

    }
}

if (helpShowFlag)
{
    Console.WriteLine($"UniversalUpdater v1.1.0");
    Console.WriteLine("==========================================");
    Console.WriteLine("Описание аргументов:");
    Console.WriteLine("-t [time]         : Максимальное время ожидания процесса разархивации;");
    Console.WriteLine("-p [path]         : Указать путь до папки, куда необходимо распаковать обновление;");
    Console.WriteLine("-h                : Вывести справочную ифнормацию по утилите;");
    Console.WriteLine("[href]            : Адрес нахождения прошивки вводится просто как строка.");

    return;
}

UpdateHandler.Update(updateHRef, extractionPath, maxSecondsWaiting);

public class UpdateHandler
{
    public static void Update(string updateHRef, string? extractionPath, int maxSecondsWaiting)
    {
        string pathToUpdateFile = "";

        if (extractionPath == null)
        {
            pathToUpdateFile = $"{AppDomain.CurrentDomain.BaseDirectory}update.rar";
        }
        else
        {
            pathToUpdateFile = $"{extractionPath}update.rar";
        }


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

        while (!extrectionSuccessFlag && (DateTime.Now - startTimeMark).TotalSeconds < maxSecondsWaiting)
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