using System.Security.Cryptography;
using System.Text;

namespace OpenAPI3.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string EncryptionKeyName = "key";
        private const string EncryptionIvName = "iv";
        private const string ApiKey = "ApiKey";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            

            if (!context.Request.Headers.TryGetValue(ApiKey, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided. (Using ApiKeyMiddleware) ");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

            var encryptionKeyValue = appSettings.GetValue<string>(EncryptionKeyName);
            var encryptionIvValue = appSettings.GetValue<string>(EncryptionIvName);
            var keyBytes = Encoding.UTF8.GetBytes(encryptionKeyValue);
            var ivBytes = Encoding.UTF8.GetBytes(encryptionIvValue);
            var decryptedFromJavascript =
                DecryptStringFromBytes(Convert.FromBase64String(extractedApiKey), keyBytes, ivBytes);
            var g = string.Format(decryptedFromJavascript);

            var dateInApiKey = DateTime.Parse(g).Date;

            var currentDate = DateTime.Today.Date;

            if (!dateInApiKey.Equals(currentDate))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client. (Using ApiKeyMiddleware)");
                return;
            }
            
            

            await _next(context);
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
               
                try
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }
            return plaintext;
        }
    }
}
