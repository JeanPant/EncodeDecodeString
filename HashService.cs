using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

//SourceCode References
using SourceCode.SmartObjects.Services.ServiceSDK;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;

using HashService;

namespace K2HashServiceService
{
    class K2HashService : ServiceAssemblyBase
    {
        private const string _HashKey = "P@@Sw0rd";
        private const string _SaltKey = "S@LT&KEY";
        private const string _VIKey = "@1B2c3D4e5F6g7H8";


        //Define the ServiceInstance Properties
        public override string GetConfigSection()
        {
            base.Service.ServiceConfiguration.Add("HashKey", _HashKey);
            base.Service.ServiceConfiguration.Add("SALTKey", _SaltKey);
            base.Service.ServiceConfiguration.Add("VIKey", _VIKey);

            return base.GetConfigSection();
        }

       
        //Define the properties of the ServiceInstance based on the values from the Resource File
        public override string DescribeSchema()
        {
            string strHashKey = string.Empty;
            string strSaltKey = string.Empty;
            string strVIKey = string.Empty;

            EncodeMethod encode = null;
            DecodeMethod decode = null;

            base.Service.Name = Resources.K2_Name;
            base.Service.MetaData.DisplayName = Resources.K2_DisplayName;
            base.Service.MetaData.Description = Resources.K2_Description;

            strHashKey = base.Service.ServiceConfiguration["HashKey"] + string.Empty;
            strSaltKey = base.Service.ServiceConfiguration["SALTKey"] + string.Empty;
            strVIKey = base.Service.ServiceConfiguration["VIKey"] + string.Empty;

            encode = new EncodeMethod(strHashKey, strSaltKey, strVIKey);
            base.Service.ServiceObjects.Create(encode.DescribeServiceObject());
            encode.Dispose();
            encode = null;

            decode = new DecodeMethod(strHashKey, strSaltKey, strVIKey);
            base.Service.ServiceObjects.Create(decode.DescribeServiceObject());
            decode.Dispose();
            decode = null;

            return base.DescribeSchema();
        }

        public override void Extend()
        {
            throw new NotImplementedException();
        }

        //Set the Properties of the ServiceInstance based on the values entered.

        public override void Execute()
        {
            string strHashKey = string.Empty;
            string strSaltKey = string.Empty;
            string strVIKey = string.Empty;

            Dictionary<string, object> dicProperties = new Dictionary<string, object>();
            Dictionary<string, object> dicParameters = new Dictionary<string, object>();

            EncodeMethod encode = null;
            DecodeMethod decode = null;

            base.ServicePackage.ResultTable = null;

            try
            {
                strHashKey = base.Service.ServiceConfiguration["HashKey"] + string.Empty;
                strSaltKey = base.Service.ServiceConfiguration["SALTKey"] + string.Empty;
                strVIKey = base.Service.ServiceConfiguration["VIKey"] + string.Empty;

                foreach (ServiceObject serviceObject in base.Service.ServiceObjects)
                {
                    foreach (Method method in serviceObject.Methods)
                    {
                        foreach (Property property in serviceObject.Properties)
                        {
                            if ((property.Value != null) && (!string.IsNullOrEmpty(property.Value + string.Empty)))
                            {
                                dicProperties.Add(property.Name, property.Value);
                            }
                        }
                        foreach (MethodParameter methodParameter in method.MethodParameters)
                        {
                            if ((methodParameter.Value != null) && (!string.IsNullOrEmpty(methodParameter.Value + string.Empty)))
                            {
                                dicParameters.Add(methodParameter.Name, methodParameter.Value);
                            }
                        }

                        if (serviceObject.Name.Equals(Resources.K2_Decode_ServiceObject_Name))
                        {
                            decode = new DecodeMethod(strHashKey, strSaltKey, strVIKey);

                            if (method.Name.Equals(Resources.K2_Decode_Method_Name))
                            {
                                base.ServicePackage.ResultTable = decode.List(dicProperties, dicParameters);
                            }

                        }
                        else if (serviceObject.Name.Equals(Resources.K2_Encode_ServiceObject_Name))
                        {
                            encode = new EncodeMethod(strHashKey, strSaltKey, strVIKey);

                            if (method.Name.Equals(Resources.K2_Encode_Method_Name))
                            {
                                base.ServicePackage.ResultTable = encode.List(dicProperties, dicParameters);
                            }

                        }
                    }
                }
                base.ServicePackage.IsSuccessful = true;
            }
            catch (Exception exp)
            {
                base.ServicePackage.IsSuccessful = false;
                base.ServicePackage.ServiceMessages.Add(new ServiceMessage(exp.Message, MessageSeverity.Error));

                throw new Exception("Error Execute: " + exp.Message);
            }
        }
    }
}
