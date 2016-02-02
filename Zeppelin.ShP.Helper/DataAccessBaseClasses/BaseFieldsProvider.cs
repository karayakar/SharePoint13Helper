using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.Helpers;

namespace Zeppelin.ShP.Helper.DataAccessBaseClasses
{
    /// <summary>
    /// Класс для хранения перечня полей, которые следует загружать. 
    /// Реализован для простого списка
    /// Является базовым классом для создания аналогичных классов для пользовательских списков
    /// </summary>
    public class BaseFieldsProvider
    {
        protected List<string> _FieldsList = new List<string>();

        /// <summary>
        /// Добавить колонку в перечень колонок
        /// </summary>
        /// <param name="columnName"></param>
        public void AddColumn(string columnName)
        {
            if (!_FieldsList.Exists(p => StringHelper.IsEqual(p, columnName)))
            {
                _FieldsList.Add(columnName);
            }
        }

        /// <summary>
        /// Конструктор для инициализации по умолчанию
        /// </summary>
        public BaseFieldsProvider()
        {
            InitFields();
        }

        /// <summary>
        /// Конструктор, который инициирует экземпляр на основе перечня названий полей
        /// </summary>
        /// <param name="fields"></param>
        public BaseFieldsProvider(string[] fields)
        {
            InitFields();
            foreach (string field in fields)
            {
                AddColumn(field);
            }
        }

        /// <summary>
        /// Инициализация полей по умолчанию
        /// </summary>
        public virtual void InitFields()
        {
            AddColumn(Constants.FieldNamesBaseTypes.Id);
            AddColumn(Constants.FieldNamesBaseTypes.Title);
            AddColumn(Constants.FieldNamesBaseTypes.DocIcon);
        }

        /// <summary>
        /// Возвращает количество определенных полей
        /// </summary>
        public int Count { get { return _FieldsList.Count; } }

        /// <summary>
        /// Возвращает часть CAML запроса по ограничению количества загружаемых полей 
        /// </summary>
        /// <param name="addNulableAttribute"></param>
        /// <returns></returns>
        public string GetFieldsForQuery(bool addNulableAttribute)
        {
            var sb = new StringBuilder();
            foreach (string fieldName in _FieldsList)
            {
                if (addNulableAttribute)
                {
                    //Используется в SPSiteDataQuery чтобы получать элементы даже без данного поля
                    sb.AppendFormat("<FieldRef Name='{0}' Nullable=\"TRUE\"/>", fieldName);
                }
                else
                {
                    sb.AppendFormat("<FieldRef Name='{0}'/>", fieldName);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Получить перечень полей в виде массива
        /// </summary>
        public string[] Fields
        {
            get
            {
                return _FieldsList.ToArray();
            }
        }

        /// <summary>
        /// Получить перечень полей в видк списка
        /// </summary>
        public List<string> FieldList
        {
            get
            {
                return _FieldsList;
            }
        }

        /// <summary>
        /// Проверка определено ли в списке полей указанное поле
        /// </summary>
        /// <param name="fieldName">Ноле списка</param>
        /// <returns></returns>
        public bool FieldExists(string fieldName)
        {
            return _FieldsList.Exists(p => (StringHelper.IsEqual(p, fieldName)));
        }

        /// <summary>
        /// Инициализировать ли поля создано, модернизировано, кем создано и кем изменено
        /// </summary>
        public virtual bool InitEditorFields
        {
            get
            {
                return false;
            }
        }
    }
}
