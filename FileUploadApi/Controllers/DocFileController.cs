﻿using FileUploadApi.Models;
using IronPdf;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;


namespace FileUploadApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-Custom-Header", SupportsCredentials = true)]
    public class DocFileController : ApiController
    {
       
        [HttpPost]
        public List<FinalFileData> DataExtraction(string authKey)
        {
            try
            {
                //string authKey= "10005-P10225-10000";
                string patientName = "";
                string patientAddress = "";
                string facility = "";
                string[] dateDOB = null;
                string[] phone = null;
                string[] authorsList = null;
                string[] patList = null;
                string[] dataExtract;
                List<FinalFileData> fileData = new List<FinalFileData>();
                List<PatientData> data = new List<PatientData>();
                if (authKey == "10005-P10225-10000")
                {
                    int fileType = 0;
                    int cc = HttpContext.Current.Request.Files.Count;
                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        var file = HttpContext.Current.Request.Files.Count > 0 ?
                            HttpContext.Current.Request.Files[i] : null;
                        if (file != null && file.ContentLength > 0)
                        {

                            using (PdfReader reader = new PdfReader(file.InputStream))
                            {
                                for (int j = 1; j <= reader.NumberOfPages; j++)
                                {                                    
                                        bool isCorrect = false;
                                        ITextExtractionStrategy its = new SimpleTextExtractionStrategy();
                                        string filereader = PdfTextExtractor.GetTextFromPage(reader, j, its);
                                        
                                        string[] dataExtractParent = filereader.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                                        if (dataExtractParent[0].Contains("MPS CCC"))
                                        {
                                            fileType = 3;
                                        }
                                        if (dataExtractParent[0].Contains("Profile Summary"))
                                            {
                                            fileType = 1;
                                        }
                                        if (dataExtractParent[0].Contains("Patient Profile"))
                                        {
                                            fileType = 2;
                                        }
                                    if (fileType == 1)
                                    {

                                        dataExtract = filereader.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                                        if (dataExtract[0].Contains("Profile"))
                                        {
                                            isCorrect = true;
                                            var data4 = dataExtract[4];
                                            var data5 = dataExtract[5];
                                            authorsList = dataExtract[0].Split(',');
                                            facility = dataExtract[2];
                                            patientAddress = dataExtract[3];
                                            patList = authorsList[1].Split('-');
                                            if (data4.Contains("DOB"))
                                            {
                                                string[] DOB = data4.Split(new string[] { "Sex" }, StringSplitOptions.None);
                                                dateDOB = DOB[0].Split(':');
                                                patientName = dataExtract[5].Substring(0, dataExtract[5].IndexOf("-"));
                                                phone = dataExtract[5].Split('-');
                                                if (dataExtract[6] != "Allergies")
                                                {
                                                    phone[0] = phone[1];
                                                    phone[1] = dataExtract[6];
                                                }
                                                else
                                                {
                                                    phone[0] = phone[1];
                                                    phone[1] = phone[2];
                                                }
                                            }
                                            else
                                            {
                                                string[] DOB = data5.Split(new string[] { "Sex" }, StringSplitOptions.None);
                                                dateDOB = DOB[0].Split(':');
                                                patientAddress = dataExtract[3] + " " + dataExtract[4];
                                                if (dataExtract[6].Contains("Dr"))
                                                {

                                                    patientName = dataExtract[6].Substring(0, dataExtract[6].IndexOf("-"));
                                                    phone = dataExtract[6].Split('-');
                                                    if (dataExtract[7] != "Allergies")
                                                    {
                                                        phone[0] = phone[1];
                                                        phone[1] = dataExtract[7];
                                                    }
                                                    else
                                                    {
                                                        phone[0] = phone[1];
                                                        phone[1] = phone[2];
                                                    }
                                                }

                                            }
                                        }
                                        else if (dataExtract[1].Contains("Profile"))
                                        {
                                            isCorrect = true;
                                            var data5 = dataExtract[5];
                                            var data6 = dataExtract[6];
                                            facility = dataExtract[3];
                                            patientAddress = dataExtract[4];
                                            authorsList = dataExtract[1].Split(',');
                                            patList = authorsList[1].Split('-');
                                            if (data5.Contains("DOB"))
                                            {
                                                string[] DOB = data5.Split(new string[] { "Sex" }, StringSplitOptions.None);
                                                dateDOB = DOB[0].Split(':');
                                                patientName = dataExtract[6].Substring(0, dataExtract[6].IndexOf("-"));
                                                phone = dataExtract[6].Split('-');
                                                if (dataExtract[7] != "Allergies")
                                                {
                                                    phone[0] = phone[1];
                                                    phone[1] = dataExtract[7];
                                                }
                                                else
                                                {
                                                    phone[0] = phone[1];
                                                    phone[1] = phone[2];
                                                }
                                            }
                                            else
                                            {
                                                string[] DOB = data6.Split(new string[] { "Sex" }, StringSplitOptions.None);
                                                dateDOB = DOB[0].Split(':');
                                                patientAddress = dataExtract[4] + " " + dataExtract[5];
                                                if (dataExtract[7].Contains("Dr"))
                                                {
                                                    patientName = dataExtract[7].Substring(0, dataExtract[7].IndexOf("-"));
                                                    phone = dataExtract[7].Split('-');
                                                    if (dataExtract[8] != "Allergies")
                                                    {
                                                        phone[0] = phone[1];
                                                        phone[1] = dataExtract[8];

                                                    }
                                                    else
                                                    {
                                                        phone[0] = phone[1];
                                                        phone[1] = phone[2];
                                                    }

                                                }

                                            }
                                        }
                                        if (isCorrect == true)
                                        {
                                            string phoneNumber = "";
                                            if (phone != null)
                                            {
                                                phoneNumber = phone[0] + phone[1];
                                            }
                                            //string DateoFbirth = Convert.ToDateTime(dateDOB[1].Trim()).ToString("dd/MM/yyyy");
                                            //string[] DateBith = DateoFbirth.ToString().Split(new string[] { " " }, StringSplitOptions.None);
                                            data.Add(
                                                    new PatientData
                                                    {
                                                        LastName = authorsList[0].Trim(),
                                                        FirstName = patList[0].Trim(),
                                                        Facility = facility.Trim(),
                                                        Address = patientAddress.Trim(),
                                                        DOB = dateDOB[1].Trim(),
                                                        PatientDr = patientName.Trim(),
                                                        //PhoneNumber=phoneNumber.Trim()

                                                    });



                                        }
                                    }
                                    else if (fileType == 2)
                                    {

                                        filereader = (PdfTextExtractor.GetTextFromPage(reader, j));
                                        string[] extractData = filereader.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                                        if (extractData[6].Contains("Sex"))
                                        {
                                            var data5 = extractData[5];
                                            string[] mcare = extractData[9].Split(':');
                                            patList = extractData[3].Split(',');
                                            string[] DOB = data5.Split(new string[] { "Sex" }, StringSplitOptions.None);
                                            string[] patientDr = extractData[16].Split(new string[] { "Page" }, StringSplitOptions.None);
                                            dateDOB = DOB[0].Split(':');
                                            string DateoFbirth = Convert.ToDateTime(dateDOB[1].Trim()).ToString("dd/MM/yyyy");
                                            //string[] DateBith = DateoFbirth.ToString().Split(new string[] { " " }, StringSplitOptions.None);
                                            data.Add(
                                                new PatientData
                                                {
                                                    LastName = patList[0].Trim(),
                                                    FirstName = patList[1].Trim(),
                                                    Facility = extractData[12].Trim(),
                                                    MedicareNumber = mcare[1].Trim(),
                                                    DOB = DateoFbirth.Trim(),
                                                    PatientDr = patientDr[0].Trim()
                                                });
                                        }
                                        else
                                        {
                                            var data5 = extractData[5];
                                            string[] mcare = extractData[8].Split(':');
                                            patList = extractData[3].Split(',');
                                            string[] DOB = data5.Split(new string[] { "Sex" }, StringSplitOptions.None);
                                            string[] patientDr = extractData[14].Split(new string[] { "Page" }, StringSplitOptions.None);
                                            dateDOB = DOB[0].Split(':');
                                            string DateoFbirth = Convert.ToDateTime(dateDOB[1].Trim()).ToString("dd/MM/yyyy");
                                            //string[] DateBith = DateoFbirth.ToString().Split(new string[] { " " }, StringSplitOptions.None);
                                            data.Add(
                                                new PatientData
                                                {
                                                    LastName = patList[0].Trim(),
                                                    FirstName = patList[1].Trim(),
                                                    Facility = extractData[11].Trim(),
                                                    MedicareNumber = mcare[1].Trim(),
                                                    DOB = DateoFbirth.Trim(),
                                                    PatientDr = patientDr[0].Trim()
                                                });
                                        }

                                    }
                                    else if (fileType == 3)
                                    {
                                        filereader = (PdfTextExtractor.GetTextFromPage(reader, j));
                                        //filereader =filereader.Replace("\n", "");
                                        string[] extractData = filereader.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                                        if (extractData[0] == " " && extractData.Count()>15)
                                        {
                                            if (extractData[0] == " " && extractData[2] == " ")
                                            {
                                                string[] mcare = { "" };
                                                if (extractData[15] != " " && extractData[15] != "  ")
                                                {
                                                    var data5 = extractData[15];
                                                    mcare = extractData[15].Split(':');
                                                    var patientAddress1 = mcare[1].Split(new string[] { " DOB" }, StringSplitOptions.None);


                                                    patList = extractData[4].Split(' ');
                                                    string[] DOB = mcare[2].Split(new string[] { " Medicare" }, StringSplitOptions.None);
                                                    string[] patientDr = { "" };
                                                    string DoctorName = "";
                                                    if (extractData[29].Contains("Doctor Name"))
                                                    {
                                                        patientDr = extractData[29].Split(':');
                                                        DoctorName = patientDr[2];
                                                    }
                                                    if (extractData[30].Contains("Doctor Name"))
                                                    {
                                                        patientDr = extractData[30].Split(':');
                                                        DoctorName = patientDr[1];
                                                    }

                                                    dateDOB = DOB[0].Split(':');
                                                    string DateoFbirth = Convert.ToDateTime(DOB[0].Trim()).ToString("dd/MM/yyyy");
                                                    data.Add(
                                                        new PatientData
                                                        {
                                                            LastName = patList[0].Trim(),
                                                            FirstName = patList[1].Trim(),
                                                            Address = patientAddress1[0].Trim(),
                                                            MedicareNumber = mcare[3].Trim(),
                                                            DOB = DateoFbirth.Trim(),
                                                            PatientDr = DoctorName.Trim()
                                                        });
                                                }
                                            }
                                            if (extractData[0] == " " && extractData[2] != " ")
                                            {

                                                var data5 = extractData[9];
                                                string[] mcare = extractData[9].Split(':');
                                                var patientAddress1 = mcare[1].Split(new string[] { " DOB" }, StringSplitOptions.None);
                                                patList = extractData[2].Split(' ');
                                                string[] DOB = mcare[2].Split(new string[] { " Medicare" }, StringSplitOptions.None);
                                                string[] patientDr = { "" };
                                                string DoctorName = "";
                                                if (extractData[15].Contains("Doctor Name"))
                                                {
                                                    patientDr = extractData[15].Split(':');
                                                    DoctorName = patientDr[2];
                                                }
                                                if (extractData[16].Contains("Doctor Name"))
                                                {
                                                    patientDr = extractData[16].Split(':');
                                                    if (patientDr.Count() > 2)
                                                    {
                                                        DoctorName = patientDr[2];
                                                    }
                                                    else
                                                    {
                                                        DoctorName = patientDr[1];
                                                    }
                                                }

                                                dateDOB = DOB[0].Split(':');
                                                string DateoFbirth = Convert.ToDateTime(DOB[0].Trim()).ToString("dd/MM/yyyy");
                                                data.Add(
                                                    new PatientData
                                                    {
                                                        LastName = patList[0].Trim(),
                                                        FirstName = patList[1].Trim(),
                                                        Address = patientAddress1[0].Trim(),
                                                        MedicareNumber = mcare[3].Trim(),
                                                        DOB = DateoFbirth.Trim(),
                                                        PatientDr = DoctorName.Trim()
                                                    });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("File is not in correct formate");

                                    }
                                    }
                               
                            }                          
                        }

                    }

                    fileData.Add(new FinalFileData
                    {
                        Authentication = "Successfull",
                        PatientData = data
                    });
                }
                else
                {
                    fileData.Add(new FinalFileData
                    {
                        Authentication = "Authentication key is not valid"

                    });
                }


                return fileData;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Index was outside the bounds of the array.")
                {
                    throw new Exception("File is not in correct format");
                }

                throw new Exception(ex.Message+" "+ex.InnerException+" "+ex.Source);
            }
        }
    }
}
