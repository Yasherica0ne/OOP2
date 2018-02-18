using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace Lab2
{
    [Serializable]
    //[XmlRoot(Namespace = "lab2")]
    [XmlType("account")]
    public class Account
    {
        [XmlIgnore]
        public static long count = 0;
        [XmlElement(ElementName = "number")]
        public long number { get; set; }
        [XmlElement(ElementName = "depositeType")]
        public DepositeType dType { get; set; }
        [XmlElement(ElementName = "balance")]
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

        public Account()
        {
            number = count;
            dType = 0;
            balance = new Random().Next(1000) + 1000;
            createDate = DateTime.Now;
            smsNotify = true;
            banking = true;
        }
        public Account(string _name, string _midname, string _surname, long _passNumber, DateTime _birthDate, int _dType, float _balance, bool _smsNotify, bool _banking, float sum, OperationType opType)
        {
            owner = new Owner(_name, _midname, _surname, _passNumber, _birthDate);
            number = count;
            dType = (DepositeType)_dType;
            balance = _balance;
            createDate = DateTime.Now;
            smsNotify = _smsNotify;
            banking = _banking;
            history = new History(sum, opType);
        }

        public static string AccountHistory(long passNumber)
        {
            XDocument xdoc = XDocument.Load("accounts.xml");
            var items1 = from xe in xdoc.Element("ArrayOfAccount").Elements("account")
                         where (xe.Element("owner").Element("passNumber").Value.Equals(passNumber.ToString()))
                         select new Account
                         {
                             history = new History(float.Parse(xe.Element("history").Element("sum").Value), (OperationType)int.Parse(xe.Element("history").Element("opType").Value), DateTime.Parse(xe.Element("history").Element("date").Value))
                         };
            if (items1.Count() == 0) throw new Exception("Счёт не найден");
            StringBuilder buffer = new StringBuilder();
            foreach(Account acc in items1)
            {
                string buf = acc.history.operType.ToString() + " " + acc.history.sum + " " + acc.history.date + "\n";
                buffer.Insert(buffer.Length, buf);
            }
            return buffer.ToString();
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
        public string surname { get; set; }
        [XmlElement(ElementName = "name")]
        public string name { get; set; }
        [XmlElement(ElementName = "midname")]
        public string midname { get; set; }
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
    public static class XmlSerializeWrapper
    {
        public static void Serialize<T>(T obj, string filename)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));

            using (FileStream fs = new FileStream(filename, FileMode.Truncate))
            {
                formatter.Serialize(fs, obj);
            }
        }

        public static T Deserialize<T>(string filename)
        {
            T obj;
            using (FileStream fs = new FileStream(filename, FileMode.Truncate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                obj = (T)formatter.Deserialize(fs);
            }
            return obj;
        }
        public static string ShowInfo(string name)
        {
            string[] FIO = name.Split(' ');
            XDocument xdoc = XDocument.Load("accounts.xml");
            IEnumerable<Account> items1 = from xe in xdoc.Element("ArrayOfAccount").Elements("account")
                         where (xe.Element("number").Value.Equals(FIO[0]))
                         select new Account
                         {
                             owner = new Owner(FIO[2], FIO[3], FIO[1], long.Parse(xe.Element("owner").Element("passNumber").Value), DateTime.Parse(xe.Element("owner").Element("birthDate").Value)),
                             dType = (DepositeType)int.Parse(xe.Element("depositeType").Value),
                             balance = float.Parse(xe.Element("balance").Value),
                             smsNotify = bool.Parse(xe.Element("smsNotify").Value),
                             banking = bool.Parse(xe.Element("banking").Value),
                             number = long.Parse(xe.Element("number").Value),
                             createDate = DateTime.Parse(xe.Element("createDate").Value)
                         };
            var acc = items1.First();
            return "ФИО: " + acc.owner.surname + " " + acc.owner.name + " " + acc.owner.midname + "\n" +
                "№" + acc.number + "\n" +
                "Номер паспорта: " + acc.owner.passNumber + "\n" +
                "Дата рождения: " + acc.owner.birthDate + "\n" +
                "Тип вклада: " + acc.dType + "\n" +
                "Баланс: " + acc.balance + "\n" +
                "Дата создания: " + acc.createDate + "\n" +
                "СМС уведомления: " + (acc.smsNotify ? "подключено" : "не подключено") + "\n" +
                "Интернет-банкинг: " + (acc.banking ? "подключено" : "не подключено") + "\n";
        }
    }
}
