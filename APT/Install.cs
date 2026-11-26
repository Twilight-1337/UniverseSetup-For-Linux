using System;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Linq;

if (!Environment.GetCommandLineArgs().Contains("--terminal"))
{
    string? executablePath = Environment.ProcessPath;

    if (string.IsNullOrEmpty(executablePath))
    {
        Console.WriteLine("Cannot determine executable path.");
        return;
    }

    var terminals = new (string Name, string Argument)[]
{
    ("konsole", "-e"),
    ("ptyxis", "--"),
    ("gnome-terminal", "--"),
};

    bool launched = false;

    foreach (var terminal in terminals)
    {
        string? terminalPath = FindExecutable(terminal.Name);

        if (terminalPath == null)
            continue;

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = terminalPath
        };

        startInfo.ArgumentList.Add(terminal.Argument);
        startInfo.ArgumentList.Add(executablePath);
        startInfo.ArgumentList.Add("--terminal");

        try
        {
            Process.Start(startInfo);
            launched = true;
            break;
        }
        catch (Win32Exception)
        {
            continue;
        }
    }

    if (!launched)
    {
        Console.WriteLine("No supported terminal found!");
    }

    return;
}

static string? FindExecutable(string name)
{
    string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? "";

    foreach (string directory in pathVariable.Split(Path.PathSeparator))
    {
        string path = Path.Combine(directory, name);

        if (File.Exists(path))
            return path;
    }

    return null;
}

Console.WriteLine("===== UniverseSetup for Linux =====");
Console.WriteLine("Packages to install:");
Console.WriteLine("- Fastfetch");
Console.WriteLine("- Firefox");
Console.WriteLine("- Git");
Console.WriteLine("- VSCodium");
Console.WriteLine("- Prism Launcher");
Console.WriteLine("- Btop");
Console.WriteLine("- Rustup");
Console.WriteLine("- .NET SDK 10.0");

Console.WriteLine();
Console.Write("Do you want to continue? (yes/no): ");

string answer = Console.ReadLine() ?? "";

if (answer.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine();
    Console.WriteLine("Starting installation...");

    Process process = new Process();

    process.StartInfo.FileName = "pkexec";
    process.StartInfo.ArgumentList.Add("sh");
    process.StartInfo.ArgumentList.Add("-c");

    process.StartInfo.ArgumentList.Add("""
set -e

. /etc/os-release

case "$ID" in
    debian|ubuntu)
        ;;
    *)
        echo "Unsupported distribution: $ID"
        exit 1
        ;;
esac

echo "Updating package lists..."
apt-get update

echo "Installing repository dependencies..."
apt-get install -y curl gpg

echo "Adding VSCodium repository..."

curl -fsSL https://repo.vscodium.dev/vscodium.gpg \
    | gpg --dearmor --yes -o /usr/share/keyrings/vscodium.gpg

if { [ "$ID" = "debian" ] && dpkg --compare-versions "$VERSION_ID" ge 13; } ||
   { [ "$ID" = "ubuntu" ] && dpkg --compare-versions "$VERSION_ID" ge 24.04; }; then

    curl -fsSL https://repo.vscodium.dev/vscodium.sources \
        -o /etc/apt/sources.list.d/vscodium.sources
else
    curl -fsSL https://repo.vscodium.dev/vscodium.list \
        -o /etc/apt/sources.list.d/vscodium.list
fi

echo "Adding Prism Launcher repository..."

curl -fsSL https://prism-launcher-for-debian.github.io/repo/prismlauncher.gpg \
    -o /usr/share/keyrings/prismlauncher-archive-keyring.gpg

CODENAME="${UBUNTU_CODENAME:-${DEBIAN_CODENAME:-${VERSION_CODENAME}}}"

printf 'Types: deb\nURIs: https://prism-launcher-for-debian.github.io/repo\nSuites: %s\nComponents: main\nSigned-By: /usr/share/keyrings/prismlauncher-archive-keyring.gpg\n' "$CODENAME" \
    > /etc/apt/sources.list.d/prismlauncher.sources

echo "Updating repositories..."
apt-get update

echo "Installing packages..."

apt-get install -y \
    fastfetch \
    firefox \
    git \
    codium \
    prismlauncher \
    btop \
    rustup

echo "Installing .NET SDK 10.0..."

if ! apt-cache show dotnet-sdk-10.0 >/dev/null 2>&1; then
    echo ".NET SDK 10.0 is not available in configured repositories."
    echo "Please configure the official Microsoft APT repository first."
    exit 1
fi

apt-get install -y dotnet-sdk-10.0

echo "Installation completed successfully!"
""");

    process.Start();
    process.WaitForExit();

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
