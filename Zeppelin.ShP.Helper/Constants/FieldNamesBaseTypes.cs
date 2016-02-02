using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeppelin.ShP.Helper.Constants
{
    /// <summary>
    /// Класс для хранения констант для работы с полями списков
    /// </summary>
    public static class FieldNamesBaseTypes
    {
        public static readonly Guid PublishingRollupImage = new Guid("543bc2cf-1f30-488e-8f25-6fe3b689d9ac");

        public static readonly string Id = "ID";
        public static readonly string Title = "Title";
        public static readonly string LinkTitle = "LinkTitle";
        public static readonly string Created = "Created";
        public static readonly string CreatedBy = "Author";
        public static readonly string Modified = "Modified";
        public static readonly string ModifiedBy = "Editor";
        public static readonly string ContentTypeId = "ContentTypeId";
        public static readonly string EncodedAbsUrl = "EncodedAbsUrl";


        public static readonly string FileUrl = "FileRef";
        public static readonly string FileName = "FileLeafRef";
        public static readonly string DocIcon = "DocIcon";
        public static readonly string FileSize = "File_x0020_Size";
        public static readonly string LinkFilename = "LinkFilename";

        public static readonly string WebId = "WebId";
        public static readonly string ListId = "ListId";

        public static readonly string DocumentSize = "File Size";
        public static readonly string DocumentName = "Name";
        public static readonly string FileRef = "FileRef";
        public static readonly string TaxKeyword = "TaxKeyword";
        public static readonly string ContentType = "ContentType";
        public static readonly string ModerationStatus = "_ModerationStatus";
        public static readonly string ModerationComments = "_ModerationComments";
        public static readonly string Comments = "Comments";
        public static readonly string CheckinComment = "_CheckinComment";
        public static readonly string CheckoutUser = "CheckoutUser";

        public static readonly string AverageRating = "AverageRating";
        public static readonly string RatingCount = "RatingCount";
        public static readonly string PublishingRollupImageName = "PublishingRollupImage";
        public static readonly string PublishingPageImage = "PublishingPageImage";
    }
}
