using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;


namespace Lab2
{
    [Serializable]
    //[XmlRoot(Namespace = "lab2")]
    [XmlType("account")]
    public class Account
    {
        [XmlIgnore]
        public static long count = 0;
        public static string mainFilePath = "accounts.xml";
        [XmlElement(ElementName = "number")]
        public long number { get; set; }
        [XmlElement(ElementName = "depositeType")]
        public DepositeType dType { get; set; }
        [XmlElement(ElementName = "balance")]
        [Required]
        [Range(0, 10000)]
        public float balance { get; set; }
        [XmlElement(ElementName = "createDate")]
        public DateTime createDate { get; set; }
        [XmlElement(ElementName = "smsNotify")]
        public bool smsNotify { get; set; }
        [XmlElement(ElementName = "banking")]
        public bool banking { get; set; }
        [XmlElement(ElementName = "owner")]
        public Owner owner { get; set; }
        [XmlElement(ElementName = "history")]
        public History history { get; set; }

        public override string ToString()
        {
            string deposite = "";
            switch(this.dType)
            {
                case DepositeType.term: deposite = "Срочный сберегательный вклад"; break;
                case DepositeType.accumulative: deposite = "Накопительный вклад"; break;
                case DepositeType.call: deposite = "Вклад до вотребования"; break;
            }
            return "ФИО: " + this.owner.surname + " " + this.owner.name + " " + this.owner.midname + Environment.NewLine +
                "№" + this.number + Environment.NewLine +
                "Номер паспорта: " + this.owner.passNumber + Environment.NewLine +
                "Дата рождения: " + this.owner.birthDate + Environment.NewLine +
                "Тип вклада: " + deposite + Environment.NewLine +
                "Баланс: " + this.balance + Environment.NewLine +
                "Дата создания: " + this.createDate + Environment.NewLine +
                "СМС уведомления: " + (this.smsNotify ? "подключено" : "не подключено") + Environment.NewLine +
                "Интернет-банкинг: " + (this.banking ? "подключено" : "не подключено") + Environment.NewLine;
        }

        public Account()
        {
            number = count;
            dType = 0;
            balance = new Random().Next(1000) + 1000;
            createDate = DateTime.Now;
            smsNotify = true;
            banking = true;
        }
        public Account(XElement xe)
        {
            owner = new Owner(xe.Element("owner").Element("name").Value, xe.Element("owner").Element("midname").Value, xe.Element("owner").Element("surname").Value, long.Parse(xe.Element("owner").Element("passNumber").Value), DateTime.Parse(xe.Element("owner").Element("birthDate").Value));
            dType = (DepositeType)int.Parse(xe.Element("depositeType").Value);
            balance = float.Parse(xe.Element("balance").Value);
            smsNotify = bool.Parse(xe.Element("smsNotify").Value);
            banking = bool.Parse(xe.Element("banking").Value);
            number = long.Parse(xe.Element("number").Value);
            createDate = DateTime.Parse(xe.Element("createDate").Value);
        }
        public Account(string _name, string _midname, string _surname, long _passNumber, DateTime _birthDate, int _dType, float _balance, bool _smsNotify, bool _banking, OperationType opType)
        {
            owner = new Owner(_name, _midname, _surname, _passNumber, _birthDate);
            number = count;
            dType = (DepositeType)_dType;
            balance = _balance;
            createDate = DateTime.Now;
            smsNotify = _smsNotify;
            banking = _banking;
            history = new History(_balance, opType);
        }

        public static string AccountHistory(long passNumber)
        {
            XDocument xdoc = XDocument.Load(Account.mainFilePath);
            var items1 = from xe in xdoc.Element("ArrayOfAccount").Elements("account")
                         where (xe.Element("owner").Element("passNumber").Value.Equals(passNumber.ToString()))
                         select new Account
                         {
                             history = new History(float.Parse(xe.Element("history").Element("sum").Value), (OperationType)int.Parse(xe.Element("history").Element("opType").Value), DateTime.Parse(xe.Element("history").Element("date").Value))
                         };
            if (items1.Count() == 0) throw new Exception("Счёт не найден");
            StringBuilder buffer = new StringBuilder();
            foreach (Account acc in items1)
            {
                string buf = acc.history.operType.ToString() + " " + acc.history.sum + " " + acc.history.date + Environment.NewLine;
                buffer.Insert(buffer.Length, buf);
            }
            return buffer.ToString();
        }

        private static bool Matches(string str1, string str2)
        {
            string pattern1 = str2.Substring(0, 3);
            string pattern2 = str2.Substring(str2.Length - 3, 3);
            string pattern3 = str2.Substring((str2.Length / 2) - 1, 3);
            if (Regex.IsMatch(str1, $"^{pattern1}") && Regex.IsMatch(str1, $"[{str2}]")) return true;
            else if(Regex.IsMatch(str1, $"{pattern3}") && Regex.IsMatch(str1, $"[{str2}]")) return true;
            else if (Regex.IsMatch(str1, $"{pattern2}$") && Regex.IsMatch(str1, $"[{str2}]")) return true;
            else return false;
        }

        public static string AccountsToString(IEnumerable<Account> accs)
        {
            StringBuilder buffer = new StringBuilder();
            foreach (Account acc in accs)
            {
                buffer.AppendLine(acc.ToString());
            }
            return buffer.ToString();
        }

