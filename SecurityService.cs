using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public static class SecurityService
{
    private const int SaltSizeBytes = 16;
    private const int HashSizeBytes = 32;
    private const int SecretIterations = 120000;
    private const int AnswerIterations = 8000;
    private const int EncryptionKeySizeBytes = 32;
    private const int EncryptionIterations = 6000;

    private const string PortablePayloadVersion = "v3";
    private const string PortableLegacyPayloadVersion = "v2";
    private const string LegacyPayloadVersion = "v1";

    // Секреты подгружаются из security.keys.json рядом с EXE.
    private static readonly byte[] PortableKey = BuildPortableKey();

    public static string NormalizeLogin(string value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }

    public static string NormalizeText(string value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }

    public static bool VerifySecret(string value, string saltBase64, string hashBase64, bool normalize)
    {
        if (string.IsNullOrWhiteSpace(saltBase64) || string.IsNullOrWhiteSpace(hashBase64))
            return false;

        var candidate = normalize ? NormalizeLogin(value) : (value ?? string.Empty);
        var computed = ComputeHash(candidate, saltBase64, SecretIterations);
        return ConstantTimeEquals(hashBase64, computed);
    }

    // Enc-only: для новых/измененных вопросов храним только зашифрованный ответ.
    public static void ProtectQuestionAnswer(Question question)
    {
        if (question == null)
            return;

        if (string.IsNullOrWhiteSpace(question.Answer))
            return;

        question.AnswerEncrypted = EncryptAnswerForStorage(question.Answer.Trim());
        question.AnswerHash = string.Empty;
        question.AnswerSalt = string.Empty;
        question.Answer = string.Empty;
    }

    public static string EncryptAnswerForStorage(string answer)
    {
        if (string.IsNullOrWhiteSpace(answer))
            return string.Empty;

        var plainBytes = Encoding.UTF8.GetBytes(answer);
        var iv = GenerateRandomBytes(16);

        using (var aes = Aes.Create())
        {
            aes.KeySize = EncryptionKeySizeBytes * 8;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = PortableKey;
            aes.IV = iv;

            using (var encryptor = aes.CreateEncryptor())
            {
                var encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return string.Join(".",
                    PortablePayloadVersion,
                    Convert.ToBase64String(iv),
                    Convert.ToBase64String(encrypted)
                );
            }
        }
    }

    public static string TryDecryptAnswerForStorage(string encryptedPayload)
    {
        if (string.IsNullOrWhiteSpace(encryptedPayload))
            return string.Empty;

        try
        {
            var parts = encryptedPayload.Split('.');
            if (parts.Length == 3 && string.Equals(parts[0], PortablePayloadVersion, StringComparison.Ordinal))
            {
                var iv = Convert.FromBase64String(parts[1]);
                var cipher = Convert.FromBase64String(parts[2]);
                return DecryptAes(PortableKey, iv, cipher);
            }

            if (parts.Length == 4 && string.Equals(parts[0], PortableLegacyPayloadVersion, StringComparison.Ordinal))
            {
                var keySalt = Convert.FromBase64String(parts[1]);
                var iv = Convert.FromBase64String(parts[2]);
                var cipher = Convert.FromBase64String(parts[3]);
                var key = DeriveEncryptionKey(RuntimeSecrets.PortableEncryptionSecret, keySalt, EncryptionIterations);
                return DecryptAes(key, iv, cipher);
            }

            if (parts.Length == 4 && string.Equals(parts[0], LegacyPayloadVersion, StringComparison.Ordinal))
            {
                var keySalt = Convert.FromBase64String(parts[1]);
                var iv = Convert.FromBase64String(parts[2]);
                var cipher = Convert.FromBase64String(parts[3]);
                var key = DeriveEncryptionKey(RuntimeSecrets.LegacyAnswerEncryptionSecret, keySalt, EncryptionIterations);
                return DecryptAes(key, iv, cipher);
            }

            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static bool VerifyQuestionAnswer(Question question, object userAnswer)
    {
        if (question == null)
            return false;

        var canonicalUser = CanonicalizeUserAnswer(question.Type, userAnswer);

        // Новый путь: проверка по расшифрованному значению.
        if (!string.IsNullOrWhiteSpace(question.AnswerEncrypted))
        {
            var decrypted = TryDecryptAnswerForStorage(question.AnswerEncrypted);
            if (!string.IsNullOrWhiteSpace(decrypted))
            {
                var canonicalStored = CanonicalizeAnswer(question.Type, decrypted);
                return string.Equals(canonicalUser, canonicalStored, StringComparison.Ordinal);
            }
        }

        // Совсем старый fallback: открытый Answer.
        var legacy = CanonicalizeAnswer(question.Type, question.Answer);
        return string.Equals(canonicalUser, legacy, StringComparison.Ordinal);
    }

    public static string CanonicalizeAnswer(QuestionType type, string answer)
    {
        if (type == QuestionType.Multiple)
            return CanonicalizeMultiple(answer);

        return NormalizeText(answer);
    }

    private static string CanonicalizeUserAnswer(QuestionType type, object userAnswer)
    {
        if (type == QuestionType.Multiple)
        {
            var list = userAnswer as List<string> ?? new List<string>();
            return CanonicalizeMultiple(list);
        }

        return NormalizeText(userAnswer as string);
    }

    private static string CanonicalizeMultiple(string semicolonSeparated)
    {
        return CanonicalizeMultiple(
            (semicolonSeparated ?? string.Empty)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
        );
    }

    private static string CanonicalizeMultiple(IEnumerable<string> values)
    {
        return string.Join(
            ";",
            (values ?? Enumerable.Empty<string>())
                .Select(NormalizeText)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(x => x, StringComparer.Ordinal)
        );
    }

    private static string GenerateSalt()
    {
        var salt = GenerateRandomBytes(SaltSizeBytes);
        return Convert.ToBase64String(salt);
    }

    private static byte[] GenerateRandomBytes(int size)
    {
        var bytes = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return bytes;
    }

    private static string ComputeHash(string value, string saltBase64, int iterations)
    {
        var salt = Convert.FromBase64String(saltBase64);
        using (var derive = new Rfc2898DeriveBytes(
            value ?? string.Empty,
            salt,
            iterations,
            HashAlgorithmName.SHA256))
        {
            return Convert.ToBase64String(derive.GetBytes(HashSizeBytes));
        }
    }

    private static byte[] DeriveEncryptionKey(string secret, byte[] salt, int iterations)
    {
        using (var derive = new Rfc2898DeriveBytes(
            secret ?? string.Empty,
            salt,
            iterations,
            HashAlgorithmName.SHA256))
        {
            return derive.GetBytes(EncryptionKeySizeBytes);
        }
    }

    private static byte[] BuildPortableKey()
    {
        using (var sha = SHA256.Create())
        {
            return sha.ComputeHash(Encoding.UTF8.GetBytes(RuntimeSecrets.PortableEncryptionSecret));
        }
    }

    private static string DecryptAes(byte[] key, byte[] iv, byte[] cipher)
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = EncryptionKeySizeBytes * 8;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor())
            {
                var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
        }
    }

    private static bool ConstantTimeEquals(string leftBase64, string rightBase64)
    {
        byte[] left;
        byte[] right;

        try
        {
            left = Convert.FromBase64String(leftBase64);
            right = Convert.FromBase64String(rightBase64);
        }
        catch
        {
            return false;
        }

        if (left.Length != right.Length)
            return false;

        var diff = 0;
        for (var i = 0; i < left.Length; i++)
        {
            diff |= left[i] ^ right[i];
        }

        return diff == 0;
    }
}

