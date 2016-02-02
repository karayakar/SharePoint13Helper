using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeppelin.ShP.Helper.Helpers
{
    public class UlsLogging : SPDiagnosticsServiceBase
    {

        // Product name
        private const string PRODUCT_NAME = "Zeppelin.Helper";

        #region private variables

        // Current instance
        private static UlsLogging _current;

        // area
        private static SPDiagnosticsArea _area;

        // category
        private static SPDiagnosticsCategory _catError;
        private static SPDiagnosticsCategory _catWarning;
        private static SPDiagnosticsCategory _catLogging;

        #endregion

        private static class CategoryName
        {
            public const string Error = "Error";
            public const string Warning = "Warning";
            public const string Logging = "Logging";
        }

        private static UlsLogging Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new UlsLogging();
                }
                return _current;
            }
        }

        // Get Area
        private static SPDiagnosticsArea Area
        {
            get
            {
                if (_area == null)
                {
                    _area = UlsLogging.Current.Areas[PRODUCT_NAME];
                }
                return _area;
            }
        }

        // Get error category
        private static SPDiagnosticsCategory CategoryError
        {
            get
            {
                if (_catError == null)
                {
                    _catError = Area.Categories[CategoryName.Error];
                }
                return _catError;
            }
        }

        // Get warning category
        private static SPDiagnosticsCategory CategoryWarning
        {
            get
            {
                if (_catWarning == null)
                {
                    _catWarning = Area.Categories[CategoryName.Warning];
                }
                return _catWarning;
            }
        }

        // Get logging category
        private static SPDiagnosticsCategory CategoryLogging
        {
            get
            {
                if (_catLogging == null)
                {
                    _catLogging = Area.Categories[CategoryName.Logging];
                }
                return _catLogging;
            }
        }

        private UlsLogging()
            : base(PRODUCT_NAME, SPFarm.Local)
        {
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            var cat = new List<SPDiagnosticsCategory>{
                new SPDiagnosticsCategory(CategoryName.Error, TraceSeverity.High,EventSeverity.Error),
                new SPDiagnosticsCategory(CategoryName.Warning, TraceSeverity.Medium,EventSeverity.Warning),
                new SPDiagnosticsCategory(CategoryName.Logging,TraceSeverity.Verbose,EventSeverity.Information)
            };
            var areas = new List<SPDiagnosticsArea>();
            areas.Add(new SPDiagnosticsArea(PRODUCT_NAME, cat));

            return areas;
        }

        // Log Error
        public static void LogError(string msg)
        {
            UlsLogging.Current.WriteTrace(0, CategoryError, TraceSeverity.High, msg);
        }
        public static void LogError(string msg, params object[] args)
        {
            UlsLogging.Current.WriteTrace(0, CategoryError, TraceSeverity.High, msg, args);
            //string[] sArgs = Array.ConvertAll(args, p => (p ?? String.Empty).ToString());
        }

        // Log Warning
        public static void LogWarning(string msg)
        {
            UlsLogging.Current.WriteTrace(0, CategoryWarning, TraceSeverity.Medium, msg);
        }
        public static void LogWarning(string msg, params object[] args)
        {
            UlsLogging.Current.WriteTrace(0, CategoryWarning, TraceSeverity.Medium, msg, args);
            //string[] sArgs = Array.ConvertAll(args, p => (p ?? String.Empty).ToString());
        }

        // Log Information
        public static void LogInformation(string msg)
        {
            UlsLogging.Current.WriteTrace(0, CategoryLogging, TraceSeverity.Verbose, msg);

        }
        public static void LogInformation(string msg, params object[] args)
        {
            UlsLogging.Current.WriteTrace(0, CategoryLogging, TraceSeverity.Verbose, msg, args);
            //string[] sArgs = Array.ConvertAll(args, p => (p ?? String.Empty).ToString());

        }

    }
}
