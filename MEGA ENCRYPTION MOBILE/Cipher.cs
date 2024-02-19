using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Views;
using MEGA_ENCRYPTION_MOBILE.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MEGA_ENCRYPTION_MOBILE
{
    public class Cipher
    {/////////////////////////////////////Encryption///////////////////////////////////////////

        private static byte[] _abc;
        private static byte[,] _table;


        public static async Task Encrypt(string path, string psw, IFileSaver file, CancellationTokenSource cancel)
        {
            // Initialize abc and table
            _abc = new byte[256];
            for (int i = 0; i < 256; i++)
                _abc[i] = Convert.ToByte(i);

            _table = new byte[256, 256];
            for (int i = 0; i < 256; i++)
                for (int j = 0; j < 256; j++)
                    _table[i, j] = _abc[(i + j) % 256];
            try
            {
                var fileContent = File.ReadAllBytes(path);
                var pswTmp = Encoding.ASCII.GetBytes(psw);
                var keys = new byte[fileContent.Length];
                var result = new byte[fileContent.Length];
                for (int i = 0; i < fileContent.Length; i++)
                    keys[i] = pswTmp[i % pswTmp.Length];

                //Encrypt
                for (int i = 0; i < fileContent.Length; i++)
                {
                    var value = fileContent[i];
                    var key = keys[i];
                    int valueIndex = -1, keyIndex = -1;
                    for (int j = 0; j < 256; j++)
                        if (_abc[j] == value)
                        {
                            valueIndex = j;
                            break;
                        }
                    for (int j = 0; j < 256; j++)
                        if (_abc[j] == key)
                        {
                            keyIndex = j;
                            break;
                        }
                    result[i] = _table[keyIndex, valueIndex];
                }


                // Save result to a new file with the same extension
                var extension = Path.GetExtension(path);
                var name = Path.GetFileName(path);
                var fullName = Path.ChangeExtension(name, extension);

                using var stream = new MemoryStream(result);
                var final = await file.SaveAsync(fullName, stream, cancel.Token);
                if (final.IsSuccessful)
                    await Toast.Make($"File downloaded as: {fullName}").Show(cancel.Token);
                else
                    await Toast.Make("Failed to download the file.").Show(cancel.Token);
            }
            catch (Exception ex) { await Toast.Make(ex.Message).Show(cancel.Token); }
        }
        public static async Task Decrypt(string path, string psw, IFileSaver file, CancellationTokenSource cancel)
        {
            // Initialize abc and table
            _abc = new byte[256];
            for (int i = 0; i < 256; i++)
                _abc[i] = Convert.ToByte(i);

            _table = new byte[256, 256];
            for (int i = 0; i < 256; i++)
                for (int j = 0; j < 256; j++)
                    _table[i, j] = _abc[(i + j) % 256];
            try
            {
                var fileContent = File.ReadAllBytes(path);
                var pswTmp = Encoding.ASCII.GetBytes(psw);
                var keys = new byte[fileContent.Length];
                var result = new byte[fileContent.Length];
                for (int i = 0; i < fileContent.Length; i++)
                    keys[i] = pswTmp[i % pswTmp.Length];

                //Decrypt
                for (int i = 0; i < fileContent.Length; i++)    
                {
                    var value = fileContent[i];
                    var key = keys[i];
                    int valueIndex = -1, keyIndex = -1;
                    for (int j = 0; j < 256; j++)
                        if (_abc[j] == key)
                        {
                            keyIndex = j;
                            break;
                        }
                    for (int j = 0; j < 256; j++)
                        if (_table[keyIndex, j] == value)
                        {
                            valueIndex = j;
                            break;
                        }
                    result[i] = _abc[valueIndex];
                }


                // Save result to a new file with the same extension
                var extension = Path.GetExtension(path);
                var name = Path.GetFileName(path);
                var fullName = Path.ChangeExtension(name, extension);

                using var stream = new MemoryStream(result);
                var final = await file.SaveAsync(fullName, stream, cancel.Token);
                if (final.IsSuccessful)
                    await Toast.Make($"File downloaded as: {fullName}").Show(cancel.Token);
                else
                    await Toast.Make("Failed to download the file.").Show(cancel.Token);
            }
            catch (Exception) { await Toast.Make("Error Error Error").Show(cancel.Token); }
        }
        //////////////////////////////////////////////////////////////////////////////////////////

    }
}
