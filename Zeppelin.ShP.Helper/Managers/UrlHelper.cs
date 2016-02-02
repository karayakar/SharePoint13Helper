using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Zeppelin.ShP.Helper.AdditionalStructures;
using Zeppelin.ShP.Helper.Enums;
using Zeppelin.ShP.Helper.Helpers;

namespace Zeppelin.ShP.Helper.Managers
{
    /// <summary>
    /// Класс содержит методы по работе с URL 
    /// </summary>
    public static class UrlHelper
    {
        /// <summary>
        /// Returns first part of url: Scheme://host:port, example http://mail.ru:443
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetServerUrl(string absoluteUrl)
        {
            return GetServerUrl(absoluteUrl, UrlPortBehavior.MakeAsSource);
        }

        public static string GetServerUrl(string absoluteUrl, UrlPortBehavior urlPort)
        {
            //LoggingManager.GetLogWriter().WriteTrace(TraceSeverity.Verbose, string.Format("GetServerUrl. Url={0}, UrlPortBehavior={1}", absoluteUrl, urlPort));
            UlsLogging.LogInformation(string.Format("GetServerUrl. Url={0}, UrlPortBehavior={1}", absoluteUrl, urlPort));
            try
            {
                if (string.IsNullOrEmpty(absoluteUrl))
                {
                    return string.Empty;
                }

                Uri uri = new Uri(absoluteUrl);
                string serverUrl = string.Format("{0}://{1}:{2}", uri.Scheme, uri.Host, uri.Port);

                if ((uri.Port == 80) || uri.Port == 443)
                {
                    bool urlHasPort = (absoluteUrl.StartsWith(serverUrl));
                    bool removePort = ((urlPort == UrlPortBehavior.Remove80Port) || ((urlPort == UrlPortBehavior.MakeAsSource) && (!urlHasPort)));
                    if (removePort)
                    {
                        serverUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);
                    }
                }
                return serverUrl;
            }
            catch (Exception ex)
            {
                //LoggingManager.GetLogWriter().WriteError(
                //    string.Format("Ошибка при вызове метода GetServerUrl. Url={0}, UrlPortBehavior={1}", absoluteUrl, urlPort)
                //    , ex);
                UlsLogging.LogError(string.Format("Ошибка при вызове метода GetServerUrl. Url={0}, UrlPortBehavior={1}", absoluteUrl, urlPort));
                UlsLogging.LogError(string.Format("Ошибка при вызове метода GetServerUrl. Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace));
                throw;
            }
        }


        public static string GetUrlIncludingPort(string url)
        {
            string serverUrlofUrlToCorrect = GetServerUrl(url, UrlPortBehavior.Preserve80Port);
            string relativeUrlToCorrect = MakeRelative(url);
            string result = CombineUrl(serverUrlofUrlToCorrect, relativeUrlToCorrect);
            return result;
        }

        /// <summary>
        /// Формирует строку URL из текста
        /// Используется, например для создания URL для страницы, для которой пользователь в названии ввел спец символы
        /// </summary>
        /// <param name="unencodedUrl"></param>
        /// <returns></returns>
        public static string GetEncodedUrlFromString(this string unencodedUrl)
        {
            if (string.IsNullOrEmpty(unencodedUrl))
            {
                return string.Empty;
            }

            string normalizedUrl = unencodedUrl.ToLower().Replace("æ", "ae").Replace("ø", "o").Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(200);
            foreach (char c in normalizedUrl)
            {
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        sb.Append(c);
                        break;
                    case UnicodeCategory.SpaceSeparator:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DashPunctuation:
                        sb.Append('-');
                        break;
                    default:
                        break;
                }
            }

            return string.Join("-", sb.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)); // Removes repeating '-'
        }

        /// <summary>
        /// return ServerRelativeUrl for web site url 
        /// Can be used for application pages or pages on web site
        /// </summary>
        /// <param name="web"></param>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string GetFullReltiveUrl(this SPWeb web, string relativeUrl)
        {
            return CombineUrl(web.ServerRelativeUrl, relativeUrl);
        }

        private const char Separator = '/';

