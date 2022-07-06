using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

//SourceCode References
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;

using HashService;
using System.Security.Cryptography;
using System.IO;

namespace K2HashServiceService
{
    class DecodeMethod : IDisposable
    {
        //Local Variables to match ServiceInstance Configurations
        private string _HashKey = string.Empty;
        private string _SaltKey = string.Empty;
        private string _VIKey = string.Empty;

        internal DecodeMethod()
        {

        }

        internal DecodeMethod(string strHashKey, string strSalKey, string strVIKey)
        {
            _HashKey = strHashKey;
            _SaltKey = strSalKey;
            _VIKey = strVIKey;
        }

        public void Dispose()
        {
            _HashKey = string.Empty;
            _SaltKey = string.Empty;
            _VIKey = string.Empty;
        }

        //Set properties of the ServiceObject based on the values from the Resource File
        internal ServiceObject DescribeServiceObject()
        {
            ServiceObject serviceObject = null;

            serviceObject = new ServiceObject();
            serviceObject.Name = Resources.K2_Decode_ServiceObject_Name;
            serviceObject.MetaData.DisplayName = Resources.K2_Decode_ServiceObject_DisplayName;
            serviceObject.MetaData.Description = Resources.K2_Decode_ServiceObject_Description;
            serviceObject.Type = typeof(DecodeMethod).Name;
            serviceObject.Active = true;
            serviceObject.Properties = GetProperties(serviceObject);
            serviceObject.Methods = GetMethods(serviceObject);

            return serviceObject;
        }

        //Define the SmartObjects Properties and the Property Types
        private Properties GetProperties(ServiceObject serviceObject)
        {
            try
            {
                Properties properties = new Properties();

                properties.Create(new Property("InputString", "System.String", SoType.Text, new MetaData("InputString", string.Empty)));
                properties.Create(new Property("ReturnString", "System.String", SoType.Text, new MetaData("ReturnString", string.Empty)));

                return properties;
            }
            catch (Exception ex)
            {
                throw new Exception("Error GetProperties: " + ex.ToString());
            }
        }

        //Define the method with all its properties based on the values from the Resource File 
        private Methods GetMethods(ServiceObject serviceObject)
        {
            Methods methods = new Methods();

            methods.Create(new Method(Resources.K2_Decode_Method_Name, MethodType.List, new MetaData(Resources.K2_Decode_Method_DisplayName, 
                   Resources.K2_Decode_Method_Description), 
                   GetRequiredProperties(serviceObject, Resources.K2_Decode_Method_Name), 
                   GetMethodParameters(serviceObject, Resources.K2_Decode_Method_Name), 
                   GetInputProperties(serviceObject, Resources.K2_Decode_Method_Name), 
                   GetReturnProperties(serviceObject, Resources.K2_Decode_Method_Name)));

            return methods;
        }

        //Define the Input Properties of the SmartObject Method
        private InputProperties GetInputProperties(ServiceObject serviceObject, string strMethodName)
        {
            InputProperties inputProperties = new InputProperties();

            if (strMethodName.Equals(Resources.K2_Decode_Method_Name))
            {
                inputProperties.Add(serviceObject.Properties["InputString"]);
            }

            return inputProperties;
        }

        //Define the Method Parameters of the SmartObject Method
        private MethodParameters GetMethodParameters(ServiceObject serviceObject, string strMethodName)
        {
            MethodParameters methodParameters = new MethodParameters();

            return methodParameters;
        }

        //Set the Input Properties of the SmartObject Method

        private Validation GetRequiredProperties(ServiceObject serviceObject, string strMethodName)
        {
            Validation validation = new Validation();
            RequiredProperties requiredProperties = new RequiredProperties();

            try
            {

                if (strMethodName.Equals(Resources.K2_Decode_Method_Name))
                {
                    requiredProperties.Add(serviceObject.Properties["InputString"]);
                }

                validation.RequiredProperties = requiredProperties;

                return validation;
            }
            catch (Exception ex)
            {
                throw new Exception("Error GetReturnProperties: " + ex.ToString());
            }
        }

        //Define the Return Properties of the SmartObject Method
        private ReturnProperties GetReturnProperties(ServiceObject serviceObject, string strMethodName)
        {
            ReturnProperties returnProperties = new ReturnProperties();

            if (strMethodName.Equals(Resources.K2_Decode_Method_Name))
            {
                returnProperties.Add(serviceObject.Properties["ReturnString"]);
            }


            return returnProperties;
        }

        //The method that will execute the List and return a DataTable of results
        internal DataTable List(Dictionary<string, object> dicProperties, Dictionary<string, object> dicParameters)
        {
            DataTable dataTable = null;
            DataRow dataRow = null;
            string strInput = string.Empty;

            try
            {
                strInput = dicProperties["InputString"] + string.Empty;
                string[] arr1 = Decrypt(strInput, _HashKey, _SaltKey, _VIKey);
                dataTable = GetDataTable();
                foreach (var item in arr1)
                {
                    try
                    {
                        dataRow = dataTable.NewRow();
                        dataRow["ReturnString"] = item;

                        dataTable.Rows.Add(dataRow);
                    }
                    catch (Exception innerEx)
                    {
                        throw new Exception("Inner Exception: " + innerEx.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception ("Error List: " + ex.ToString());
            }

            return dataTable;
        }

        public static string[] Decrypt(string encryptedText, string PasswordHash, string SaltKey, string VIKey)
        {
            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
                byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
                var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

                var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
                var memoryStream = new MemoryStream(cipherTextBytes);
                var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();

                return new[] { Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray()) };
            }
            catch (Exception ex)
            {
                throw new Exception("Error Decrypt: " + ex);
            }
        }

        //Define the DataTable with its Properties
        private DataTable GetDataTable()
        {
            try
            {
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add(new DataColumn("ReturnString", typeof(String)));

                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception("Error GetDataTable: " + ex);
            }
        }
    }
}
