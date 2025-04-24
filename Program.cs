using System;
using System.Diagnostics;
using System.IO;
using System.Net;

class Program
{
    static string localVersionFile = "version.txt";
    static string remoteVersionUrl = "https://tuservidor.com/version.txt"; // ← cámbialo
    static string remoteExeUrl = "https://tuservidor.com/tuApp.exe";       // ← cámbialo
    static string localExe = "tuApp.exe";

    static void Main()
    {
        try
        {
            string localVersion = File.Exists(localVersionFile) ? File.ReadAllText(localVersionFile).Trim() : "0.0.0";

            using (WebClient client = new WebClient())
            {
                Console.WriteLine("Verificando versión...");
                string remoteVersion = client.DownloadString(remoteVersionUrl).Trim();

                if (remoteVersion != localVersion)
                {
                    Console.WriteLine($"Nueva versión {remoteVersion} disponible. Actualizando...");

                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;

                    Uri downloadUri = new Uri(remoteExeUrl);
                    client.DownloadFileAsync(downloadUri, localExe);

                    Console.WriteLine("Descargando archivo...");
                    while (client.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    File.WriteAllText(localVersionFile, remoteVersion);
                }
                else
                {
                    Console.WriteLine("Ya tienes la última versión.");
                }
            }

            if (File.Exists(localExe))
            {
                Console.WriteLine("Iniciando aplicación...");
                Process.Start(localExe);
            }
            else
            {
                Console.WriteLine("No se encontró el archivo ejecutable.");
            }
        }
        catch (WebException ex)
        {
            Console.WriteLine("Error de red: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado: " + ex.Message);
        }
    }

    static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        Console.CursorLeft = 0;
        Console.Write($"Progreso: {e.ProgressPercentage}%   ");
    }

    static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        Console.WriteLine("\nDescarga completada.");
        if (e.Error != null)
        {
            Console.WriteLine("Error al descargar: " + e.Error.Message);
        }
    }
}
