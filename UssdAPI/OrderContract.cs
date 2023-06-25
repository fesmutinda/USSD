using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace UssdAPI
{
    
        [DataContract]
        public class OrderContract
        {
            [DataMember]
            public string Phonenumber { get; set; }

            [DataMember]
            public string OrgCode { get; set; }

            [DataMember]
            public string Message { get; set; }
        }

        [DataContract]
        public class OrderContract2
        {
            [DataMember]
            public string MemberNo { get; set; }

            [DataMember]
            public string LOANNO { get; set; }

            [DataMember]
            public string phone { get; set; }

            [DataMember]
            public string amount { get; set; }

            [DataMember]
            public string TransactionType { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public string FirstName { get; set; }

            [DataMember]
            public string SecondName { get; set; }

            [DataMember]
            public string Surname { get; set; }

            [DataMember]
            public string IDNo { get; set; }

            //[DataMember]
            //public string Gender { get; set; }

            [DataMember]
            public string SessionID { get; set; }

            [DataMember]
            public string Package { get; set; }

            //[DataMember]
            //public string InvitedBy { get; set; }
            [DataMember]
            public string PackageType { get; set; }

        }

        [DataContract]
        public class verification
        {
            [DataMember]
            public string parameter1 { get; set; }

            [DataMember]
            public string parameter2 { get; set; }

            [DataMember]
            public string parameter3 { get; set; }

            [DataMember]
            public string parameter4 { get; set; }

        }

        [DataContract]
        public class RequestResponse2
        {
            [DataMember]
            public string code { get; set; }

            [DataMember]
            public string message { get; set; }

            [DataMember]
            public string data { get; set; }

        }

        [DataContract]
        public class RegisterContract
        {
            [DataMember]
            public string Firstname { get; set; }

            [DataMember]
            public string Surname { get; set; }

            [DataMember]
            public string IDno { get; set; }

            [DataMember]
            public string MSIDN { get; set; }

            [DataMember]
            public double amount { get; set; }
        }
    
}