using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeppelin.ShP.Helper.DataAccessBaseClasses
{
    /// <summary>
    /// Класс для хранения данных об SPUser
    /// </summary>
    [Serializable]
    public class UserData
    {
        public string Sid { get; protected set; }

        public int Id { get; protected set; }

        public string Name { get; protected set; }

        public bool IsSiteAdmin { get; protected set; }

        public string Email { get; protected set; }

        public string LoginName { get; protected set; }

        // public bool IsDomainGroup { get; protected set; }

        //public UserDataType Type { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="email">Email</param>
        /// <param name="type">Тип пользователя</param>
        public UserData(string name, string email/*, UserDataType type*/)
        {
            Name = name;
            Email = email;
            LoginName = email;
            //this.Type = type;
            // IsDomainGroup = isDomainGroup;
        }

        /// <summary>
        /// Создание на основе объекта SPUser
        /// </summary>
        /// <param name="user"></param>
        public UserData(Microsoft.SharePoint.SPUser user)
        {
            Init(user);
        }

        /// <summary>
        /// Создание на основе значения (в текстовом виде) поля элемента списка
        /// </summary>
        /// <param name="userFieldValue"></param>
        /// <param name="site"></param>
        public UserData(string userFieldValue, SPSite site)
        {
            SPFieldUserValue field = new SPFieldUserValue(site.RootWeb, userFieldValue);
            if (field.User != null)
            {
                Init(field.User);
            }
            else
            {
                SPGroup group = site.RootWeb.SiteGroups[field.LookupValue];
                Init(group);
            }
        }

        /// <summary>
        /// Создание на основе значения поля элемента списка
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <param name="fieldValue">Значение поля</param>
        public UserData(SPWeb web, SPFieldUserValue fieldValue)
        {
            if (fieldValue.User != null)
            {
                Init(fieldValue.User);
            }
            else
            {
                if (string.IsNullOrEmpty(fieldValue.LookupValue))
                {
                    this.Name = string.Empty;
                    this.Email = string.Empty;
                    //this.Type = UserDataType.Empty;
                }
                else
                {
                    try
                    {
                        SPGroup group = web.SiteGroups[fieldValue.LookupValue];
                        Init(group);
                    }
                    catch
                    {
#warning need log with warning level
                        this.Id = fieldValue.LookupId;
                        this.Name = fieldValue.LookupValue;
                        this.LoginName = fieldValue.User.LoginName;
                        //this.Type = UserDataType.UnKnown;
                    }
                }
            }
        }

        /// <summary>
        /// Создание на основе SPGroup
        /// </summary>
        /// <param name="group"></param>
        public UserData(Microsoft.SharePoint.SPGroup group)
        {
            Init(group);
        }

        /// <summary>
        /// Создание пустого элемента на основе ИД и Имени
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public UserData(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Email = string.Empty;
            this.LoginName = string.Empty;
            //this.Type = UserDataType.UnKnown;
        }

        /// <summary>
        /// Создание на основе логина
        /// </summary>
        /// <param name="loginName"></param>
        public UserData(string loginName)
        {
            this.LoginName = loginName;
            ///this.Type = UserDataType.UnKnown;
        }



        /// <summary>
        /// Получение пустого объекта
        /// </summary>
        /// <returns></returns>
        public static UserData GetEmpty()
        {
            UserData result = new UserData(string.Empty, string.Empty/*, UserDataType.Empty*/);
            return result;
        }

        /// <summary>
        /// Заполнение значениями на основе объекта SPUser 
        /// </summary>
        /// <param name="user"></param>
        private void Init(Microsoft.SharePoint.SPUser user)
        {
            if (user == null)
            {
                //this.Type = UserDataType.Empty;
                return;
            }

            this.Sid = user.Sid;
            this.Id = user.ID;
            this.Name = user.Name;
            this.IsSiteAdmin = user.IsSiteAdmin;
            this.Email = user.Email;
            if (!string.IsNullOrEmpty(user.LoginName))
            {
                this.LoginName = user.LoginName.ToLower();
            }
            else
            {
                this.LoginName = string.Empty;
            }

            if (user.IsDomainGroup)
            {
                //this.Type = UserDataType.WindowsGroup;
            }
            else
            {
                //this.Type = UserDataType.Login;
            }
            // this.IsDomainGroup = user.IsDomainGroup;
        }

        /// <summary>
        /// Заполнение значениями на основе объекта SPGroup
        /// </summary>
        /// <param name="group"></param>
        private void Init(Microsoft.SharePoint.SPGroup group)
        {
            if (group == null)
            {
                //this.Type = UserDataType.Empty;
                return;
            }

            this.Id = group.ID;
            this.Name = group.Name;
            this.LoginName = group.LoginName.ToLower();
            //this.Type = UserDataType.SPGroup;
            // this.IsDomainGroup = user.IsDomainGroup;
        }

        /// <summary>
        /// Получение форматированной строки
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}({1})", this.Name/*, this.Type*/);
        }
    }
}