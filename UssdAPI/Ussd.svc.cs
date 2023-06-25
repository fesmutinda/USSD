using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using UssdAPI;
using System.Net.Sockets;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Xml;
using System.Threading;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Data;
using System.Runtime.Serialization;
using RestSharp;
namespace UssdAPI
{
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    //[ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple)]


    //[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]


    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class Ussd : IUssd
    {
        public static string InsertSessionRequestSQL = "";
        public static string InsertSessionResponseSQL = "";
        public static string UserExistSQl = "";
        //public static string InsertSessionResponseSQL="";
        public static int Sequence = 0;
        public static string ussdrequesttype = "";
        public static string ussdResponsetype = "";
        public static string ussdSessionSQL = "";
        public static string UserLoginSQL = "";
        public static string UserSaccoSQL = "";
        public static string sqlquery = "";
        public static string responsemsg = "";

        string[] ussdstring_explode;
        int ussdlevel;


        public string checkLogin(string name, string pass)
        {
            //DataAccess dataAccess = new DataAccess();
            return "1";// dataAccess.checkLogin(name, pass);
        }
        public Stream Get(string id)
        {

            string response = "CON Choose your stew: \n1. Chicken Stew\n2. Groundnut Stew";
            OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
            context.ContentType = "text/plain";


            return new System.IO.MemoryStream(ASCIIEncoding.Default.GetBytes(response));
        }

        public Stream getussd(string MSISDN, string session_id, string ussd_string, string service_code)
        {

            SqlDataReader drsession, myreader;
            System.Data.SqlClient.SqlDataReader DR, DR2, DR4, DR7, DR8, DR3;
            Thread.Sleep(200);
            string response = "";
            var url = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.OriginalString;
            string myNum;
            string mydate2 = System.DateTime.Now.ToString("ddMMyyyyHHmmss");
            int ClientCount = 0;
            string OrgCode = "20";

            double flAmount = 0;
            ussd_string = ussd_string.Replace("*98*", "*");
            ussd_string = ussd_string.Replace("*98", "");
            ussd_string = ussd_string.Replace("*0*", "*");
            string Last2 = ussd_string.Substring(ussd_string.Length - 2);
            if (Last2 == "*0")
            {
                ussd_string = ussd_string.Substring(0, ussd_string.Length - 2) + "";
            }
            string ussdstring = "";



            //ussdstring = ussd_string;
            string Message = "";
            double LoanBalance = 0;
            string Content = "";
            string myRep = "";
            string code = "";
            string myF2 = "";

            //string myval2 = "";

            List<string> Rmessage = ussd_string.Split('*').ToList();
            int Count_i = 0;
            int Check = 0;

            for (int i = 0; i < Rmessage.Count; i++)
            {
                if (Rmessage[i].ToString() == "00")
                {
                    Count_i = i;
                }

            }
            if (Count_i != 0)
            {
                int j = 1;
                for (j = 0; j < Count_i - 7; ++j)
                {
                    Rmessage.RemoveAt(7);
                }

                Rmessage.RemoveAt(6);
                Check = Rmessage.Count - 1;
                for (int p = 0; p < Rmessage.Count; p++)
                {
                    if (p < Check)
                    {
                        ussdstring += Rmessage[p].ToString() + "*";
                        ussdstring = ussdstring.Replace("*00", "");
                    }
                    if (p == Check)
                    {
                        ussdstring += Rmessage[p].ToString();
                        ussdstring = ussdstring.Replace("*00", "");
                    }
                }
            }

            if (Count_i == 0)
            {
                Check = Rmessage.Count - 1;
                for (int p = 0; p < Rmessage.Count; p++)
                {
                    if (p < Check)
                    {
                        ussdstring += Rmessage[p].ToString() + "*";
                    }
                    if (p == Check)
                    {
                        ussdstring += Rmessage[p].ToString();
                    }
                }
            }

            Rmessage.Clear();
     
            if (ussdstring != "")
            {
                ussdstring = ussdstring.Replace("#", "*");
                ussdstring_explode = ussdstring.Split('*');
                ussdlevel = ussdstring_explode.Count();
                Sequence = ussdlevel;
            }

            myF2 = MSISDN.Substring(0, 1);
            if (myF2 == "0")
            {
                MSISDN = MSISDN.Remove(0, 1);
                MSISDN = "254" + MSISDN;
            }
            if (myF2 == "+")
            {
                MSISDN = MSISDN.Remove(0, 1);
            }

            try
            {

                responsemsg = "";
                ussdSessionSQL = "";
                ussdSessionSQL = "SELECT TOP 1 * FROM SessionUSSD WHERE (SessionID = '" + session_id + "') AND (Type = 'Response') AND (Mobile = '" + MSISDN + "') ORDER BY ID DESC";
                drsession = new ConnectDB.cConnect().ReadDB(ussdSessionSQL);
                if (drsession.HasRows)
                {
                    drsession.Read();
                    responsemsg = drsession["Message"].ToString();
                    Sequence = Sequence - 1;
                }
                else
                {
                    Sequence = 0;
                }
                drsession.Close();
                drsession.Dispose();


                if (Sequence == 0)
                {
                    ussdrequesttype = "";
                    ussdrequesttype = "Initiation";
                    string Phone = "";
                    Phone = MSISDN.Substring(3, MSISDN.Length - 3);
                    InsertSessionRequestSQL = "";
                    InsertSessionRequestSQL = "INSERT INTO SessionUSSD (Mobile, SessionID, ServiceCode, Type, Message, Operator, Sequence) VALUES     ('" + MSISDN + "', '" + session_id + "', '" + service_code + "', '" + ussdrequesttype + "', '" + ussd_string + "', 'Safaricom', " + Sequence + ")";
                    new ConnectDB.cConnect().WriteDB(InsertSessionRequestSQL);

                    response = "CON Welcome to TUJIPANGE WELFARE\n\n1. Registration(New Member)\n2. Existing Member\n3. Contribution for another member";
                }

                if (Sequence == 1)
                {

                    string ActiveStatus = "";
                    string mnChoice = ussdstring_explode[1].ToString();
                    string MenuName = "";
                    ussdrequesttype = "";
                    ussdrequesttype = "Request";
                    InsertSessionRequestSQL = "";
                    InsertSessionRequestSQL = "INSERT INTO SessionUSSD (Mobile, SessionID, ServiceCode, Type, Message, Operator, Sequence) VALUES     ('" + MSISDN + "', '" + session_id + "', '" + service_code + "', '" + ussdrequesttype + "', '" + ussd_string + "', 'Safaricom', " + Sequence + ")";
                    new ConnectDB.cConnect().WriteDB(InsertSessionRequestSQL);


                    if (mnChoice == "1")//Registration
                    {
                        response = "CON \n1. Register Yourself\n2. Register Another Number";
                    }
                    if (mnChoice == "2")
                    {
                        DR3 = new ConnectDB.cConnect().ReadDB("select top 1 id, Status from main_details where primary_mobile_number = '" + MSISDN + "'");
                        if (DR3.HasRows)
                            while (DR3.Read())
                            {
                                if (DR3["Status"].ToString() == "1")
                                {
                                    response = "CON \n1. Submit Savings\n2. View Member Number\n3. My Account";
                                }
                                if (DR3["Status"].ToString() != "1")
                                {
                                    response = "CON \n1. Reactivate";
                                }
                            }
                        else
                        {
                            response = "END This number has not been registered. Please select the registration option First";
                        }
                        DR3.Close();
                    }
                    if (mnChoice == "3")
                    {
                        response = "CON Please enter the Phone Number\n";
                    }
                }


                if (Sequence == 2)
                {
                    string mnChoice = ussdstring_explode[1].ToString();
                    string mnChoice2 = ussdstring_explode[2].ToString();
                    //string chamaname = ussdstring_explode[2].ToString();
                    string amount=ussdstring_explode[2].ToString();
                    string amount2=ussdstring_explode[2].ToString();                    
                    


                    string MenuName = "";
                    string myamtopt = ussdstring_explode[2].ToString();                    
                    ussdrequesttype = "";
                    ussdrequesttype = "Request";
                    InsertSessionRequestSQL = "";
                    InsertSessionRequestSQL = "INSERT INTO SessionUSSD (Mobile, SessionID, ServiceCode, Type, Message, Operator, Sequence) VALUES     ('" + MSISDN + "', '" + session_id + "', '" + service_code + "', '" + ussdrequesttype + "', '" + ussd_string + "', 'Safaricom', " + Sequence + ")";
                    new ConnectDB.cConnect().WriteDB(InsertSessionRequestSQL);



                    if (mnChoice == "1")//Registration
                    {
                        if (mnChoice2 == "1")
                        {
                            DR3 = new ConnectDB.cConnect().ReadDB("select * from main_details where primary_mobile_number = '" + MSISDN + "'");
                            if (DR3.HasRows)
                            {
                                DR3.Read();
                                response = "END This phone number had already been used to register another member please use another number";
                            }
                            else
                            {
                                response = "CON Welcome to TUJIPANGE WELFARE\nPlease enter Your Firstname\n";
                            }
                            DR3.Close();
                        }
                        if (mnChoice2 == "2")
                        {
                            response = "CON Enter their Phone Number";
                            
                        }
                    }
                    if (mnChoice == "2")
                    {
                        if (mnChoice2 == "2")
                        {
                            DR3 = new ConnectDB.cConnect().ReadDB("select first_name, member_id from main_details where primary_mobile_number = '" + MSISDN + "'");
                            if (DR3.HasRows)
                            {
                                DR3.Read();
                                string first_name = DR3["first_name"].ToString();
                                string member_id = DR3["member_id"].ToString();
                                //string first_name = DR3["first_name"].ToString();
                                string message = "Dear. " + first_name + ". Your MEMBERSHIP NUMBER is " + member_id + ". Dial ...comming soon... to enjoy our Services. TOGETHER, WE CARE";

                                string inDb = "Insert into Messages (Phonenumber,Message, MsgType) values ('" + MSISDN + "','" + message + "','Outbox')";
                                new ConnectDB.cConnect().WriteDB(inDb);

                                response = "END You will recieve an SMS reply Shortly.";
                            }
                            else
                            {
                                response = "END This Number is not Registered with TUJIPANGE WELFARE, Please select registration Option";
                            }
                            DR3.Close();
                        }
                        if (mnChoice2 == "1")
                        {
                            response = "CON Enter Amount";

                        }
                    }
                    if (mnChoice == "3")
                    {
                        response = "CON Enter Amount to Contribute";
                    }                                                                                                                                                                
                }

                if (Sequence == 3)
                {
                    string mnChoice = ussdstring_explode[1].ToString();
                    string mnChoice2 = ussdstring_explode[2].ToString();
                    string confirmstatus = ussdstring_explode[3].ToString();
                    //double amount=Convert.ToDouble(ussdstring_explode[3].ToString());
                    
                    string phonenumber = ussdstring_explode[2].ToString();
                    string membernos="";
                    string chamaid = "";
                    string chamaid2 = ussdstring_explode[3].ToString();
                    chamaid2 = chamaid2.ToUpper();
                    string savings = "";
                    string confirmstatus2 = ussdstring_explode[3].ToString();

                    if (mnChoice == "1")
                    {
                        if (mnChoice2 == "1")
                        {
                            response = "CON Enter your Second Name";
                        }
                        if (mnChoice2 == "2")
                        {
                            response = "CON Enter their First Name";
                        }
                    }
                    if (mnChoice == "2")
                    {
                        double amount = Convert.ToDouble(ussdstring_explode[3].ToString());
                        response = "CON Confirm you want to save Kshs." + amount + " for " + phonenumber + "\n1. Confirm\n2. Cancel";
                        
                    }
                    if (mnChoice == "3")
                    {
                        double amount = Convert.ToDouble(ussdstring_explode[3].ToString());
                        response = "CON Confirm you want to save Kshs." + amount + " for " + phonenumber + "\n1. Confirm\n2. Cancel";
                        
                    }                                        
                }

                if (Sequence == 4)
                {
                    string mnChoice = ussdstring_explode[1].ToString();
                    string mnChoice2 = ussdstring_explode[2].ToString();
                    string savingstarget = ussdstring_explode[4].ToString();
                    //string chamaname = ussdstring_explode[2].ToString();
                    string chamaid = "";
                    string chamaname = ussdstring_explode[3].ToString(); 
                    string confirmstatus=ussdstring_explode[4].ToString(); 
                    string phonenumber=ussdstring_explode[2].ToString(); //nana
                    
                
                    if (mnChoice == "1")//Registration
                    {
                        if (mnChoice2 == "1")
                        {
                            response = "CON Enter your SurName";
                        }
                        if (mnChoice2 == "2")
                        {
                            response = "CON Please provide Second Name";
                        }
                    }

                    if (mnChoice == "2")
                    {
                        if (mnChoice2 == "1")
                        {
                            response = "END Please complete the M-Pesa Transaction";
                        }
                        if (mnChoice2 == "2")
                        {
                            response = "END Transaction Cancelled Successfully";
                        }
                    }

                    if (mnChoice == "3")//Loan
                    {
                        if (mnChoice2 == "1")
                        {
                            response = "END Please complete the M-Pesa Transaction";
                        }
                        if (mnChoice2 == "2")
                        {
                            response = "END Transaction Cancelled Successfully";
                        }
                    }
                }

                if (Sequence == 5)
                {
                    string mnChoice = ussdstring_explode[1].ToString();
                    string mnChoice2 = ussdstring_explode[2].ToString();
                    //string monthlysavingspermember = ussdstring_explode[4].ToString();
                    //string chamaname = ussdstring_explode[2].ToString();

                    if (mnChoice == "1")
                    {
                        if (mnChoice2 == "1")
                        {
                            response = "CON Enter Your National ID";
                        }
                        if (mnChoice2 == "2")
                        {
                            response = "CON Enter Surname";
                        }
                    }                    
                }

                if (Sequence == 6)
                {
                    string mnChoice = ussdstring_explode[1].ToString();
                    string mnChoice2 = ussdstring_explode[2].ToString();                    

                    if (mnChoice == "1")
                    {
                        string national_id = ussdstring_explode[5].ToString();
                        if (mnChoice2 == "1")
                        {
                            myreader = new ConnectDB.cConnect().ReadDB("set dateformat dmy Select national_id from main_details where national_id='" + national_id + "'");
                            if (myreader.HasRows)
                                while (myreader.Read())
                                {
                                    response = "END Sorry, That National ID is Already Registered with TUJIPANGE WELFARE\n Please Visit our Office to Update Your Details";
                                }
                            else
                            {
                                response = "CON Select Your Package \n1. Individual\n2. Couples\n3. Couples + Children\n4. Couples + Children + Parents\n5. Couples + Children + Parents + InLaws ";
                            }
                        }
                        if (mnChoice2 == "2")
                        {
                            response = "CON Enter the National ID";
                        }
                    }                    
                }

                if (Sequence == 7)
                {
                    string mnChoice = ussdstring_explode[1].ToString();
                    string mnChoice2 = ussdstring_explode[2].ToString();
                    

                    if (mnChoice == "1")
                    {
                        if (mnChoice2 == "1")
                        {
                            response = "CON Select Your Age Category \n1. Below 70 Years\n2. 71 - 80 Years\n3. 81 - 90 Years\n4. 91 - 100 Years";
                        }
                        if (mnChoice2 == "2")
                        {
                            response = "CON Select Your Package \n1. Individual\n2. Couples\n3. Couples + Children\n4. Couples + Children + Parents\n5. Couples + Children + Parents + InLaws ";
                        }

                    }
                    
                }


                if (Sequence == 8)
                {
                    string mnChoice = ussdstring_explode[1].ToString();
                    string mnChoice2 = ussdstring_explode[2].ToString();

                    string PackageType = ussdstring_explode[7].ToString();
                    string AgesCategory = ussdstring_explode[8].ToString();
                    string description = "", dr_description = "", dr_age = "";

                    if (PackageType == "1")
                    {
                        dr_description = "INDIVIDUAL";
                    }
                    if (PackageType == "2")
                    {
                        dr_description = "COUPLES";
                    }
                    if (PackageType == "3")
                    {
                        dr_description = "COUPLES WITH CHILDREN";
                    }
                    if (PackageType == "4")
                    {
                        dr_description = "COUPLES WITH CHILDREN & PARENTS";
                    }
                    if (PackageType == "5")
                    {
                        dr_description = "COUPLES WITH CHILDREN & PARENTS & INLAWS";
                    }
                    //now get the ages.....
                    if (AgesCategory == "1")
                    {
                        dr_age = "BELOW 70";
                    }
                    if (AgesCategory == "2")
                    {
                        dr_age = "71 - 80";
                    }
                    if (AgesCategory == "3")
                    {
                        dr_age = "81 - 90";
                    }
                    if (AgesCategory == "4")
                    {
                        dr_age = "91 - 100";
                    }

                    if (mnChoice == "1")//Registration
                    {
                        if (mnChoice2 == "1")//Register chama
                        {
                            response = "CON Confirm That you Want to Register Under "+ dr_description + " "+dr_age+" Membership, You will be Charged Ksh.200 as Registration Fee \n1. Confirm\n2. Cancel";
                        }
                        if (mnChoice2 == "2")//Register members to chama
                        {
                            response = "CON Select Your Age Category \n1. Below 70 Years\n2. 71 - 80 Years\n3. 81 - 90 Years\n4. 91 - 100 Years";
                        }
                    }                   
                }
                if (Sequence == 9)
                {
                    string PackageType = "", AgesCategory;
                    string mnChoice = ussdstring_explode[1].ToString();
                    string mnChoice2 = ussdstring_explode[2].ToString();

                    string phonenumber = ussdstring_explode[7].ToString();

                    string first_name = ussdstring_explode[3].ToString().ToUpper();
                    string middle_name = ussdstring_explode[4].ToString().ToUpper();
                    string surname = ussdstring_explode[5].ToString().ToUpper();
                    string national_id = ussdstring_explode[6].ToString();
                    PackageType = ussdstring_explode[7].ToString();
                    AgesCategory = ussdstring_explode[8].ToString();

                    string member_id = "";
                    string primary_mobile_number = MSISDN;
                    string date_created = System.DateTime.Now.ToString("dd/MM/yyyy");
                    
                    string MonthlyContribution = "";
                    string description = "", dr_description = "", dr_age = "";
                    int num2 = 0;

                    if (mnChoice == "1")
                    {
                        string Confirmation = ussdstring_explode[9].ToString();
                        if (mnChoice2 == "1")
                        {
                            if (Confirmation == "1")
                            {
                                //\n1. Individual\n2. Couples\n3. Couples + Children\n4. Couples + Children + Parents\n5. Couples + Children + Parents + InLaws ";                                
                                if (PackageType == "1")
                                {
                                    dr_description = "INDIVIDUAL";
                                }
                                if (PackageType == "2")
                                {
                                    dr_description = "COUPLES";
                                }
                                if (PackageType == "3")
                                {
                                    dr_description = "COUPLES WITH CHILDREN";
                                }
                                if (PackageType == "4")
                                {
                                    dr_description = "COUPLES WITH CHILDREN & PARENTS";
                                }
                                if (PackageType == "5")
                                {
                                    dr_description = "COUPLES WITH CHILDREN & PARENTS & INLAWS";
                                }

                                //now get the ages.....
                                if (AgesCategory == "1")
                                {
                                    dr_age = "BELOW 70";
                                }
                                if (AgesCategory == "2")
                                {
                                    dr_age = "71 - 80";
                                }
                                if (AgesCategory == "3")
                                {
                                    dr_age = "81 - 90";
                                }
                                if (AgesCategory == "4")
                                {
                                    dr_age = "91 - 100";
                                }                                

                                myreader = new ConnectDB.cConnect().ReadDB("Select top 1 member_id from main_details order by id desc");
                                if (myreader.HasRows)
                                    while (myreader.Read())
                                    {
                                        member_id = myreader["member_id"].ToString();
                                        member_id = member_id.Replace("TC", "");
                                        num2 = Convert.ToInt32(member_id);
                                        num2 = num2 + 1;
                                        member_id = "TC" + num2.ToString().PadLeft(7, '0');
                                    }
                                else
                                {
                                    member_id = "TC0000001";
                                }
                                myreader.Close();

                                myreader = new ConnectDB.cConnect().ReadDB("Select innitials,Monthly From MembershipType where Description='" + dr_description.TrimStart().TrimEnd() + "' and Age='" + dr_age.TrimStart().TrimEnd() + "'");
                                if (myreader.HasRows)
                                {
                                    myreader.Read();
                                    PackageType = myreader["innitials"].ToString();
                                    MonthlyContribution = myreader["Monthly"].ToString();
                                }
                                myreader.Close();

                                string indDb = "set dateformat dmy Insert into main_details (member_id, national_id, surname, middle_name, first_name, primary_mobile_number,creator, date_created, Channel, PackageType, MonthlyContribution,Status,MembershipStatus) values ('" + member_id + "', '" + national_id + "', '" + surname + "', '" + middle_name + "', '" + first_name + "', '" + primary_mobile_number + "','MYSELF', '" + date_created + "', 'USSD', '" + PackageType + "', '" + MonthlyContribution + "', '1','INACTIVE')";

                                new ConnectDB.cConnect().WriteDB(indDb);//17.21

                                string message = "Dear. " + first_name + ". You have successfully been registered as a TUJIPANGE WELFARE " + description + " MEMBER. Your MEMBERSHIP NUMBER is " + member_id + ". Dial ...comming soon... to enjoy our Services. TOGETHER, WE CARE";

                                string inDb = "Insert into Messages (Phonenumber,Message, MsgType) values ('" + primary_mobile_number + "','" + message + "','Outbox')";
                                new ConnectDB.cConnect().WriteDB(inDb);


                                response = "END Please supply your MPESA PIN on the screen that will pop up";
                            }
                            if (Confirmation == "2")
                            {
                                response = "END Your registration request has been cancelled successfully.";
                            }
                        }
                        if (mnChoice2 == "2")
                        {
                            response = "CON Confirm That you Want to Register Under COUPLES Below 70 Years Membership, You will be Charged Ksh.200 as Registration Fee \n1. Confirm\n2. Cancel";
                        }

                    }

                }
                if (Sequence == 10)
                {
                    string mnChoice = ussdstring_explode[1].ToString();
                    string mnChoice2 = ussdstring_explode[2].ToString();
                    string phonenumber = ussdstring_explode[7].ToString();
                    string idnumber = ussdstring_explode[8].ToString();
                    string firstname = ussdstring_explode[4].ToString();
                    string secondname = ussdstring_explode[5].ToString();
                    string surname = ussdstring_explode[6].ToString();
                    string chamaid = ussdstring_explode[3].ToString();
                    chamaid = chamaid.ToUpper();
                    string memberno = "";

                    if (mnChoice == "1")
                    {
                        string Confirmation = ussdstring_explode[10].ToString();
                        if (mnChoice2 == "1")
                        {
                            
                        }
                        if (mnChoice2 == "2")
                        {
                            if (Confirmation == "1")
                            {
                                response = "END Please supply your MPESA PIN on the screen that will pop up";
                            }
                            if (Confirmation == "2")
                            {
                                response = "END Your registration request has been cancelled successfully.";
                            }
                        }

                    }

                }
            }

            catch (ChannelTerminatedException cteex)
            {
                cteex.Data.Clear();
                response = "END Request not Complete,Try again later";
            }
            catch (EndpointNotFoundException endpointex)
            {
                endpointex.Data.Clear();
                response = "END Request not Complete,Try again later";
            }
            catch (ServerTooBusyException serverbusyex)
            {
                serverbusyex.Data.Clear();
                response = "END Request not Complete,Try again later";
            }
            catch (TimeoutException timeoutex)
            {
                timeoutex.Data.Clear();
                response = "END Request not Complete,Try again later";
            }
            catch (CommunicationException comex)
            {
                comex.Data.Clear();
                response = "END Request not Complete,Try again later";
            }

            catch (Exception ex)
            {
                ex.Data.Clear();
                response = "END Request not Complete,Try again later";
            }

            finally
            {
                //=======RESPONSE======
                ussdResponsetype = "";
                ussdResponsetype = "Response";
                InsertSessionResponseSQL = "";
                InsertSessionResponseSQL = "INSERT INTO SessionUSSD (Mobile, SessionID, ServiceCode, Type, Message, Operator, Sequence) VALUES     ('" + MSISDN + "', '" + session_id + "', '" + service_code + "', '" + ussdResponsetype + "', '" + response + "', 'Safaricom', " + Sequence + ")";
                new ConnectDB.cConnect().WriteDB(InsertSessionResponseSQL);
                MSISDN = "";
                session_id = "";
                ussd_string = "";
                service_code = "";
            }
            //}
            ////Hapa
            //if (service_code != "210")
            //{
            //    response = "END You are not allowed to use this serviceee";
            //}

            OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
            context.ContentType = "text/plain";
            return new System.IO.MemoryStream(ASCIIEncoding.Default.GetBytes(response));

        } // End Here

        public string SendToMPESA(string OrgCode, string PhoneNo, string SessionID, string MemberNo, string TransType, double Amount, string AuditDate, string FirstName, string SecondName, string Surname, string national_id, string Phoneto, string LoanNo, string ChamaID)
        {
            string repfromMpesa = "";
            string RespCode = "0";
            string myResponse = "";
            try
            {
                repfromMpesa = SendLipaNaMpesaRequest2(PhoneNo, Amount, MemberNo);
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(MyDetail2));
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(repfromMpesa));
                stream.Position = 0;
                MyDetail2 dataContractDetail = (MyDetail2)jsonSerializer.ReadObject(stream);
                RespCode = dataContractDetail.ResponseCode;

                if (RespCode.TrimEnd().TrimStart() == "0")
                {
                    string indb = "set dateformat dmy Insert into MpesaLoanRep2 (MemberNo, Description, Amount, Trasdate, AuditId, PhoneNumber,TrasType, FirstName, SecondName, Surname, national_id, Phoneto, LoanNo, OrgCode, SessionID, originid, CheckoutRequestID, Status2, chamaid) values ('" + MemberNo + "','" + TransType + "','" + Amount + "','" + System.DateTime.Today + "','" + PhoneNo + "','" + PhoneNo + "','" + TransType + "', '" + FirstName + "', '" + SecondName + "', '" + Surname + "', '" + national_id + "','" + Phoneto + "','" + LoanNo + "', '" + OrgCode + "', '" + SessionID + "', '" + dataContractDetail.MerchantRequestID + "', '" + dataContractDetail.CheckoutRequestID + "', 'DONE','"+ChamaID+"')";
                    //string indb = "set dateformat dmy Insert into MpesaLoanRep (MemberNo, Description, Amount, Trasdate, AuditId, PhoneNumber,originid,TrasType, FirstName, SecondName, Surname, national_id, Phoneto) values ('" + MemberNo + "','" + TransType + "','" + Amount + "','" + System.DateTime.Today + "','" + PhoneNo + "','" + PhoneNo + "','" + dataContractDetail.MerchantRequestID + "','" + TransType + "', '" + FirstName + "', '" + SecondName + "', '" + Surname + "', '" + national_id + "','" + Phoneto + "')";
                    new ConnectDB.cConnect().WriteDB(indb);
                    myResponse = "END Please supply your MPESA PIN on the screen that will pop up";
                }
                if (RespCode.TrimEnd().TrimStart() != "0")
                {
                    myResponse = "END Payment Unsuccessful Please try again Later";
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
                myResponse = "END Payment Unsuccessful Please try again Later";
            }

            return myResponse;
        }