        /// <summary>
        /// Выполняет объединение частей URL адреса
        /// </summary>
        /// <param name="initUrl">первая часть адреса</param>
        /// <param name="relativeUrl">вторая часть адреса</param>
        /// <returns></returns>
        public static string CombineUrl(this string initUrl, string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl) || relativeUrl == "#")
            {
                return initUrl;
            }

            relativeUrl = relativeUrl.TrimStart(Separator);

            if (!string.IsNullOrEmpty(initUrl))
            {
                initUrl = initUrl.TrimEnd(Separator);
            }

            var result = string.Format("{0}/{1}", initUrl, relativeUrl);
            return result;
        }

        public static string CombineUrl(SPWeb web, string relativeUrl)
        {
            return CombineUrl(web.Url, relativeUrl);
        }

        /// <summary>
        /// Выполняет объединение частей URL адреса
        /// </summary>
        /// <param name="urlParts">части URL адреса</param>
        /// <returns></returns>
        public static string CombineUrl(string[] urlParts)
        {
            StringBuilder result = new StringBuilder();

            if ((urlParts == null) || (urlParts.Length == 0))
            {
                return string.Empty;
            }
            for (int i = 0; i < urlParts.Length; i++)
            {
                urlParts[i] = urlParts[i].TrimStart(Separator).TrimEnd(Separator);
                if (i != 0)
                {
                    result.Append(Separator);
                }
                result.Append(urlParts[i]);
            }
            return result.ToString();
        }

        /// <summary>
        /// URL Utility for generating the correct URL for a given Page, ListItem, Site etc. regardless of the Alternate Access Mapping
        /// </summary>
        /// <param name="orgUrl"></param>
        /// <returns></returns>
        public static string EnsureServerRelativeUrl(this string orgUrl)
        {
            //return SPContext.Current.Site.MakeFullUrl(orgUrl);
            return GetCurrentServerRelativeUrl(orgUrl);
        }

        /// <summary>
        /// URL Utility for generating the correct Server Url, from an absolute URL.
        /// i.e. retrieving URL Field from an SPListItem, which has/gets Absolute URL's.
        /// WARNING: method can change only url of curent web application
        /// </summary>
        /// <param name="absoluteUrl">Absolute url</param>
        /// <returns>Relative url</returns>
        public static string GetCurrentServerRelativeUrl(this string absoluteUrl)
        {
            return absoluteUrl.GetCurrentServerRelativeUrl(SPContext.Current.Site.WebApplication);
        }

        private static string GetCurrentServerRelativeUrl(this string absoluteUrl, SPWebApplication application)
        {
            if (string.IsNullOrEmpty(absoluteUrl))
            {
                return string.Empty;
            }

            absoluteUrl = absoluteUrl.Trim().ToLower();
            var webApplicationUrl = FoundWebApplicationUrl(absoluteUrl, application);
            if (!string.IsNullOrEmpty(webApplicationUrl))
            {
                if (absoluteUrl.Length > webApplicationUrl.Length)
                    absoluteUrl = absoluteUrl.Remove(0, webApplicationUrl.Length);
                else
                    //for example absoluteUrl = http://mail.ru and webApplicationUrl=http://mail.ru
                    absoluteUrl = "/";
                return EnshureStartSlash(absoluteUrl);
            }

            //we dont need to add slash here because will be corrupted urls to external sources
            return absoluteUrl;
        }

        /// <summary>
        /// Возвращает относительный адрес 
        /// </summary>
        /// <param name="absoluteUrl"></param>
        /// <returns></returns>
        public static string GetServerRelativeUrl(this string absoluteUrl)
        {
            foreach (var web in GetAllWebApplications())
            {
                var stringApplicationUrl = FoundWebApplicationUrl(absoluteUrl, web);
                if (!string.IsNullOrEmpty(stringApplicationUrl))
                {
                    return GetCurrentServerRelativeUrl(absoluteUrl, web);
                }
            }

            return absoluteUrl;
        }

        /// <summary>
        /// Возвращает ссылку на страницу пользователя для текущей сайтовой коллекции
        /// </summary>
        /// <param name="userId">ИД пользователя в сайтовой коллекции</param>
        /// <returns></returns>
        public static string GetSPUserLink(int userId)
        {
            return String.Format("/_layouts/userdisp.aspx?ID={0}", userId);
        }

        /// <summary>
        /// Возвращает относительную ссылку на страницу пользователя для текущей сайтовой коллекции
        /// </summary>
        /// <param name="site">Текущий сайт</param>
        /// <param name="userId">ИД пользователя в сайтовой коллекции</param>
        /// <returns></returns>
        public static string GetSPUserServerRelativeLink(SPSite site, int userId)
        {
            return UrlHelper.CombineUrl(site.ServerRelativeUrl, String.Format("/_layouts/userdisp.aspx?ID={0}", userId));
        }

        public static List<SPWebApplication> GetAllWebApplications()
        {
            var applications = new List<SPWebApplication>();
            if (SPFarm.Local != null && SPFarm.Local.Services != null)
            {
                var webServices = SPFarm.Local.Services.Where(p => p is SPWebService).Cast<SPWebService>();

                foreach (var service in webServices)
                {
                    applications.AddRange(service.WebApplications);
                }
            }

            return applications;
        }

        /// <summary>
        /// Check is url related to specified web.
        /// Method will find webApplication by specified webApplicationUrl.
        /// Then method will check is url from current webapplication. 
        /// NOTE: all relative urls will return true.
        /// </summary>
        /// <param name="urlToCheck">url to site which we need to check</param>
        /// <param name="webApplicationUrl">expected web application</param>
        /// <returns>Is url belongs to current web application</returns>
        public static bool CheckIsWebApplicationUrl(string urlToCheck, string webApplicationUrl)
        {
            if (string.IsNullOrEmpty(urlToCheck))
                return false;

            urlToCheck = urlToCheck.ToLower().Trim();

            if (urlToCheck.IsRelative())
                return false;

            var site = SPContext.Current.Site;

            SPWebApplication pointsToWebApp = null;
            SPAlternateUrl alternateUrl = null;

            //Url not found in current web application. Will search in farm
            //Search in all web applications
            foreach (SPWebApplication webApp in GetAllWebApplications())
            {
                alternateUrl = FindAlternateUrl(webApplicationUrl, webApp);
                if (alternateUrl != null)
                {
                    pointsToWebApp = webApp;
                    break;
                }
            }

            if (pointsToWebApp != null)
            {
                var createdAlternateUrl = FindAlternateUrl(urlToCheck, pointsToWebApp);
                return createdAlternateUrl != null;
            }

            return false;
        }

        /// <summary>
        /// Get relative link if it points to current site
        /// and absolute if link points to other web. 
        /// Also zone will be checked. 
        /// This method is cross web application urls "problem" fix.
        /// </summary>
        /// <param name="url">Relative url</param>
        /// <param name="linkToWeb">link to web where content should be located</param>
        /// <returns>Relative or absolute url</returns>
        public static string GetRelativeOrAbsolute(this string relativeUrl, string linkToWeb)
        {
            relativeUrl = relativeUrl.GetCurrentServerRelativeUrl();
            SPWebApplication pointsToWebApp = null;
            SPAlternateUrl alternateUrl = null;

            //Url not found in current web application. Will search in farm
            foreach (SPWebApplication webApp in GetAllWebApplications())
            {
                alternateUrl = FindAlternateUrl(linkToWeb, webApp);
                if (alternateUrl != null)
                {
                    pointsToWebApp = webApp;
                    break;
                }
            }

            if (pointsToWebApp != null)
            {
                if (SPContext.Current.Site.WebApplication != pointsToWebApp)
                {
                    return GetAbsoluteUrlByWebAndZone(relativeUrl, pointsToWebApp, alternateUrl);
                }
            }

            return relativeUrl;
        }

        /// <summary>
        /// Возвращает зону для открытого сайта
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public static SPUrlZone GetCurrentZone(SPSite site)
        {
            var alternateUrl = FindAlternateUrl(site.Url, site.WebApplication);
            if (alternateUrl != null)
            {
                return alternateUrl.UrlZone;
            }

            return SPUrlZone.Default;
        }

        /// <summary>
        /// Возвращает абсолютный url для веб-приложения 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="web"></param>
        /// <param name="alternateUrl"></param>
        /// <returns></returns>
        private static string GetAbsoluteUrlByWebAndZone(string url, SPWebApplication web, SPAlternateUrl alternateUrl)
        {
            var site = SPContext.Current.Site;

            // Get current site zone. site.Zone for some reasons always default
            var curZone = GetCurrentZone(site);

            // Get current site url zone or default
            var currentZone = web.AlternateUrls.Where(p => p.UrlZone == curZone).FirstOrDefault();
            if (currentZone == null || string.IsNullOrEmpty(currentZone.Uri.ToString()))
            {
                currentZone = web.AlternateUrls.Where(p => p.UrlZone == SPUrlZone.Default).FirstOrDefault();
            }

            var targetZone = currentZone.Uri.GetUrlString();
            if (alternateUrl != null)
            {
                string currentSiteUrl = alternateUrl.Uri.GetUrlString();
                currentSiteUrl = RemoveEndSlash(currentSiteUrl);
                targetZone = RemoveEndSlash(targetZone);

                var toReturn = url.Replace(currentSiteUrl, targetZone);
                if (toReturn.StartsWith("http"))
                {
                    return toReturn;
                }
                else
                {
                    return CombineUrl(targetZone, toReturn);
                }
            }

            return CombineUrl(targetZone, url);
        }

        /// <summary>
        /// обрезвет из url начальный и заверщающие пробелы
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetUrlString(this Uri uri)
        {
            return uri.ToString().Trim().ToLower();
        }

        /// <summary>
        /// Относится ли url к веб-приложению
        /// </summary>
        /// <param name="absoluteUrl"></param>
        /// <param name="webApp"></param>
        /// <returns></returns>
        private static bool IsUrlPointsToWeb(string absoluteUrl, SPWebApplication webApp)
        {
            var result = FoundWebApplicationUrl(absoluteUrl, webApp);
            return !string.IsNullOrEmpty(result);
        }

        /// <summary>
        /// Возвращает базовый url для веб-приложения
        /// </summary>
        /// <param name="absoluteUrl"></param>
        /// <param name="webApp"></param>
        /// <returns></returns>
        private static string FoundWebApplicationUrl(string absoluteUrl, SPWebApplication webApp)
        {
            var alternateUrl = FindAlternateUrl(absoluteUrl, webApp);
            if (alternateUrl != null)
            {
                return alternateUrl.Uri.GetUrlString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Находит альтернативный базовый URL для полного пути и веб-приложения
        /// </summary>
        /// <param name="absoluteUrl"></param>
        /// <param name="webApp"></param>
        /// <returns></returns>
        private static SPAlternateUrl FindAlternateUrl(string absoluteUrl, SPWebApplication webApp)
        {
            if (string.IsNullOrEmpty(absoluteUrl))
            {
                return null;
            }

            absoluteUrl = absoluteUrl.ToLower().Trim();

            foreach (SPAlternateUrl alternateUrl in webApp.AlternateUrls)
            {
                string webAppUrl = alternateUrl.Uri.GetUrlString();
                if (absoluteUrl.StartsWith(webAppUrl) || webAppUrl.StartsWith(absoluteUrl))
                {
                    return alternateUrl;
                }
            }

            return null;
        }

        /// <summary>
        /// Является ли данный путь относительным
        /// </summary>
        /// <param name="absUrl"></param>
        /// <returns></returns>
        public static bool IsRelative(this string absUrl)
        {
            return absUrl.StartsWith("/") || !absUrl.StartsWith("http");
        }


        /// <summary>
        /// Убирает из пути завершающий слеш
        /// </summary>
        /// <param name="absUrl"></param>
        /// <returns></returns>
        public static string RemoveEndSlash(string absUrl)
        {
            if (absUrl.EndsWith("/"))
            {
                return absUrl.Remove(absUrl.Length - 1, 1);
            }

            return absUrl;
        }

        /// <summary>
        /// Добавляет слеш в начало строки
        /// </summary>
        /// <param name="absUrl"></param>
        /// <returns></returns>
        public static string EnshureStartSlash(this string absUrl)
        {
            if (!absUrl.StartsWith("/"))
            {
                absUrl = "/" + absUrl;
            }

            return absUrl.Replace("//", "/");
        }

        /// <summary>
        /// Формирует Url к файлу иконки исходя из расширения файла
        /// </summary>
        /// <param name="fileExt">расширение файла (например docx)</param>
        /// <returns></returns>
        public static string GetFileIconUrl(string fileExt)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(fileExt))
            {
                result = @"/_layouts/Images/ICgen.gif";
            }
            else
            {
                result = string.Format(@"/_layouts/Images/IC{0}.gif", fileExt);
            }
            return result;
        }

        /// <summary>
        /// Возвращает путь к странице загрузки документа в библиотеку документов
        /// </summary>
        /// <param name="spWeb">сайт</param>
        /// <param name="listName">имя библиотеки документов</param>
        /// <returns></returns>
        public static string GetNewDocumentUrl(SPWeb spWeb, string listName)
        {
            string result = string.Empty;
            SPList list = spWeb.Lists.TryGetList(listName);
            if (list != null)
            {
                result = GetFullReltiveUrl(spWeb
                    , string.Format("/_layouts/Upload.aspx?List={0}&RootFolder=&Source={1}", list.ID, System.Web.HttpContext.Current.Request.RawUrl)
                    );
            }

            return result;
        }

        /// <summary>
        /// Возвращает URL представления по умолчанию для списка
        /// </summary>
        /// <param name="spWeb">сайт</param>
        /// <param name="listName">имя списка</param>
        /// <returns></returns>
        public static string GetListDefaultViewUrl(SPWeb spWeb, string listName)
        {
            string result = string.Empty;
            SPList list = spWeb.Lists.TryGetList(listName);
            if (list != null)
            {
                result = list.DefaultViewUrl;
            }

            return result;
        }

        /// <summary>
        /// Получить относительный URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetServerRelUrlFromFullUrl(string url)
        {
            int index = url.IndexOf("//");
            if ((index < 0) || (index == (url.Length - 2)))
            {
                throw new ArgumentException();
            }
            int startIndex = url.IndexOf('/', index + 2);
            if (startIndex < 0)
            {
                return "/";
            }
            string str = url.Substring(startIndex);
            if (str.IndexOf("?") >= 0)
                str = str.Substring(0, str.IndexOf("?"));

            //if (str.IndexOf(".aspx") > 0)
            //    str = str.Substring(0, str.LastIndexOf("/"));

            if ((str.Length > 1) && (str[str.Length - 1] == '/'))
            {
                return str.Substring(0, str.Length - 1);
            }
            return str;
        }

        /// <summary>
        /// Получить ссылку к странице типа pageType элемента
        /// </summary>
        /// <param name="pageType">Тип страницы</param>
        /// <param name="webUrl">полный url cfqnf</param>
        /// <param name="listId">Ид списка</param>
        /// <param name="itemId">Ид элемента</param>
        /// <returns></returns>
        public static string GetURlForItem(PAGETYPE pageType, string webUrl, Guid listId, int itemId)
        {
            int pageTypeAsInt = (int)pageType;
            return UrlHelper.CombineUrl(
                webUrl,
                string.Format("/_layouts/listform.aspx?PageType={0}&ListId={1}&ID={2}", pageType, listId, itemId)
                );
        }

        /// <summary>
        /// Получить ссылку к странице типа pageType элемента
        /// </summary>
        /// <param name="pageType">тип страницы</param>
        /// <param name="item">элемент списка</param>
        /// <param name="pageUrlType">тип ссылки (абсолютный/относительный)</param>
        /// <returns></returns>
        public static string GetURlForItem(PAGETYPE pageType, SPListItem item, PageUrlTypeEnum pageUrlType)
        {
            if (pageUrlType == PageUrlTypeEnum.Relative)
            {
                return GetURlForItem(pageType, item.Web.ServerRelativeUrl, item.ParentList.ID, item.ID);
            }
            else
            {
                return GetURlForItem(pageType, item.Web.Url, item.ParentList.ID, item.ID);
            }
        }

        /// <summary>
        /// Получить значение параметра
        /// </summary>
        /// <param name="Request">Объет типа запрос</param>
        /// <param name="paramName">Имя параметра</param>
        /// <returns></returns>
        public static string GetUrlParamValue(HttpRequest Request, string paramName)
        {
            string result = string.Empty;
            result = Request.QueryString[paramName];
            return result;
        }

        /// <summary>
        /// Получить значение параметра в виде целого числа
        /// </summary>
        /// <param name="Request">Объет типа запрос</param>
        /// <param name="paramName">Имя параметра</param>
        /// <returns></returns>
        public static int? GetUrlParamValueInt(HttpRequest Request, string paramName)
        {
            try
            {
                string value = GetUrlParamValue(Request, paramName);
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                return int.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Установить значение параметра для url
        /// </summary>
        /// <param name="currentPageUrl">url</param>
        /// <param name="paramToReplace">имя параметра</param>
        /// <param name="newValue"значение></param>
        /// <returns></returns>
        public static string SetQueryStringParam(string currentPageUrl, string paramToReplace, string newValue)
        {
            string urlWithoutQuery = currentPageUrl.IndexOf('?') >= 0
                ? currentPageUrl.Substring(0, currentPageUrl.IndexOf('?'))
                : currentPageUrl;

            string queryString = currentPageUrl.IndexOf('?') >= 0
                ? currentPageUrl.Substring(currentPageUrl.IndexOf('?'))
                : null;

            var queryParamList = queryString != null
                ? HttpUtility.ParseQueryString(queryString)
                : HttpUtility.ParseQueryString(string.Empty);

            if (queryParamList[paramToReplace] != null)
            {
                queryParamList[paramToReplace] = newValue;
            }
            else
            {
                queryParamList.Add(paramToReplace, newValue);
            }
            return String.Format("{0}?{1}", urlWithoutQuery, queryParamList);
        }

        public static string MakeRelative(string absoluteOrRelativeUrl)
        {
            if (string.IsNullOrEmpty(absoluteOrRelativeUrl))
            {
                return string.Empty;
            }
            if (IsRelative(absoluteOrRelativeUrl))
            {
                return absoluteOrRelativeUrl;
            }
            else
            {
                string serverUrl = GetServerUrl(absoluteOrRelativeUrl, UrlPortBehavior.MakeAsSource);
                string relativeUrl = absoluteOrRelativeUrl.Substring(serverUrl.Length);
                if (string.IsNullOrEmpty(relativeUrl))
                {
                    relativeUrl = "/";
                }
                return relativeUrl;
            }
        }

        public static string TryMakeRelative(string absoluteOrRelativeUrl, string currentRequestUrl)
        {
            if (string.IsNullOrEmpty(absoluteOrRelativeUrl))
            {
                return string.Empty;
            }
            if (IsRelative(absoluteOrRelativeUrl))
            {
                return absoluteOrRelativeUrl;
            }
            else
            {
                string serverUrl = GetServerUrl(absoluteOrRelativeUrl.ToLower(), UrlPortBehavior.Preserve80Port);
                string currentServerUrl = GetServerUrl(currentRequestUrl.ToLower(), UrlPortBehavior.Preserve80Port);
                if (StringHelper.IsEqual(serverUrl, currentServerUrl))
                {
                    return MakeRelative(absoluteOrRelativeUrl);
                }
                return absoluteOrRelativeUrl;
            }
        }

        /// <summary>
        /// Проверяет зону текущего currentRequestUrl и корректирует адрес для absoluteUrlToCorrect
        /// Возвращает абсолютный адрес (включая порт в url)
        /// </summary>
        /// <param name="absoluteUrl"></param>
        /// <param name="currentRequestUrl"></param>
        /// <returns></returns>
        public static string CorrectUrlForAlternateAccess(string absoluteUrlToCorrect, string currentRequestUrl)
        {
            if (string.IsNullOrEmpty(absoluteUrlToCorrect))
            {
                return string.Empty;
            }

            absoluteUrlToCorrect = absoluteUrlToCorrect.ToLower();
            currentRequestUrl = currentRequestUrl.ToLower();

            string currentServerUrl = GetServerUrl(currentRequestUrl, UrlPortBehavior.Preserve80Port);
            if (IsRelative(absoluteUrlToCorrect))
            {
                return CombineUrl(currentServerUrl, absoluteUrlToCorrect);
            }

            string serverUrlofUrlToCorrect = GetServerUrl(absoluteUrlToCorrect, UrlPortBehavior.Preserve80Port);
            string relativeUrlToCorrect = MakeRelative(absoluteUrlToCorrect);
            absoluteUrlToCorrect = CombineUrl(serverUrlofUrlToCorrect, relativeUrlToCorrect);

            if (currentServerUrl == serverUrlofUrlToCorrect)
            {
                //если итак понятно
                return absoluteUrlToCorrect;
            }

            string result = string.Empty;
            List<AlternateAccessRecord> alternateAccessRecords = GetAlternateAccessRecord();

            AlternateAccessRecord currentZone = alternateAccessRecords.FirstOrDefault(p => p.Url == currentServerUrl);
            if (currentZone == null)
            {
                //Непонятная ситуация. Зашли на страницу с помощью левого адреса
                return absoluteUrlToCorrect;
            }
            else
            {
                AlternateAccessRecord zoneInfoOfUrlToCorrect = alternateAccessRecords.SingleOrDefault(p => StringHelper.IsEqualWithTrim(serverUrlofUrlToCorrect, GetServerUrl(p.Url, UrlPortBehavior.Preserve80Port)));
                if (zoneInfoOfUrlToCorrect == null)
                {
                    //Не найдено приложения для этого адреса - это внешний адрес
                    return absoluteUrlToCorrect;
                }
                else
                {
                    if (StringHelper.IsEqual(currentZone.Application, zoneInfoOfUrlToCorrect.Application))
                    {
                        return GetServerRelUrlFromFullUrl(absoluteUrlToCorrect);
                    }

#warning тут проблема с несколькими альтернативными путями в одной зоне
                    //AlternateAccessRecord correctionZoneInfo = alternateAccessRecords.SingleOrDefault(p => ((p.Zone == currentZone.Zone) && (p.Application == zoneInfoOfUrlToCorrect.Application)));
                    AlternateAccessRecord correctionZoneInfo = alternateAccessRecords.FirstOrDefault(p => ((p.Zone == currentZone.Zone) && (p.Application == zoneInfoOfUrlToCorrect.Application)));
                    if (correctionZoneInfo == null)
                    {
                        //Не найдена зона для адреса, который корректируем
                        return absoluteUrlToCorrect;
                    }
                    else
                    {
                        //заменяем сервер
                        return CombineUrl(correctionZoneInfo.Url, relativeUrlToCorrect);
                    }
                }
            }
        }

        //private static List<AlternateAccessRecord> AlternateAccessRecordsCache;
        private static DateTime? LastAlternateAccessRecordsUpdated;
        private static object AlternateAccessRecordsCacheSync;

        static UrlHelper()
        {
            LastAlternateAccessRecordsUpdated = null;
            AlternateAccessRecordsCacheSync = new object();
            //AlternateAccessRecordsCache = GetAlternateAccessRecord();
        }

        //public static List<AlternateAccessRecord> GetAlternateAccessRecord()
        //{
        //    if ((!LastAlternateAccessRecordsUpdated.HasValue) || (LastAlternateAccessRecordsUpdated.Value.AddMinutes(1) < DateTime.Now))
        //    {
        //        if (Monitor.TryEnter(AlternateAccessRecordsCacheSync))
        //        {
        //            try
        //            {
        //                List<AlternateAccessRecord> alternateAccessRecords = new List<AlternateAccessRecord>();
        //                foreach (SPWebApplication webApp in GetAllWebApplications())
        //                {
        //                    foreach (SPAlternateUrl alternateUrl in webApp.AlternateUrls)
        //                    {
        //                        if (alternateUrl.Uri.AbsoluteUri != null)
        //                        {
        //                            alternateAccessRecords.Add(new AlternateAccessRecord()
        //                            {
        //                                Application = webApp.Name,
        //                                Zone = alternateUrl.UrlZone,
        //                                Url = GetServerUrl(alternateUrl.Uri.AbsoluteUri, UrlPortBehavior.Preserve80Port)
        //                            });
        //                        }
        //                    }
        //                }
        //                LastAlternateAccessRecordsUpdated = DateTime.Now;
        //                AlternateAccessRecordsCache = alternateAccessRecords;
        //            }
        //            finally
        //            {
        //                Monitor.Exit(AlternateAccessRecordsCacheSync);
        //            }
        //        }
        //    }
        //    return AlternateAccessRecordsCache;
        //}

    }
}
