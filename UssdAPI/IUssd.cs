using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Linq;
using System.IO;

namespace UssdAPI
{
    [ServiceContract]
    //ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IUssd
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "checkLogin?name={name}&pass={pass}")]
        string checkLogin(string name, string pass);

        [OperationContract]
        [WebGet(UriTemplate = "{id}", 
            BodyStyle = WebMessageBodyStyle.Bare)]
        Stream Get(string id);

        [OperationContract]

        [WebInvoke(Method = "GET",

            BodyStyle = WebMessageBodyStyle.Bare,
            //UriTemplate = "GetUssd?sessionId={sessionId}&ServiceCode={ServiceCode}&phoneNumber={phoneNumber}&text={text}")]
        //UriTemplate = "GetUssd?sessionId={sessionId}&ServiceCode={ServiceCode}&phoneNumber={phoneNumber}&text={text}")]
        UriTemplate = "GetUssd?MSISDN={MSISDN}&session_id={session_id}&ussd_string={ussd_string}&service_code={service_code}")
        ]


        Stream getussd(string MSISDN, string session_id, string ussd_string, string service_code);
        [WebInvoke(Method = "GET",

            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "GetUssd2?sessionId={sessionId}&ServiceCode={ServiceCode}&phoneNumber={phoneNumber}&text={text}")]
        Stream getussd2(string sessionId, string serviceCode, string phoneNumber, string text);
    }
}