        public static string SendLipaNaMpesaRequest2(string phon, double MepAmount, string LoanNo)              //(string msidn, double amount, int OrgCode)
        {
            System.Data.SqlClient.SqlDataReader LR;
            string myAcessToken = "";
            string mimi3 = "";
            string Message = "";
            string consumerKey = "", consumerSecret = "";
            string PassKey = "", ShortCode = "", BusinessShortCode2 = "";
            string CallBackURL = "";
            //response=null;
            try
            {
                LR = new ConnectDB.cConnect().ReadDB("set dateformat dmy select top 1 OrgCode,SaccoIP,port,BusinessShortCode,Password,CallBackURL,consumerKey,consumerSecret,PassKey, TransactionType,BusinessShortCode2 from C2BOrgs order by ID asc");
                if (LR.HasRows)
                {
                    LR.Read();
                    ShortCode = LR["BusinessShortCode"].ToString();
                    CallBackURL = LR["CallBackURL"].ToString();
                    consumerKey = LR["consumerKey"].ToString();
                    consumerSecret = LR["consumerSecret"].ToString();
                    PassKey = LR["PassKey"].ToString();
                    BusinessShortCode2 = LR["BusinessShortCode2"].ToString();
                    LR.Close();
                }

                //string mimi = GenerateSafaricomToken("jVtAeO2rwJYUVY42sVzyAHcIOi2qMTUJ", "MlrzfFQ39SYNe3Tv");
                string mimi = GenerateSafaricomToken(consumerKey, consumerSecret);
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(MyDetail));
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(mimi));
                stream.Position = 0;
                MyDetail dataContractDetail = (MyDetail)jsonSerializer.ReadObject(stream);
                myAcessToken = dataContractDetail.access_token;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                string Timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                string mypass = ShortCode + PassKey + Timestamp;
                string myencPass = Base64Encode(mypass);
                Console.WriteLine(Timestamp);