        public static IEnumerable<Account> Search(List<string> search, int[] numbers)
        {
            XDocument xdoc = XDocument.Load(Account.mainFilePath);
            IEnumerable<Account> items1 = null;
            bool isFirst = true;
            int i = 1;
            foreach(string search1 in search)
            {
                if (isFirst)
                {
                    isFirst = false;
                    switch (numbers[0])
                    {
                        case 0:
                            {
                                items1 = from xe in xdoc.Element("ArrayOfAccount").Elements("account")
                                         where xe.Element("number").Value.Equals(search1)
                                         select new Account(xe);
                                break;
                            }
                        case 1:
                            {
                                string[] FIO = search1.Split(' ');
                                items1 = from xe in xdoc.Element("ArrayOfAccount").Elements("account")
                                         //where ((xe.Element("owner").Element("surname").Value.Equals(FIO[0]) && xe.Element("owner").Element("name").Value.Equals(FIO[1]) && xe.Element("owner").Element("midname").Value.Equals(FIO[2]))
                                         //||
                                         where (Matches(xe.Element("owner").Element("surname").Value, FIO[0]) && Matches(xe.Element("owner").Element("name").Value, FIO[1]) && Matches(xe.Element("owner").Element("midname").Value, FIO[2]))
                                         select new Account(xe);
                                break;
                            }
                        case 2:
                            {
                                items1 = from xe in xdoc.Element("ArrayOfAccount").Elements("account")
                                         where xe.Element("balance").Value.Equals(search1)
                                         select new Account(xe);
                                break;
                            }
                        case 3:
                            {
                                items1 = from xe in xdoc.Element("ArrayOfAccount").Elements("account")
                                         where xe.Element("depositeType").Value.Equals(search1)
                                         select new Account(xe);
                                break;
                            }
                    }
                }
                else
                {
                    switch (numbers[i++])
                    {
                        case 1:
                            {
                                string[] FIO = search1.Split(' ');
                                items1 = items1.Where(n => (Matches(n.owner.surname, FIO[0]) && Matches(n.owner.name, FIO[1]) && Matches(n.owner.midname, FIO[2])));
                                break;
                            }
                        case 2: items1 = items1.Where(n => n.balance == float.Parse(search1)); break;
                        case 3: items1 = items1.Where(n => n.dType.Equals((DepositeType)int.Parse(search1))); break;
                    }
                }
            }
            if (items1 == null) throw new Exception("Не найдено ниодного элемента");
            return items1;
        }

        public static IEnumerable<Account> Sort(int ch)
        {
            List<Account> items = XmlSerializeWrapper.Deserialize<List<Account>>(Account.mainFilePath);
            switch (ch)
            {
                case 1: return items.OrderByDescending(n => n.createDate);
                case 2: return items.OrderBy(n => n.createDate);
                case 3: return items.OrderBy(n => n.dType);
                default: throw new Exception("Ошибка сортировки");
            }
        }

        public enum OperationType
        {
            [XmlEnum("0")]
            deposit = 0,
            [XmlEnum("1")]
            withdraw
        }

        public class History
        {
            [XmlElement(ElementName = "date")]
            public DateTime date;
            [XmlElement(ElementName = "opType")]
            public OperationType operType;
            [XmlElement(ElementName = "sum")]
            public float sum;
            public History()
            {
                sum = 0f;
                operType = OperationType.deposit;
                date = DateTime.Now;
            }
            public History(float _count, OperationType _opType)
            {
                sum = _count;
                operType = _opType;
                date = DateTime.Now;
            }
            public History(float _count, OperationType _opType, DateTime _date)
            {
                sum = _count;
                operType = _opType;
                date = _date;
            }
        }

        [Serializable]
        public class Owner
        {
            Owner()
            {
                name = "Ivan";
                midname = "Ivanov";
                surname = "Ivanovich";
                passNumber = new Random().Next(900000) + 100000;
                birthDate = DateTime.Now;
            }
            public Owner(string _name, string _midname, string _surname, long _passNumber, DateTime _birthDate)
            {
                name = _name;
                midname = _midname;
                surname = _surname;
                passNumber = _passNumber;
                birthDate = _birthDate;
            }
            [XmlElement(ElementName = "surname")]
            [Required(AllowEmptyStrings = false)]
            [RegularExpression(@"[a-zA-Z]+")]
            public string surname { get; set; }
            [XmlElement(ElementName = "name")]
            [Required(AllowEmptyStrings = false)]
            [RegularExpression(@"[a-zA-Z]+")]
            public string name { get; set; }
            [XmlElement(ElementName = "midname")]
            [Required(AllowEmptyStrings = false)]
            [RegularExpression(@"[a-zA-Z]+")]
            public string midname { get; set; }
            [Required]
            [Range(0, 10000000000)]
            [XmlElement(ElementName = "passNumber")]
            public long passNumber { get; set; }
            [XmlElement(ElementName = "birthDate")]
            public DateTime birthDate { get; set; }
        }

        [Serializable]
        public enum DepositeType
        {
            [XmlEnum("0")]
            term = 0,
            [XmlEnum("1")]
            accumulative,
            [XmlEnum("2")]
            call
        }
    }
    public static class XmlSerializeWrapper
    {
        public static void Serialize<T>(T obj, string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(fileName, FileMode.Truncate))
            {
                formatter.Serialize(fs, obj);
            }
        }
        public static T Deserialize<T>(string fileName)
        {
            T obj;
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                obj = (T)formatter.Deserialize(fs);
            }
            return obj;
        }
        public static bool isEmpty()
        {
            long count;
            using (FileStream fs = new FileStream(Account.mainFilePath, FileMode.OpenOrCreate))
            {
                count = fs.Length;
            }
            if (count < 30) return true;
            else return false; 
        }
    }
}

