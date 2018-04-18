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
        private long number;
        private DepositeType dType;
        [Required]
        [Range(0, 10000)]
        private float balance;
        private DateTime createDate;
        private bool smsNotify;
        private bool banking;
        private Owner owner1; 
        private History history1;
        [XmlElement(ElementName = "number")]
        public long Number { get => number; set => number = value; }
        [XmlElement(ElementName = "depositeType")]
        public DepositeType DType { get => dType; set => dType = value; }
        [XmlElement(ElementName = "balance")]
        public float Balance { get => balance; set => balance = value; }
        [XmlElement(ElementName = "createDate")]
        public DateTime CreateDate { get => createDate; set => createDate = value; }
        [XmlElement(ElementName = "smsNotify")]
        public bool SmsNotify { get => smsNotify; set => smsNotify = value; }
        [XmlElement(ElementName = "banking")]
        public bool Banking { get => banking; set => banking = value; }
        [XmlElement(ElementName = "owner")]
        public Owner Owner1 { get => owner1; set => owner1 = value; }
        [XmlElement(ElementName = "history")]
        public History History1 { get => history1; set => history1 = value; }

        public override string ToString()
        {
            string deposite = "";
            switch(this.DType)
            {
                case DepositeType.term: deposite = "Срочный сберегательный вклад"; break;
                case DepositeType.accumulative: deposite = "Накопительный вклад"; break;
                case DepositeType.call: deposite = "Вклад до вотребования"; break;
            }
            return "ФИО: " + this.Owner1.Surname + " " + this.Owner1.Name + " " + this.Owner1.Midname + Environment.NewLine +
                "№" + this.Number + Environment.NewLine +
                "Номер паспорта: " + this.Owner1.PassNumber + Environment.NewLine +
                "Дата рождения: " + this.Owner1.BirthDate.ToShortDateString() + Environment.NewLine +
                "Тип вклада: " + deposite + Environment.NewLine +
                "Баланс: " + this.Balance + Environment.NewLine +
                "Дата создания: " + this.CreateDate + Environment.NewLine +
                "СМС уведомления: " + (this.SmsNotify ? "подключено" : "не подключено") + Environment.NewLine +
                "Интернет-банкинг: " + (this.Banking ? "подключено" : "не подключено") + Environment.NewLine;
        }

        public Account()
        {
            Number = count;
            DType = 0;
            Balance = new Random().Next(1000) + 1000;
            CreateDate = DateTime.Now;
            SmsNotify = true;
            Banking = true;
        }
        public Account(XElement xe)
        {
            Owner1 = new Owner(xe.Element("owner").Element("name").Value, xe.Element("owner").Element("midname").Value, xe.Element("owner").Element("surname").Value, long.Parse(xe.Element("owner").Element("passNumber").Value), DateTime.Parse(xe.Element("owner").Element("birthDate").Value));
            DType = (DepositeType)int.Parse(xe.Element("depositeType").Value);
            Balance = float.Parse(xe.Element("balance").Value);
            SmsNotify = bool.Parse(xe.Element("smsNotify").Value);
            Banking = bool.Parse(xe.Element("banking").Value);
            Number = long.Parse(xe.Element("number").Value);
            CreateDate = DateTime.Parse(xe.Element("createDate").Value);
        }
        public Account(string _name, string _midname, string _surname, long _passNumber, DateTime _birthDate, int _dType, float _balance, bool _smsNotify, bool _banking, OperationType opType)
        {
            Owner1 = new Owner(_name, _midname, _surname, _passNumber, _birthDate);
            Number = count;
            DType = (DepositeType)_dType;
            Balance = _balance;
            CreateDate = DateTime.Now;
            SmsNotify = _smsNotify;
            Banking = _banking;
            History1 = new History(_balance, opType);
        }

        public static string AccountHistory(long passNumber)
        {
            XDocument xdoc = XDocument.Load(Account.mainFilePath);

            var items1 = from xe in xdoc.Element("ArrayOfAccount").Elements("account")
                         where (xe.Element("owner").Element("passNumber").Value.Equals(passNumber.ToString()))
                         select new Account
                         {
                             History1 = new History(float.Parse(xe.Element("history").Element("sum").Value), (OperationType)int.Parse(xe.Element("history").Element("opType").Value), DateTime.Parse(xe.Element("history").Element("date").Value))
                         };
            if (items1.Count() == 0) throw new Exception("Счёт не найден");
            StringBuilder buffer = new StringBuilder();
            foreach (Account acc in items1)
            {
                string buf = acc.History1.OperType.ToString() + " " + acc.History1.Sum + " " + acc.History1.Date + Environment.NewLine;
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
                                         where (xe.Element("number").Value.Equals(search1))
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
                                items1 = items1.Where(n => (Matches(n.Owner1.Surname, FIO[0]) && Matches(n.Owner1.Name, FIO[1]) && Matches(n.Owner1.Midname, FIO[2])));
                                break;
                            }
                        case 2: items1 = items1.Where(n => n.Balance == float.Parse(search1)); break;
                        case 3: items1 = items1.Where(n => n.DType.Equals((DepositeType)int.Parse(search1))); break;
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
                case 1: return items.OrderByDescending(n => n.CreateDate);
                case 2: return items.OrderBy(n => n.CreateDate);
                case 3: return items.OrderBy(n => n.DType);
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
            private DateTime date;
            private OperationType operType;
            private float sum;

            [XmlElement(ElementName = "date")]
            public DateTime Date { get => date; set => date = value; }
            [XmlElement(ElementName = "opType")]
            public OperationType OperType { get => operType; set => operType = value; }
            [XmlElement(ElementName = "sum")]
            public float Sum { get => sum; set => sum = value; }

            public History()
            {
                Sum = 0f;
                OperType = OperationType.deposit;
                Date = DateTime.Now;
            }
            public History(float _count, OperationType _opType)
            {
                Sum = _count;
                OperType = _opType;
                Date = DateTime.Now;
            }
            public History(float _count, OperationType _opType, DateTime _date)
            {
                Sum = _count;
                OperType = _opType;
                Date = _date;
            }
        }

        [Serializable]
        public class Owner
        {
            Owner()
            {
                Name = "Ivan";
                Midname = "Ivanov";
                Surname = "Ivanovich";
                PassNumber = new Random().Next(900000) + 100000;
                BirthDate = DateTime.Now;
            }
            public Owner(string _name, string _midname, string _surname, long _passNumber, DateTime _birthDate)
            {
                Name = _name;
                Midname = _midname;
                Surname = _surname;
                PassNumber = _passNumber;
                BirthDate = _birthDate;
            }
            private string surname;
            private string name;
            private string midname;
            private long passNumber;
            private DateTime birthDate;

            [XmlElement(ElementName = "surname")]
            [Required(AllowEmptyStrings = false)]
            [RegularExpression(@"[a-zA-Z]+")]
            public string Surname { get => surname; set => surname = value; }
            [XmlElement(ElementName = "name")]
            [Required(AllowEmptyStrings = false)]
            [RegularExpression(@"[a-zA-Z]+")]
            public string Name { get => name; set => name = value; }
            [XmlElement(ElementName = "midname")]
            [Required(AllowEmptyStrings = false)]
            [RegularExpression(@"[a-zA-Z]+")]
            public string Midname { get => midname; set => midname = value; }
            [Required]
            [Range(0, 10000000000)]
            [XmlElement(ElementName = "passNumber")]
            public long PassNumber { get => passNumber; set => passNumber = value; }
            [XmlElement(ElementName = "birthDate")]
            public DateTime BirthDate { get => birthDate; set => birthDate = value; }
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

