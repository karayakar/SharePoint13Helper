// -----------------------------------------------------------------------
// <copyright file="Mail.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Zeppelin.ShP.Helper.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.SharePoint;
    using System.Collections.Specialized;
    using Microsoft.SharePoint.Administration;
    using System.Net.Mail;
    using System.IO;

    /// <summary>
    /// Класс содержит методы для упрощения отправки почты (с помощью System.Net.Mail)
    /// </summary>
    public class EMail
    {
        /// <summary>
        /// Отправляет email
        /// </summary>
        /// <param name="web">сайт</param>
        /// <param name="emails">адресаты</param>
        /// <param name="subject">тема</param>
        /// <param name="htmlbody">тело</param>
        public static void SendMail(SPWeb web, string emails, string subject, string htmlbody)
        {
            string mailServerAddress = web.Site.WebApplication.OutboundMailServiceInstance.Server.Address;
            string fromEmail = web.Site.WebApplication.OutboundMailSenderAddress;
            SendMail(mailServerAddress, fromEmail, emails, subject, htmlbody, null);
        }

        /// <summary>
        /// Отправляет email
        /// </summary>
        /// <param name="emails">адресаты</param>
        /// <param name="subject">тема</param>
        /// <param name="htmlbody">тело</param>
        public static void SendMail(string emails, string subject, string htmlbody)
        {
            SPAdministrationWebApplication adminApp = SPAdministrationWebApplication.Local;
            string mailServerAddress = adminApp.OutboundMailServiceInstance.Server.Address;
            string fromEmail = adminApp.OutboundMailSenderAddress;
            SendMail(mailServerAddress, fromEmail, emails, subject, htmlbody, null);
        }

        /// <summary>
        /// Отправляет email
        /// </summary>
        /// <param name="SendTo">перечень адресов</param>
        /// <param name="Subject">тема</param>
        /// <param name="Body">тело сообщения</param>
        /// <param name="FileName">название файла</param>
        /// <param name="stream">прикрепленный документ</param>
        public static void SendMail(IDictionary<string, string> parameters, Stream stream)
        {
            try
            {
                if (!parameters.ContainsKey("SendTo"))
                {
                    UlsLogging.LogInformation("SendMail. \"SendTo\" is empty");
                }
                else if (!parameters.ContainsKey("Subject"))
                {
                    UlsLogging.LogInformation("SendMail. \"Subject\" is empty");
                }
                else if (!parameters.ContainsKey("Body"))
                {
                    UlsLogging.LogInformation("SendMail. \"Body\" is empty");
                }
                else if (!parameters.ContainsKey("FileName"))
                {
                    UlsLogging.LogInformation("SendMail. \"FileName\" is empty");
                }
                else
                {
                    SPAdministrationWebApplication adminApp = SPAdministrationWebApplication.Local;
                    string mailServerAddress = adminApp.OutboundMailServiceInstance.Server.Address;
                    string fromEmail = adminApp.OutboundMailSenderAddress;
                    Dictionary<string, Stream> attachmentsStream = null;
                    if (parameters["FileName"] != "" && stream != null && stream.Length > 0)
                    {
                        attachmentsStream = new Dictionary<string, Stream>();
                        attachmentsStream.Add(parameters["FileName"], stream);
                    }
                    SendMail(mailServerAddress, fromEmail, parameters["SendTo"], parameters["Subject"], parameters["Body"], attachmentsStream);
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Email. SendMail(IDictionary<string, string> parameters, Stream stream). Message:" + ex.Message + ", StackTrace:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Отправляет email
        /// </summary>
        /// <param name="SendTo">перечень адресов</param>
        /// <param name="Subject">тема</param>
        /// <param name="Body">тело сообщения</param>
        /// <param name="attachmentsStream">прикрепленные документы</param>
        public static void SendMail(IDictionary<string, string> parameters, Dictionary<string, Stream> attachmentsStream)
        {
            try
            {
                if (!parameters.ContainsKey("SendTo"))
                {
                    UlsLogging.LogInformation("SendMail. \"SendTo\" is empty");
                }
                else if (!parameters.ContainsKey("Subject"))
                {
                    UlsLogging.LogInformation("SendMail. \"Subject\" is empty");
                }
                else if (!parameters.ContainsKey("Body"))
                {
                    UlsLogging.LogInformation("SendMail. \"Body\" is empty");
                }
                else
                {
                    SPAdministrationWebApplication adminApp = SPAdministrationWebApplication.Local;
                    string mailServerAddress = adminApp.OutboundMailServiceInstance.Server.Address;
                    string fromEmail = adminApp.OutboundMailSenderAddress;
                    SendMail(mailServerAddress, fromEmail, parameters["SendTo"], parameters["Subject"], parameters["Body"], attachmentsStream);
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Email. SendMail(IDictionary<string, string> parameters, Dictionary<string, Stream> attachmentsStream). Message:" + ex.Message + ", StackTrace:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Отправляет email
        /// </summary>
        /// <param name="mailServerAddress">Имя или адрес сервера</param>
        /// <param name="fromEmail">с почтового ящика</param>
        /// <param name="emails">перечень адресов</param>
        /// <param name="subject">тема</param>
        /// <param name="htmlbody">тело сообщения</param>
        /// <param name="attachments">прикрепленные документы (split ";")</param>
        public static void SendMail(string mailServerAddress, string fromEmail, string emails, string subject, string htmlbody, Dictionary<string, Stream> attachmentsStream)
        {
            try
            {
                SPAdministrationWebApplication adminApp = SPAdministrationWebApplication.Local;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = htmlbody;
                mailMessage.IsBodyHtml = true;
                if (attachmentsStream != null)
                {
                    if (attachmentsStream.Count > 0)
                    {
                        foreach (KeyValuePair<string, Stream> streamCollection in attachmentsStream)
                        {
                            string fileName = Convert.ToString(streamCollection.Key);
                            Stream stream = streamCollection.Value;
                            if (stream != null)
                                if (fileName != "" && stream.Length > 0)
                                {
                                    Attachment attachment = new Attachment(stream, fileName);
                                    mailMessage.Attachments.Add(attachment);
                                }
                        }
                    }
                }
                else
                {
                    UlsLogging.LogInformation("SendMail attachmentsStream is null");
                }
                string[] emailArray = emails.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (emailArray.Length > 0)
                {
                    foreach (string email in emailArray)
                    {
                        mailMessage.To.Add(email.Trim());
                    }
                }
                else
                {
                    mailMessage.To.Add(emails);
                }
                SmtpClient smtpClient = new SmtpClient(mailServerAddress);
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("EMail. SendMail(string mailServerAddress, string fromEmail, string emails, string subject, string htmlbody, Dictionary<string, MemoryStream> attachmentsStream). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
    }
}
