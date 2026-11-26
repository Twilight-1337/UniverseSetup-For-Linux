using System;
using System.Diagnostics;

if (!Environment.GetCommandLineArgs().Contains("--terminal"))
{
    ProcessStartInfo startInfo = new ProcessStartInfo
    {
        FileName = "konsole"
    };

    startInfo.ArgumentList.Add("-e");
    startInfo.ArgumentList.Add(Environment.ProcessPath!);
    startInfo.ArgumentList.Add("--terminal");

    Process.Start(startInfo);

    return;
}

Console.WriteLine("===== Fedora Setup =====");
Console.WriteLine("Packages to install:");
Console.WriteLine("- Fastfetch");
Console.WriteLine("- Firefox");
Console.WriteLine("- Prism Launcher");
Console.WriteLine("- VS Codium");
Console.WriteLine("-  Git");
Console.WriteLine("- C#/.NET SDK");
Console.WriteLine("- BTop");
Console.WriteLine("- Rust");


Console.WriteLine();
Console.Write("Do you want to continue? (yes/no): ");

string answer = Console.ReadLine() ?? "";

if (answer.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("Starting installation...");

    Process process = new Process();

    process.StartInfo.FileName = "pkexec";
    process.StartInfo.ArgumentList.Add("dnf");
    process.StartInfo.ArgumentList.Add("install");
    process.StartInfo.ArgumentList.Add("-y");
    process.StartInfo.ArgumentList.Add("fastfetch");
    process.StartInfo.ArgumentList.Add("firefox");
    process.StartInfo.ArgumentList.Add("git");
    process.StartInfo.ArgumentList.Add("codium");
    process.StartInfo.ArgumentList.Add("prismlauncher");
    process.StartInfo.ArgumentList.Add("btop");
    process.StartInfo.ArgumentList.Add("rustup");
    process.StartInfo.ArgumentList.Add("dotnet-sdk-10.0");

    Console.WriteLine("Requesting Super User Permisions");

    if (!process.Start())
    {
        Console.WriteLine("Failed to start pkexec");
        return;
    }

    Console.WriteLine("Waiting for auth ant installation");

    process.WaitForExit();

    Console.WriteLine("Process finished with exit code: " + process.ExitCode);

    if (process.ExitCode == 0)
    {
        Console.WriteLine("Installation completed successfully!");
    }
    else
    {
        Console.WriteLine("Installation failed! Exit code: " + process.ExitCode);
    }
}
else
{
    Console.WriteLine("Installation cancelled.");
}