                var client = new RestClient("https://api.safaricom.co.ke/mpesa/stkpush/v1/processrequest");
                var request = new RestRequest(Method.POST);
                request.Parameters.Clear();
                //request.AddHeader("Postman-Token", "a4f49c55-83aa-bafb-8c9e-01cfe6f19e60");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + myAcessToken);
                request.AddParameter("application/json", "{\r\n    \"BusinessShortCode\": \"" + ShortCode + "\",\r\n    \"Password\": \"" + myencPass + "\",\r\n    \"Timestamp\": \"" + Timestamp + "\",\r\n    \"TransactionType\": \"CustomerBuyGoodsOnline\",\r\n    \"Amount\": \"" + MepAmount + "\",\r\n    \"PartyA\": \"" + phon + "\",\r\n    \"PartyB\": \"" + BusinessShortCode2 + "\",\r\n    \"PhoneNumber\": \"" + phon + "\",\r\n    \"CallBackURL\": \"" + CallBackURL + "\",\r\n    \"AccountReference\": \"" + LoanNo + "\",\r\n    \"TransactionDesc\": \"MPESA DEPOSITS\"\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                mimi3 = response.Content;
                Console.WriteLine(mimi3);

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
                Console.WriteLine(ex.Message);
            }
            return mimi3;
        }

        public static string GenerateSafaricomToken(string consumerKey, string consumerSecret)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            RestClient restClient = new RestClient
            {
                BaseUrl = new Uri("https://api.safaricom.co.ke").ToString(),
                Authenticator = new HttpBasicAuthenticator(consumerKey, consumerSecret)
            };
            //client_credentials
            RestRequest request = new RestRequest("/oauth/v1/generate", Method.GET);
            request.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);
            IRestResponse restResponse = restClient.Execute(request);
            if (restResponse != null && !string.IsNullOrEmpty(restResponse.Content))
            {
                return restResponse.Content;
            }
            return string.Empty;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public void loadfrommessageein(string PhoneNumber, string Orgode, string msg)
        {
            try
            {
                //SaccoCode, PhoneNumber, Message, msgstatus
                OrderContract order = new OrderContract
                {
                    Phonenumber = PhoneNumber,
                    OrgCode = Orgode,
                    Message = msg
                };

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(OrderContract));
                MemoryStream mem = new MemoryStream();
                ser.WriteObject(mem, order);
                string data = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                WebClient webClient = new WebClient();
                webClient.Headers["Content-type"] = "application/json";
                webClient.Encoding = Encoding.UTF8;
                string response = webClient.UploadString("http://198.38.85.25:8093/SendMessage", "POST", data);
                //string indb = "set dateformat dmy Insert into Swift_Messages (SaccoCode, PhoneNumber, Message,msgstatus) values ('" + Orgode + "','" + PhoneNumber + "','" + msg + "','1')";
                //new WARTECHCONNECTION.cConnect().WriteDB(indb);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
                //string indb = "set dateformat dmy Insert into Swift_Messages (SaccoCode, PhoneNumber, Message,msgstatus) values ('" + Orgode + "','" + PhoneNumber + "','" + msg + "','0')";
                //new WARTECHCONNECTION.cConnect().WriteDB(indb);
            }
        }

        public Stream getussd2(string sessionId, string serviceCode, string phoneNumber, string text)
        {

            string response = "CON MCommerce\n\n1. Register Customer\n2. Dispatch Order\n3. View Commission";
            OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
            context.ContentType = "text/plain";


            return new System.IO.MemoryStream(ASCIIEncoding.Default.GetBytes(response));
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }

    [DataContract]
    public class MyDetail
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string expires_in { get; set; }
    }

    [DataContract]
    public class MyDetail2
    {
        [DataMember]
        public string MerchantRequestID { get; set; }
        [DataMember]
        public string CheckoutRequestID { get; set; }
        [DataMember]
        public string ResponseDescription { get; set; }
        [DataMember]
        public string ResponseCode { get; set; }
        [DataMember]
        public string CustomerMessage { get; set; }

    }
}
