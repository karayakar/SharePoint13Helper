using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web.UI.WebControls.WebParts;
using Zeppelin.ShP.Helper;
using Zeppelin.ShP.Helper.Helpers;

namespace ShP.Helper.Testing.WebParts.SendEMail
{
    [ToolboxItemAttribute(false)]
    public partial class SendEMail : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SendEMail()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void SendMailButton1_Click(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            string emails = senDTo.Value;
            string subject = Subject.Value;
            string htmlbody = BodyTextBox.Text;

            if (emails == "" || subject == "" || htmlbody == "")
            {
                LabelError.Text = "Emails, Subject or Body is Empty";
                return;
            }
            EMail.SendMail(emails, subject, htmlbody);
            Page.Response.Redirect(Page.Request.Url.AbsolutePath);
        }

        protected void SendMailButton2_Click(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            string emails = senDTo.Value;
            string subject = Subject.Value;
            string htmlbody = BodyTextBox.Text;

            if (emails == "" || subject == "" || htmlbody == "")
            {
                LabelError.Text = "Emails, Subject or Body is Empty";
                return;
            }
            EMail.SendMail(web, emails, subject, htmlbody);
            Page.Response.Redirect(Page.Request.Url.AbsolutePath);
        }

        protected void SendMailButton3_Click(object sender, EventArgs e)
        {
            try
            {
                SPWeb web = SPContext.Current.Web;
                string emails = senDTo.Value;
                string subject = Subject.Value;
                string htmlbody = BodyTextBox.Text;

                if (emails == "" || subject == "" || htmlbody == "")
                {
                    LabelError.Text = "Emails, Subject or Body is Empty";
                    return;
                }

                if (FileUpload2.HasFile)
                {
                    Stream fileStream = FileUpload2.PostedFile.InputStream;
                    string fileName = FileUpload2.PostedFile.FileName;
                    Dictionary<string, string> parameters2 = new Dictionary<string, string>();
                    parameters2["SendTo"] = emails;
                    parameters2["Subject"] = subject + "_1";
                    parameters2["Body"] = htmlbody;
                    parameters2["FileName"] = fileName;
                    EMail.SendMail(parameters2, fileStream);
                    Page.Response.Redirect(Page.Request.Url.AbsolutePath);
                }
            }
            catch (Exception ex)
            {
                LabelError.Text += "<br/><strong>Button2_Click</strong>";
                LabelError.Text += "<br/>Message: " + ex.Message;
                LabelError.Text += "<br/>StackTrace: " + ex.StackTrace;
                UlsLogging.LogError("Button2_Click Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }

        protected void SendMailButton4_Click(object sender, EventArgs e)
        {
            try
            {
                SPWeb web = SPContext.Current.Web;
                string emails = senDTo.Value;
                string subject = Subject.Value;
                string htmlbody = BodyTextBox.Text;

                if (emails == "" || subject == "" || htmlbody == "")
                {
                    LabelError.Text = "Emails, Subject or Body is Empty";
                    return;
                }
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                Dictionary<string, Stream> attachmentsStream = new Dictionary<string, Stream>();

                Stream fileStream = null;
                string fileName = "";

                parameters["SendTo"] = emails;
                parameters["Subject"] = subject;
                parameters["Body"] = htmlbody;

                if (FileUpload.HasFile)
                {
                    fileStream = FileUpload.PostedFile.InputStream;
                    fileName = FileUpload.PostedFile.FileName;
                    attachmentsStream.Add(fileName, fileStream);
                }

                if (FileUpload1.HasFile)
                {
                    fileStream = FileUpload1.PostedFile.InputStream;
                    fileName = FileUpload1.PostedFile.FileName;
                    attachmentsStream.Add(fileName, fileStream);

                }

                EMail.SendMail(parameters, attachmentsStream);
                Page.Response.Redirect(Page.Request.Url.AbsolutePath);
            }
            catch (Exception ex)
            {
                LabelError.Text += "<br/><strong>Button1_Click</strong>";
                LabelError.Text += "<br/>Message: " + ex.Message;
                LabelError.Text += "<br/>StackTrace: " + ex.StackTrace;
                UlsLogging.LogError("Button1_Click Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }

    }
}
