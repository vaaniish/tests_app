using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

public static class RuntimeSecrets
{
    private const string SecretsDirectoryName = ".runtime";
    private const string SecretsFileName = "app.keys.dat";
    private const string LegacySecretsFileName = "security.keys.json";
    private static readonly Lazy<SecretsSnapshot> Snapshot = new Lazy<SecretsSnapshot>(LoadSecrets);

    public static string PortableEncryptionSecret => Snapshot.Value.PortableEncryptionSecret;
    public static string LegacyAnswerEncryptionSecret => Snapshot.Value.LegacyAnswerEncryptionSecret;
    public static string DatabasePassword => Snapshot.Value.DatabasePassword;
    public static string AdminLoginSalt => Snapshot.Value.AdminLoginSalt;
    public static string AdminLoginHash => Snapshot.Value.AdminLoginHash;
    public static string AdminPasswordSalt => Snapshot.Value.AdminPasswordSalt;
    public static string AdminPasswordHash => Snapshot.Value.AdminPasswordHash;

    private static SecretsSnapshot LoadSecrets()
    {
        try
        {
            var json = LoadSecretsJson();
            var serializer = new JavaScriptSerializer();
            var root = serializer.Deserialize<Dictionary<string, object>>(json)
                ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            return new SecretsSnapshot
            {
                PortableEncryptionSecret = ReadRequired(root, "portableEncryptionSecret"),
                LegacyAnswerEncryptionSecret = ReadRequired(root, "legacyAnswerEncryptionSecret"),
                DatabasePassword = ReadRequired(root, "dbPassword"),
                AdminLoginSalt = ReadRequired(root, "adminLoginSalt"),
                AdminLoginHash = ReadRequired(root, "adminLoginHash"),
                AdminPasswordSalt = ReadRequired(root, "adminPasswordSalt"),
                AdminPasswordHash = ReadRequired(root, "adminPasswordHash")
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Файл ключей поврежден или содержит пустые поля.",
                ex
            );
        }
    }

    private static string LoadSecretsJson()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var candidates = new[]
        {
            Path.Combine(baseDirectory, SecretsDirectoryName, SecretsFileName),
            Path.Combine(baseDirectory, SecretsDirectoryName, LegacySecretsFileName),
            Path.Combine(baseDirectory, LegacySecretsFileName)
        };

        foreach (var path in candidates)
        {
            if (File.Exists(path))
                return File.ReadAllText(path);
        }

        var asm = typeof(RuntimeSecrets).Assembly;
        using (var stream = asm.GetManifestResourceStream("TESTS.security.keys.json"))
        {
            if (stream != null)
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        throw new InvalidOperationException(
            "Не найден файл ключей. Ожидается встроенный ресурс TESTS.security.keys.json " +
            "или внешний .runtime\\app.keys.dat рядом с EXE."
        );
    }

    private static string ReadRequired(Dictionary<string, object> root, string key)
    {
        if (root == null)
            throw new InvalidOperationException("Пустой объект ключей.");

        root.TryGetValue(key, out var raw);
        var value = raw == null ? string.Empty : raw.ToString();

        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("Пустое значение ключа: " + key);

        return value.Trim();
    }

    private sealed class SecretsSnapshot
    {
        public string PortableEncryptionSecret { get; set; }
        public string LegacyAnswerEncryptionSecret { get; set; }
        public string DatabasePassword { get; set; }
        public string AdminLoginSalt { get; set; }
        public string AdminLoginHash { get; set; }
        public string AdminPasswordSalt { get; set; }
        public string AdminPasswordHash { get; set; }
    }
}
