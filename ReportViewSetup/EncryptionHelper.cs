using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace ReportViewSetup
{
    /// <summary>
    ///   Help to encrypt the data
    /// </summary>
    public static class EncryptionHelper
    {
        // Don't change these Constants
        private const string EncryptionPassword = "{DA58385B-6171-4984-9954-DB422BE24907}";
        private const string PasswordSalt = "{1AEA8E83-B8AC-4394-8F16-8C6B72DF4237}";

        /// <summary>
        ///   Encrypts the specified input.
        /// </summary>
        /// <param name = "input">The input.</param>
        /// <returns>Return the encrypted input</returns>
        /// <exception cref = "System.ArgumentNullException">Raised when the input is null.</exception>
        /// <exception cref = "EncryptionException">Raised when the input cannot be encrypted</exception>
        public static string Encrypt(string input)
        {
          

            try
            {
                string data = input;

                byte[] encryptBytes = null;

                byte[] utfdata = Encoding.UTF8.GetBytes(data);

                byte[] saltBytes = Encoding.UTF8.GetBytes(PasswordSalt);

                // Our symmetric encryption algorithm
                using (AesManaged aes = new AesManaged())
                {
                    // Setting our parameters
                    aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;

                    aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                    Rfc2898DeriveBytes rfc = null;

#if !SILVERLIGHT && !DATABASE
                    try
                    {
#endif
                        rfc = new Rfc2898DeriveBytes(EncryptionPassword, saltBytes);

                        aes.Key = rfc.GetBytes(aes.KeySize / 8);

                        aes.IV = rfc.GetBytes(aes.BlockSize / 8);
#if !SILVERLIGHT && !DATABASE
                    }
                    finally
                    {
                        if (rfc != null)
                        {
                            rfc.Dispose();
                        }
                    }
#endif
                    // Encryption
                    ICryptoTransform encryptTransf = aes.CreateEncryptor();

                    // Output stream, can be also a FileStream
                    using (MemoryStream encryptStream = new MemoryStream())
                    {
                        using (CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransf, CryptoStreamMode.Write))
                        {
                            encryptor.Write(utfdata, 0, utfdata.Length);

                            encryptor.Flush();
                        }

                        // Showing our encrypted content
                        encryptBytes = encryptStream.ToArray();
                    }
                }

                return Convert.ToBase64String(encryptBytes);
            }
            catch (Exception ex)
            {
                //throw new EncryptionException("Encryption failed. See the inner exception for more detail.", ex);

                return string.Empty; 
            }
        }

        /// <summary>
        ///   Decrypts the specified base64 input previously encrypted by the method <c>Encrypt</c>.
        /// </summary>
        /// <param name = "base64Input">The base64 input.</param>
        /// <returns>Return the decrypted input</returns>
        /// <exception cref = "System.ArgumentNullException">Raised when the input is null.</exception>
        /// <exception cref = "EncryptionException">Raised when the input cannot be decrypted</exception>
        public static string Decrypt(string base64Input)
        {
        

            try
            {
                byte[] decryptBytes = null;

                byte[] encryptBytes = Convert.FromBase64String(base64Input);

                byte[] saltBytes = Encoding.UTF8.GetBytes(PasswordSalt);

                // Our symmetric encryption algorithm
                using (AesManaged aes = new AesManaged())
                {
                    // Setting our parameters
                    aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;

                    aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                    Rfc2898DeriveBytes rfc = null;

#if !SILVERLIGHT && !DATABASE
                    try
                    {
#endif
                        rfc = new Rfc2898DeriveBytes(EncryptionPassword, saltBytes);

                        aes.Key = rfc.GetBytes(aes.KeySize / 8);

                        aes.IV = rfc.GetBytes(aes.BlockSize / 8);
#if !SILVERLIGHT && !DATABASE
                    }
                    finally
                    {
                        if (rfc != null)
                        {
                            rfc.Dispose();
                        }
                    }
#endif

                    // Now, decryption
                    ICryptoTransform decryptTrans = aes.CreateDecryptor();

                    // Output stream, can be also a FileStream
                    using (MemoryStream decryptStream = new MemoryStream())
                    {
                        using (CryptoStream decryptor = new CryptoStream(decryptStream, decryptTrans, CryptoStreamMode.Write))
                        {
                            decryptor.Write(encryptBytes, 0, encryptBytes.Length);

                            decryptor.Flush();
                        }

                        // Showing our decrypted content
                        decryptBytes = decryptStream.ToArray();
                    }
                }

                return Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
            }
            catch (Exception ex)
            {
               // throw new EncryptionException("Decryption failed.", ex);
                return string.Empty;
            }
        }
    }
}